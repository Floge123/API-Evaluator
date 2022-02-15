namespace Prototype.DataStructures
{
    public class ProblemReport
    {
        public string Type { get; set; }
        public string Location { get; set; }
        public string Problem { get; set; }
        public string Criteria { get; set; }
        public string Solution { get; set; }

        public ProblemReport(string type, string location, string problem, 
                             string criteria, string solution)
        {
            Type = type;
            Location = location;
            Problem = problem;
            Criteria = criteria;
            Solution = solution;
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
