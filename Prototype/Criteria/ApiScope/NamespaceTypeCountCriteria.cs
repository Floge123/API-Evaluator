using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prototype.DataStructures;
using Prototype.ExtensionMethods;

namespace Prototype.Criteria.ApiScope
{
	public class NamespaceTypeCountCriteria : ICriteria
	{
		private const int FlagOk = 30;
		private readonly IDictionary<string, ICollection<Type>> _namespaceDictionary = new Dictionary<string, ICollection<Type>>();

		public NamespaceTypeCountCriteria(IEnumerable<Type> types)
		{
			foreach (var type in types)
			{
				_namespaceDictionary.AddOrCreate(type.Namespace, type);
			}
		}
		
		public async Task<double> CalculateComplexity()
		{
			return await Task.Run(() =>
			{
				var complexity = 0.0;
				foreach (var (_, t) in _namespaceDictionary)
				{
					complexity += t.Count;
				}

				return complexity / _namespaceDictionary.Count;
			});
		}

		public async Task<ICollection<ProblemReport>> GenerateProblemReports()
		{
			return await Task.Run(() =>
			{
				var problems = new List<ProblemReport>();
				foreach (var (ns, t) in _namespaceDictionary)
				{
					if (t.Count > FlagOk)
					{
						problems.Add(new ProblemReport(
							"", ns,
							$"Namespace has {t.Count} types. Maximum set is 30.",
							nameof(NamespaceTypeCountCriteria), "Reduce amount of types per namespace by refactoring into more namespaces " +
							      "or removing not needed types.")
						);
					}
				}

				return problems;
			});
		}
	}
}