using Prototype.Criteria;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Prototype.ExtensionMethods;

namespace Prototype.Evaluators
{
    public class MemberEvaluator : IEvaluator
    {
        private Type[] _assemblyType;
        private Dictionary<string, ICollection<ProblemReport>> _problems;
        private Dictionary<string, double> _complexities;
        private Dictionary<string, IList<Task<double>>> _complexityTasks;
        private Dictionary<string, IList<Task<ICollection<ProblemReport>>>> _problemTasks;
        private readonly IList<string> _criteria = new List<string> {MemberCountCriteria.Name, MemberPrefixCriteria.Name};

        public async Task Evaluate(Assembly assembly,
            Dictionary<string, ICollection<ProblemReport>> problems,
            Dictionary<string, double> complexities)
        {
            _assemblyType = assembly.GetExportedTypes(); //get all public types of assembly
            this._problems = problems ?? throw new ArgumentNullException(nameof(problems));
            this._complexities = complexities ?? throw new ArgumentNullException(nameof(complexities));
            Console.WriteLine("Starting Member");
            await EvaluateMember();
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
            Console.WriteLine($"Finished Member in {sw.ElapsedMilliseconds}ms");
        }

        private void DoEvaluation()
        {
            foreach(var type in _assemblyType)
            {
                EvaluateCriteria(type, MemberCountCriteria.Name, type => new MemberCountCriteria(type));
                EvaluateCriteria(type, MemberPrefixCriteria.Name, type => new MemberPrefixCriteria(type));
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

                value /= _assemblyType.Length;
                _complexities.CreateOrIncrease(criteria, value);
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
