using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prototype.DataStructures;

namespace Prototype.Criteria.ApiScope
{
	public class TypeCountCriteria : ICriteria
	{
		private const int FlagOk = 20;
		private readonly int _typeCount;

		public TypeCountCriteria(IEnumerable<Type> types)
		{
			_typeCount = types.Count();
		}
		
		public async Task<double> CalculateComplexity()
		{
			return await Task.FromResult(_typeCount);
		}

		public async Task<ICollection<ProblemReport>> GenerateProblemReports()
		{
			return await Task.Run(() =>
			{
				var problems = new List<ProblemReport>();
				if (_typeCount > FlagOk)
				{
					problems.Add(new ProblemReport(
						"", "",
						$"Assembly has {_typeCount} types.",
						nameof(TypeCountCriteria), "This is just for info, no fix needed.")
					);
				}

				return problems;
			});
		}
	}
}