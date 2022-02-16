using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prototype.Criteria;
using Prototype.DataStructures;
using Prototype.ExtensionMethods;

namespace Prototype.Evaluators
{
	public static class EvaluatorHelper
	{
		/// <summary>
		/// Creates an <see cref="ICriteria"/> with the provided constructor and passes the provided data to it.
		/// Executes complexity calculation and generation of <see cref="ProblemReport"/>.
		/// Return the <see cref="Task"/> for both processes as a <see cref="Tuple"/>
		/// </summary>
		/// <param name="data">data to be provided to the <see cref="ICriteria"/></param>
		/// <param name="ctor">constructor of the <see cref="ICriteria"/> as lambda</param>
		/// <typeparam name="TD">Type of data</typeparam>
		/// <typeparam name="TV">Type of <see cref="ICriteria"/></typeparam>
		/// <returns><see cref="Tuple"/> with first item <see cref="Task"/> for complexity calculation and second item <see cref="Task"/> for problem generation.</returns>
		public static (Task<double>, Task<ICollection<ProblemReport>>) EvaluateCriteria<TD, TV>(TD data, Func<TD, TV> ctor) where TV : ICriteria
		{
			ICriteria criteria = ctor.Invoke(data);
			return(criteria.CalculateComplexity(), criteria.GenerateProblemReports());
		}

		/// <summary>
		/// Creates Dictionaries for <see cref="Task"/> management needed by an <see cref="IEvaluator"/> with the given keys.
		/// </summary>
		/// <param name="keys">List of <see cref="Type"/> that are used as key for the dictionaries</param>
		/// <returns><see cref="Tuple{T1, T2}"/> where the first item is the complexity dictionary and the second item is the problem dictionary.</returns>
		public static (IDictionary<string, IList<Task<double>>>,
			IDictionary<string, IList<Task<ICollection<ProblemReport>>>>)
			CreateTaskDictionaries(IEnumerable<Type> keys)
		{
			var cTasks = new Dictionary<string, IList<Task<double>>>();
			var pTasks = new Dictionary<string, IList<Task<ICollection<ProblemReport>>>>();
			foreach (var c in keys)
			{
				cTasks.Add(c.Name, new List<Task<double>>());
				pTasks.Add(c.Name, new List<Task<ICollection<ProblemReport>>>());
			}
			return (cTasks, pTasks);
		}
		
		/// <summary>
		/// Runs the provided <see cref="ICriteria"/> with the provided data.
		/// </summary>
		/// <param name="criteria">List of <see cref="ICriteria"/> types to be executed.</param>
		/// <param name="data">Data on which the criteria should be executed</param>
		/// <typeparam name="TD">Type of the provided data</typeparam>
		/// <returns><see cref="Tuple{T1, T2}"/> where the first item is the complexity dictionary and the second item is the problem dictionary.</returns>
		public static (IDictionary<string, IList<Task<double>>>,
			IDictionary<string, IList<Task<ICollection<ProblemReport>>>>) RunEvaluation<TD>(IEnumerable<Type> criteria, TD data)
		{
			var enumerable = criteria as Type[] ?? criteria.ToArray();
			var (complexityTasks, problemTasks) = CreateTaskDictionaries(enumerable);
			foreach (var c in enumerable)
			{
				var ctor = c.GetConstructor(new [] {data.GetType()});
				var (cTasks, pTasks) = EvaluateCriteria(data, t => (ICriteria)ctor?.Invoke(new object[] {t}));
				complexityTasks[c.Name].Add(cTasks);
				problemTasks[c.Name].Add(pTasks);
			}

			return (complexityTasks, problemTasks);
		}
		
		/// <summary>
		/// Goes through all <see cref="Task"/> provided, normalizes the result according to provided lambda and adds the result to the result dictionary.
		/// </summary>
		/// <param name="complexityTasks">Dictionary of <see cref="Task"/> to be awaited.</param>
		/// <param name="results">Dictionary were the results should be stored.</param>
		/// <param name="normalize">Function to normalize results with.</param>
		public static async Task ProcessComplexities(IDictionary<string, IList<Task<double>>> complexityTasks, 
			IDictionary<string, double> results, Func<double, double> normalize)
		{
			foreach (var (criteria, complexityList) in complexityTasks)
			{
				await Task.WhenAll(complexityList);
				var value = complexityList.Sum(complexity => complexity.Result);

				value = normalize(value);
				results.CreateOrIncrease(criteria, Math.Round(value, 4));
			}
		}
		
		/// <summary>
		/// Goes through all <see cref="Task"/> provided and adds the results to the result dictionary.
		/// </summary>
		/// <param name="problemTasks">Dictionary of <see cref="Task"/> to be awaited.</param>
		/// <param name="results">Dictionary were the results should be stored.</param>
		public static async Task ProcessProblems(IDictionary<string, IList<Task<ICollection<ProblemReport>>>> problemTasks, Dictionary<string, ICollection<ProblemReport>> results)
		{
			foreach (var (criteria, problemList) in problemTasks)
			{
				await Task.WhenAll(problemList);
				foreach (var pTask in problemList)
				{
					results.AddOrCreate(criteria, pTask.Result);
				}
			}
		}
		
		
	}
}