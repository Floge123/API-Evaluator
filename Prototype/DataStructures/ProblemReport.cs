using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype
{
    class ProblemReport
    {
        public string Type { get; set; }
        public string Location { get; set; }
        public string Problem { get; set; }
        public string Criteria { get; set; }
        public Extent Significance { get; set; }
        public string Solution { get; set; }

        public ProblemReport(string type, string location, string problem, 
                             string criteria, string solution)
        {
            this.Type = type;
            this.Location = location;
            this.Problem = problem;
            this.Criteria = criteria;
            this.Solution = solution;
        }

        public override string ToString()
        {
            return "Type: " + Type + "\n"
                + "Location: " + Location + "\n"
                + "Problem: " + Problem + "\n"
                + "Criteria: " + Criteria + "\n"
                + "Solution: " + Solution + "\n";
        }
    }
}
