using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype
{
    class ProblemReport
    {
        private string type;
        private string location;
        private string problem;
        private Criteria criteria;
        private Extent significance;
        private Extent magnitude;
        private string solution;

        public ProblemReport(string type, string location, string problem, 
                             Criteria criteria, Extent significance, 
                             Extent magnitude, string solution)
        {
            this.type = type;
            this.location = location;
            this.problem = problem;
            this.criteria = criteria;
            this.significance = significance;
            this.magnitude = magnitude;
            this.solution = solution;
        }

        public override string ToString()
        {
            return "Type: " + type + "\n"
                + "Location: " + location + "\n"
                + "Problem: " + problem + "\n"
                + "Criteria: " + criteria + "\n"
                + "Significance of Criteria: " + significance + "\n"
                + "Magnitude of Problem: " + magnitude + "\n"
                + "Solution: " + solution + "\n";
        }
    }
}
