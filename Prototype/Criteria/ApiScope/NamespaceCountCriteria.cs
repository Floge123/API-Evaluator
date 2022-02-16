using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prototype.DataStructures;

namespace Prototype.Criteria.ApiScope
{
	public class NamespaceCountCriteria : ICriteria
	{
		private const int FlagOk = 30;
		private const double ComplexityExponent = 1.5;
		private readonly int _count;

		public NamespaceCountCriteria(ICollection<Type> types)
		{
			HashSet<string> namespaces = new();
			foreach (var type in types)
			{
				namespaces.Add(type.Namespace);
			}

			_count = namespaces.Count;
		}
		
		public async Task<double> CalculateComplexity()
		{
			return await Task.Run(() => Math.Pow(_count, ComplexityExponent));
		}

		public async Task<ICollection<ProblemReport>> GenerateProblemReports()
		{
			return await Task.Run(() =>
			{
				var problems = new List<ProblemReport>();
				if (_count > FlagOk)
				{
					problems.Add(new ProblemReport(
						"", "",
						$"Assembly has {_count} namespaces. Maximum set to 30.",
						nameof(NamespaceCountCriteria), "Reduce the number of namespaces used in the assembly. If possible, merge " +
						      "similar namespaces."));
				}

				return problems;
			});
		}
	}
}