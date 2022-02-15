using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prototype.Criteria.ApiScope
{
	public class NamespaceCountCriteria : ICriteria
	{
		private const int FlagOk = 30;
		private const double ComplexityExponent = 1.5;
		private int count;

		public NamespaceCountCriteria(ICollection<Type> types)
		{
			HashSet<string> namespaces = new();
			foreach (var type in types)
			{
				namespaces.Add(type.Namespace);
			}

			count = namespaces.Count;
		}
		
		public async Task<double> CalculateComplexity()
		{
			return await Task.Run(() => Math.Pow(count, ComplexityExponent));
		}

		public async Task<ICollection<ProblemReport>> GenerateProblemReports()
		{
			return await Task.Run(() =>
			{
				var problems = new List<ProblemReport>();
				if (count > FlagOk)
				{
					problems.Add(new ProblemReport(
						"", "",
						$"Assembly has {count} namespaces. Maximum set to 30.",
						nameof(NamespaceCountCriteria), "Reduce the number of namespaces used in the assembly. If possible, merge " +
						      "similar namespaces."));
				}

				return problems;
			});
		}
	}
}