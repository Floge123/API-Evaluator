using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Prototype
{
    class Program
    {
        private static void EvaluateAssembly(Assembly assembly, Dictionary<Criteria, List<ProblemReport>> problems, Dictionary<Criteria, int> scores)
        {
            var fas = JsonConvert.DeserializeObject<FlagsAndScores>(File.ReadAllText("FlagsAndScores.json"));

            List<IEvaluator> evaluatorList = new()
            {
                new MethodEvaluator(fas)
                //add all Evaluator here
            };

            foreach (var evaluator in evaluatorList)
            {
                //automaticly run all evaluations
                evaluator.Evaluate(assembly, problems, scores);
            }
        }

        private static void PrintProblems(Dictionary<Criteria, List<ProblemReport>> problems)
        {
            foreach (var entry in problems)
            {
                Console.WriteLine(entry.Key + " {");
                foreach (var problem in entry.Value)
                {
                    Console.WriteLine(problem);
                }
                Console.WriteLine("}");
            }
        }

        static void Main(string[] args)
        {
            Assembly assembly = Assembly.LoadFrom("Prototype.dll");
            var problems = new Dictionary<Criteria, List<ProblemReport>>();
            var scores = new Dictionary<Criteria, int>();

            EvaluateAssembly(assembly, problems, scores);
            PrintProblems(problems);
        }
    }
}
