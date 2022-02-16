using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Prototype.DataStructures;
using Prototype.Evaluators;
using Prototype.ExtensionMethods;

namespace Prototype
{
	public class EvaluationBase
	{
		private readonly ISet<IEvaluator> _evaluators = new HashSet<IEvaluator>();
		
		/// <summary>
		/// <see cref="EvaluationBase"/> constructor that takes an arbitrary amount of <see cref="IEvaluator"/>.
		/// </summary>
		/// <param name="evaluators">Arbitrary amount of <see cref="IEvaluator"/> seperated by a comma.</param>
		public EvaluationBase(params IEvaluator[] evaluators)
		{
			foreach (var e in evaluators)
			{
				_evaluators.Add(e);
			}
		}

		/// <summary>
		/// Adds a provided <see cref="IEvaluator"/> to a set or does nothing if it already was added.
		/// </summary>
		/// <param name="evaluator"><see cref="IEvaluator"/> to be added.</param>
		/// <exception cref="ArgumentNullException">If provided <see cref="IEvaluator"/> was null</exception>
		public void AddEvaluator(IEvaluator evaluator)
		{
			if (evaluator is null) 
				throw new ArgumentNullException(nameof(evaluator));

			_evaluators.Add(evaluator);
		}

		/// <summary>
		/// Remove a provided <see cref="IEvaluator"/> from set.
		/// </summary>
		/// <param name="evaluator"><see cref="IEvaluator"/> to be removed.</param>
		/// <exception cref="ArgumentNullException">If provided <see cref="IEvaluator"/> was null</exception>
		public void RemoveEvaluator(IEvaluator evaluator)
		{
			if (evaluator is null) 
				throw new ArgumentNullException(nameof(evaluator));

			_evaluators.Remove(evaluator);
		}

		/// <summary>
		/// Remove the <see cref="IEvaluator"/> from set that matches the provided <see cref="Type"/>.
		/// Provided <see cref="Type"/> has to implement <see cref="IEvaluator"/>.
		/// </summary>
		/// <param name="evaluatorType"><see cref="Type"/> of <see cref="IEvaluator"/> to be removed.</param>
		public void RemoveEvaluator(Type evaluatorType)
		{
			if (evaluatorType.FindInterfaces(
				(t, c) => t.ToString() == c?.ToString(),
				"Prototype.Evaluators.IEvaluator").Length == 0)
			{
				return; //check if provided Type is an IEvaluator, else do nothing
			}

			var e = _evaluators.First(item => item.GetType() == evaluatorType);
			_evaluators.Remove(e);
		}
		
		/// <summary>
		/// Do the evaluation on a given <see cref="Assembly"/>.
		/// Runs all added <see cref="IEvaluator"/> and concatenates their results.
		/// </summary>
		/// <param name="assembly"><see cref="Assembly"/> to be evaluated.</param>
		/// <returns><see cref="Tuple{T1, T2}"/> where the first item is the problem dictionary and the second item is the complexity dictionary.</returns>
		public (IDictionary<string, ICollection<ProblemReport>>, IDictionary<string, double>) EvaluateAssembly(Assembly assembly)
		{
			var problems = new Dictionary<string, ICollection<ProblemReport>>();
			var complexities = new Dictionary<string, double>();
			var locker = new object();
			Parallel.ForEach(
				_evaluators,
				() => (new Dictionary<string, ICollection<ProblemReport>>(), new Dictionary<string, double>()),
				(e, _, _) => e.Evaluate(assembly).Result,
				partial =>
				{
					var (p, c) = partial;
					lock (locker)
					{
						problems.AddAll(p);
						complexities.AddAll(c);
					}
				}
			);
			return (problems, complexities);
		}
	}
}