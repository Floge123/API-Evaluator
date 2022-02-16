using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Prototype.Criteria.TypeScope;
using Prototype.DataStructures;
using Prototype.ExtensionMethods;

namespace Prototype.Evaluators
{
    public class TypeScopeEvaluator : IEvaluator
    {
        private Type[] _assemblyTypes;
        private readonly Dictionary<string, ICollection<ProblemReport>> _problems = new();
        private readonly Dictionary<string, double> _complexities = new();
        private IDictionary<string, IList<Task<double>>> _complexityTasks;
        private IDictionary<string, IList<Task<ICollection<ProblemReport>>>> _problemTasks;
        private readonly IList<Type> _criteria = new List<Type> {typeof(MemberCountCriteria), typeof(OverloadCriteria), typeof(MemberPrefixCriteria)};

        public async Task<(Dictionary<string, ICollection<ProblemReport>> problems, Dictionary<string, double> complexities)> Evaluate(Assembly assembly)
        {
            _assemblyTypes = assembly.GetExportedTypes(); //get all public types of assembly
            Console.WriteLine("Starting Type Scope");
            await EvaluateTypeScope();
            return (_problems, _complexities);
        }

        private async Task EvaluateTypeScope()
        {
            var sw = new Stopwatch();
            sw.Start();
            (_complexityTasks, _problemTasks) = EvaluatorHelper.CreateTaskDictionaries(_criteria);
            DoEvaluation();
            await Task.WhenAll(
                EvaluatorHelper.ProcessComplexities(_complexityTasks,_complexities,v => v / _assemblyTypes.Length),
                EvaluatorHelper.ProcessProblems(_problemTasks, _problems)
            );
            sw.Stop();
            Console.WriteLine($"Finished Type Scope in {sw.ElapsedMilliseconds}ms");
        }

        private void DoEvaluation()
        {
            foreach(var type in _assemblyTypes)
            {
                var (cTasks, pTasks) = EvaluatorHelper.RunEvaluation(_criteria, type);
                _complexityTasks.Merge(cTasks);
                _problemTasks.Merge(pTasks);
            }
        }
    }
}
