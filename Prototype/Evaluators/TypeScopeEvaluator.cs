using Prototype.Criteria;
using System;
using System.Collections.Concurrent;
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
        private readonly IList<string> _criteria = new List<string> {nameof(MemberCountCriteria), nameof(OverloadCriteria), nameof(MemberPrefixCriteria)};

        public async Task<(Dictionary<string, ICollection<ProblemReport>> problems, Dictionary<string, double> complexities)> Evaluate(Assembly assembly)
        {
            _assemblyTypes = assembly.GetExportedTypes(); //get all public types of assembly
            Console.WriteLine("Starting Type Scope");
            await EvaluateMember();
            return (_problems, _complexities);
        }

        private void InitTaskDictionaries()
        {
            this._complexityTasks = new Dictionary<string, IList<Task<double>>>();
            this._problemTasks = new Dictionary<string, IList<Task<ICollection<ProblemReport>>>>();
            foreach (var c in _criteria)
            {
                this._complexityTasks.Add(c, new List<Task<double>>());
                this._problemTasks.Add(c, new List<Task<ICollection<ProblemReport>>>());
            }
        }

        private async Task EvaluateMember()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            InitTaskDictionaries();
            DoEvaluation();
            await Task.WhenAll(ProcessComplexities(), ProcessProblems());
            sw.Stop();
            Console.WriteLine($"Finished Type Scope in {sw.ElapsedMilliseconds}ms");
        }

        private void DoEvaluation()
        {
            foreach(var type in _assemblyTypes)
            {
                EvaluateCriteria(type, nameof(MemberCountCriteria), type => new MemberCountCriteria(type));
                EvaluateCriteria(type, nameof(MemberPrefixCriteria), type => new MemberPrefixCriteria(type));
                EvaluateCriteria(type, nameof(OverloadCriteria), type => new OverloadCriteria(type));
            }
        }
        
        private void EvaluateCriteria<TV>(Type type, string name, Func<Type, TV> ctor) where TV : ICriteria
        {
            ICriteria criteria = ctor.Invoke(type);
            _complexityTasks[name].Add(criteria.CalculateComplexity());
            _problemTasks[name].Add(criteria.GenerateProblemReports());
        }

        private async Task ProcessComplexities()
        {
            foreach (var (criteria, complexityList) in _complexityTasks)
            {
                await Task.WhenAll(complexityList);
                var value = complexityList.Sum(complexity => complexity.Result);

                value /= _assemblyTypes.Length;
                _complexities.CreateOrIncrease(criteria, Math.Round(value, 4));
            }
        }

        private async Task ProcessProblems()
        {
            foreach (var (criteria, problemList) in _problemTasks)
            {
                await Task.WhenAll(problemList);
                foreach (var pTask in problemList)
                {
                    _problems.AddOrCreate(criteria, pTask.Result);
                }
                
            }
        }
    }
}
