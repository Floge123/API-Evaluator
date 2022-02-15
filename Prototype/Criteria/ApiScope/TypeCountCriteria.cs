using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prototype.Criteria.ApiScope
{
	public class TypeCountCriteria : ICriteria
	{
		private const int FlagOk = 20;
		private readonly int typeCount;

		public TypeCountCriteria(IEnumerable<Type> types)
		{
			typeCount = types.Count();
		}
		
		public async Task<double> CalculateComplexity()
		{
			return await Task.FromResult(typeCount);
		}

		public async Task<ICollection<ProblemReport>> GenerateProblemReports()
		{
			return await Task.Run(() =>
			{
				var problems = new List<ProblemReport>();
				if (typeCount > FlagOk)
				{
					problems.Add(new ProblemReport(
						"", "",
						$"Assembly has {typeCount} types.",
						nameof(TypeCountCriteria), "This is just for info, no fix needed.")
					);
				}

				return problems;
			});
		}
	}
}