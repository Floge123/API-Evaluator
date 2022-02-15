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
		
		public EvaluationBase(params IEvaluator[] evaluators)
		{
			foreach (var e in evaluators)
			{
				_evaluators.Add(e);
			}
		}

		public void AddEvaluator(IEvaluator evaluator)
		{
			if (evaluator is null) 
				throw new ArgumentNullException(nameof(evaluator));

			_evaluators.Add(evaluator);
		}

		public void RemoveEvaluator(IEvaluator evaluator)
		{
			if (evaluator is null) 
				throw new ArgumentNullException(nameof(evaluator));

			_evaluators.Remove(evaluator);
		}

		public void RemoveEvaluator(Type evaluatorType)
		{
			if (evaluatorType.FindInterfaces(
				(t, c) => t.ToString() == c?.ToString(),
				"Prototype.Evaluators.IEvaluator").Length == 0)
			{
				return; //check if provided Type is an IEvaluator
			}

			var e = _evaluators.First(item => item.GetType() == evaluatorType);
			_evaluators.Remove(e);
		}
		
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