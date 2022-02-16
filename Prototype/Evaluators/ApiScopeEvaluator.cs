using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Prototype.Criteria;
using Prototype.Criteria.ApiScope;
using Prototype.DataStructures;

namespace Prototype.Evaluators
{
    public class ApiScopeEvaluator : IEvaluator
    {
        private Type[] _assemblyTypes;
        private readonly Dictionary<string, ICollection<ProblemReport>> _problems = new();
        private readonly Dictionary<string, double> _complexities = new();
        private IDictionary<string, IList<Task<double>>> _complexityTasks;
        private IDictionary<string, IList<Task<ICollection<ProblemReport>>>> _problemTasks;
        private readonly IList<Type> _criteria = new List<Type> {typeof(TypeCountCriteria), typeof(NamespaceTypeCountCriteria), typeof(NamespaceCountCriteria)};

        public async Task<(Dictionary<string, ICollection<ProblemReport>> problems, Dictionary<string, double> complexities)> Evaluate(Assembly assembly)
        {
            _assemblyTypes = assembly.GetExportedTypes();
            Console.WriteLine("Starting Api Scope");
            await EvaluateApiScope();
            return (_problems, _complexities);
        }

        private async Task EvaluateApiScope()
        {
            var sw = new Stopwatch();
            sw.Start();
            (_complexityTasks, _problemTasks) = EvaluatorHelper.CreateTaskDictionaries(_criteria);
            DoEvaluation();
            await Task.WhenAll(
                EvaluatorHelper.ProcessComplexities(_complexityTasks, _complexities, v => v),
                EvaluatorHelper.ProcessProblems(_problemTasks, _problems)
            );
            sw.Stop();
            
            Console.WriteLine($"Finished Api Scope in {sw.ElapsedMilliseconds}ms");
        }
        
        private void DoEvaluation()
        {
            foreach (var c in _criteria)
            {
                var ctor = c.GetConstructor(new [] {_assemblyTypes.GetType()});
                var (cTasks, pTasks) = EvaluatorHelper.EvaluateCriteria(_assemblyTypes, t => (ICriteria)ctor?.Invoke(new object[] {t}));
                _complexityTasks[c.Name].Add(cTasks);
                _problemTasks[c.Name].Add(pTasks);
            }
        }
    }
}
