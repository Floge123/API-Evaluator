using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Prototype.Evaluators;

namespace Prototype
{
    internal static class Program
    {
        private static Assembly LoadAssembly(IReadOnlyList<string> consoleArgs)
        {
            Assembly assembly = null;
            string filepath;
            if (consoleArgs.Count < 1)
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
                assembly = Assembly.LoadFrom(filepath);
            }
            catch (Exception)
            {
                Console.WriteLine("Usage: Prototype.exe /{Filepath/}");
                Environment.Exit(-1);
            }

            return assembly;
        }

        private static async Task Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            Assembly assembly = LoadAssembly(args);

            var evaluator = new EvaluationBase(
                new ApiScopeEvaluator(),
                new MethodScopeEvaluator(),
                new TypeScopeEvaluator()
            );

            var (problems, complexities) =  evaluator.EvaluateAssembly(assembly);

            if (!Directory.Exists("results")) Directory.CreateDirectory("results");
            await Task.WhenAll(
                File.WriteAllTextAsync($"results\\{assembly.GetName().Name}_problems.json",
                    JsonConvert.SerializeObject(
                        problems.OrderBy(p => p.Key)
                            .ToDictionary(x => x.Key, x => x.Value)
                        )
                ),
                File.WriteAllTextAsync($"results\\{assembly.GetName().Name}_complexities.json",
                    JsonConvert.SerializeObject(
                        complexities.OrderBy(c => c.Key)
                            .ToDictionary(x => x.Key, x=> x.Value)
                        )
                )
            );
            sw.Stop();
            Console.WriteLine($"Successfully finished evaluation in {sw.ElapsedMilliseconds}ms.\n See results in [bin\\Debug\\net5.0\\results]");
        }
    }
}
