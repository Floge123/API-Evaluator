using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Prototype.Evaluators;

namespace Prototype
{
    class Program
    {
        private static void EvaluateAssembly(
            Assembly assembly,
            Dictionary<string, ICollection<ProblemReport>> problems,
            Dictionary<string, double> complexities)
        {
            List<IEvaluator> evaluatorList = new()
            {
                new MethodEvaluator(),
                new MemberEvaluator()
                //add all Evaluator here
            };

            foreach (var evaluator in evaluatorList)
            {
                //automaticly run all evaluations
                evaluator.Evaluate(assembly, problems, complexities);
            }
        }

        static void Main(string[] args)
        {
            Assembly assembly;
            string filepath;
            if (args.Length < 2)
            {
                Console.WriteLine("Insert Filepath:");
                filepath = Console.ReadLine();
            } else
            {
                filepath = args[0];
            }

            try
            {
                assembly = Assembly.LoadFrom(filepath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
                Console.WriteLine("Usage: Prototype.exe /{Filepath/}");
                assembly = Assembly.LoadFrom("TestingAssemblies/Newtonsoft.Json.dll");
            }

            var problems = new Dictionary<string, ICollection<ProblemReport>>();
            var complexities = new Dictionary<string, double>();

            EvaluateAssembly(assembly, problems, complexities);

            if (!Directory.Exists("results")) Directory.CreateDirectory("results");
            File.WriteAllText($"results\\{assembly.GetName().Name}_problems.json", JsonConvert.SerializeObject(problems));
            File.WriteAllText($"results\\{assembly.GetName().Name}_complexities.json", JsonConvert.SerializeObject(complexities));
            Console.WriteLine("Successfully finished evaluation.\n See results in [bin\\Debug\\net5.0\\results");
        }
    }
}
