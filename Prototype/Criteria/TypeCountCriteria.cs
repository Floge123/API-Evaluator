using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prototype.Criteria
{
	public class TypeCountCriteria : ICriteria
	{
		private const int FlagOk = 20;
		private readonly Type[] types;
		
		public static string Name => "Complexity of Type Count";

		public TypeCountCriteria(Type[] types)
		{
			this.types = types ?? throw new ArgumentNullException(nameof(types));
		}
		
		public async Task<double> CalculateComplexity()
		{
			return await Task.FromResult(types.Length);
		}

		public async Task<ICollection<ProblemReport>> GenerateProblemReports()
		{
			var problems = new List<ProblemReport>();
			if (types.Length > FlagOk)
			{
				problems.Add(new ProblemReport(
					"", "",
					$"Assembly has {types.Length} types.",
					Name, "This is just for info, no fix needed.")
				);
			}
			return await Task.FromResult(problems);
		}
	}
}