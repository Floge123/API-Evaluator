using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Prototype.Criteria.MethodScope;
using Prototype.DataStructures;
using Prototype.ExtensionMethods;

namespace Prototype.Evaluators
{
    public class MethodScopeEvaluator : IEvaluator
    {
        private readonly IList<Type> _criteria = new List<Type> {typeof(ParamCountCriteria), typeof(ReturnValueCriteria)};
        
        private Type[] _assemblyTypes;
        private IDictionary<string, IList<Task<double>>> _complexityTasks;
        private IDictionary<string, IList<Task<ICollection<ProblemReport>>>> _problemTasks;
        
        private readonly Dictionary<string, ICollection<ProblemReport>> _problems = new();
        private readonly Dictionary<string, double> _complexities = new();
        
        public async Task<(Dictionary<string, ICollection<ProblemReport>> problems, 
                           Dictionary<string, double> complexities)> Evaluate(Assembly assembly)
        {
            _assemblyTypes = assembly.GetExportedTypes();
            Console.WriteLine("Starting Method Scope");
            await EvaluateMethodScope();
            return (_problems, _complexities);
        }

        private async Task EvaluateMethodScope()
        {
           var sw = new Stopwatch();
           sw.Start();
           (_complexityTasks, _problemTasks) = EvaluatorHelper.CreateTaskDictionaries(_criteria);
           DoEvaluation();
           await Task.WhenAll(
               EvaluatorHelper.ProcessComplexities(_complexityTasks, _complexities, v => v / _assemblyTypes.Sum(type => type.GetMethods().Length)),
               EvaluatorHelper.ProcessProblems(_problemTasks, _problems)
            );
           sw.Stop();
           Console.WriteLine($"Finished Method Scope in {sw.ElapsedMilliseconds}ms");
        }

        private void DoEvaluation()
        {
            foreach(var type in _assemblyTypes)
            {
                foreach (var method in type.GetMethods())
                {
                    var (cTasks, pTasks) = EvaluatorHelper.RunEvaluation(_criteria, method);
                    _complexityTasks.Merge(cTasks);
                    _problemTasks.Merge(pTasks);
                }
            }
        }
    }
}
