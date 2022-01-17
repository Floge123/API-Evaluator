using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
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
                new MemberEvaluator(),
                new TypeEvaluator()
                //add all Evaluator here
            };

            Parallel.ForEach(evaluatorList, e =>
            {
                e.Evaluate(assembly, problems, complexities);
            });
        }

        private static Assembly LoadAssembly(string[] consoleArgs)
        {
            string filepath;
            if (consoleArgs.Length < 2)
            {
                Console.WriteLine("Insert Filepath:");
                filepath = Console.ReadLine();
            }
            else
            {
                filepath = consoleArgs[0];
            }

            try
            {
                return Assembly.LoadFrom(filepath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
                Console.WriteLine("Usage: Prototype.exe /{Filepath/}");
                Console.WriteLine("Using default!");
                return Assembly.LoadFrom("TestingAssemblies/Newtonsoft.Json.dll");
            }
        }

        static async Task Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            Assembly assembly = LoadAssembly(args);

            var problems = new Dictionary<string, ICollection<ProblemReport>>();
            var complexities = new Dictionary<string, double>();

            EvaluateAssembly(assembly, problems, complexities);

            if (!Directory.Exists("results")) Directory.CreateDirectory("results");
            await Task.WhenAll(
                File.WriteAllTextAsync($"results\\{assembly.GetName().Name}_problems.json",
                    JsonConvert.SerializeObject(problems)),
                File.WriteAllTextAsync($"results\\{assembly.GetName().Name}_complexities.json",
                    JsonConvert.SerializeObject(complexities))
            );
            sw.Stop();
            Console.WriteLine($"Successfully finished evaluation in {sw.ElapsedMilliseconds}ms.\n See results in [bin\\Debug\\net5.0\\results");
        }
    }
}
