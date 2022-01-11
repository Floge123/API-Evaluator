﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
		
		public async Task<double> CalculateComplexity()
		{
			var complexity = 0.0;
			foreach (var (_, t) in namespaceDictionary)
			{
				complexity += t.Count;
			}

			return await Task.FromResult(complexity / namespaceDictionary.Count);
		}

		public async Task<ICollection<ProblemReport>> GenerateProblemReports()
		{
			var problems = new List<ProblemReport>();
			foreach (var (ns, t) in namespaceDictionary)
			{
				if (t.Count > FlagOk)
				{
					problems.Add(new ProblemReport(
						"", ns,
						$"Namespace has {t.Count} types. Maximum set is 30.",
						Name, "Reduce amount of types per namespace by refactoring into more namespaces " +
						      "or removing not needed types.")
					);
				}
			}
			return await Task.FromResult(problems);
		}
	}
}