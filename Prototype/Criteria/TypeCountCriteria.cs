using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prototype.Criteria
{
	public class TypeCountCriteria : ICriteria
	{
		private const int FlagOk = 20;
		private readonly int typeCount;
		
		public static string Name => "Complexity of Type Count";

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
			var problems = new List<ProblemReport>();
			if (typeCount > FlagOk)
			{
				problems.Add(new ProblemReport(
					"", "",
					$"Assembly has {typeCount} types.",
					Name, "This is just for info, no fix needed.")
				);
			}
			return await Task.FromResult(problems);
		}
	}
}