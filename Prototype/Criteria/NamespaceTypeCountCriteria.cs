using System;
using System.Collections.Generic;
using Prototype.ExtensionMethods;

namespace Prototype.Criteria
{
	public class NamespaceTypeCountCriteria : ICriteria
	{
		private const int FlagOk = 30;
		private readonly Type[] types;
		private Dictionary<string, ICollection<Type>> namespaceDictionary = new();

		public static string Name => "Complexity of Type Count per Namespace";

		public NamespaceTypeCountCriteria(Type[] types)
		{
			this.types = types ?? throw new ArgumentNullException(nameof(types));
			foreach (var type in types)
			{
				namespaceDictionary.AddOrCreate(type.Namespace, type);
			}
		}
		
		public double CalculateComplexity()
		{
			var complexity = 0.0;
			foreach (var (_, types) in namespaceDictionary)
			{
				complexity += types.Count;
			}

			return complexity / namespaceDictionary.Count;
		}

		public ICollection<ProblemReport> GenerateProblemReports()
		{
			var problems = new List<ProblemReport>();
			foreach (var (ns, types) in namespaceDictionary)
			{
				if (types.Count > FlagOk)
				{
					problems.Add(new ProblemReport(
						"", ns,
						$"Namespace has {types.Count} types. Maximum set is 30.",
						Name, "Reduce amount of types per namespace by refactoring into more namespaces " +
						      "or removing not needed types.")
					);
				}
			}
			return problems;
		}
	}
}