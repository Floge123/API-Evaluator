using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Prototype.ExtendMethods;

namespace Prototype
{
    class Program
    {
        public static string Indent(int count)
        {
            return "".PadLeft(count);
        }

        private static void EvaluateAssembly(
            Assembly assembly,
            Dictionary<string, ICollection<ProblemReport>> problems,
            Dictionary<string, double> scores)
        {
            List<IEvaluator> evaluatorList = new()
            {
                new MethodEvaluator()
                //add all Evaluator here
            };

            foreach (var evaluator in evaluatorList)
            {
                //automaticly run all evaluations
                evaluator.Evaluate(assembly, problems, scores);
            }
        }

        static void Main(string[] args)
        {
            Assembly assembly = Assembly.LoadFrom("TestingAssemblies/Newtonsoft.Json.dll");
            var problems = new Dictionary<string, ICollection<ProblemReport>>();
            var scores = new Dictionary<string, double>();

            EvaluateAssembly(assembly, problems, scores);

            File.WriteAllText("problems.json", JsonConvert.SerializeObject(problems));
            File.WriteAllText("scores.json", JsonConvert.SerializeObject(scores));

        }
    }
}
