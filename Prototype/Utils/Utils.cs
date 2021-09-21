using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype
{
    static class Utils
    {
        public static void UpdateDictionaries(Dictionary<Criteria, List<ProblemReport>> problems,
                                              Dictionary<Criteria, int> scores,
                                              Criteria criteria, ProblemReport problemReport, int score)
        {
            if (problems.ContainsKey(criteria))
            {
                problems[criteria].Add(problemReport);
                scores[criteria] = score;
            }
            else
            {
                problems.Add(criteria, new ProblemReport[] { problemReport }.ToList());
                scores.Add(criteria, score);
            }
        }
    }
}
