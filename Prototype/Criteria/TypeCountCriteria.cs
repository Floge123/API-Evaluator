using System;
using System.Collections.Generic;

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
		
		public double CalculateComplexity()
		{
			return types.Length;
		}

		public ICollection<ProblemReport> GenerateProblemReports()
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
			return problems;
		}
	}
}