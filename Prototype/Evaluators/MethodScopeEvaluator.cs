using Prototype.Criteria;
using Prototype.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Prototype.Criteria.MethodScope;
using Prototype.Criteria.TypeScope;

namespace Prototype.Evaluators
{
    public class MethodScopeEvaluator : IEvaluator
    {
        private Type[] _assemblyTypes;
        private Dictionary<string, ICollection<ProblemReport>> _problems;
        private Dictionary<string, double> _complexities;
        private Dictionary<string, IList<Task<double>>> _complexityTasks;
        private Dictionary<string, IList<Task<ICollection<ProblemReport>>>> _problemTasks;
        private readonly IList<string> _criteria = new List<string> {ParamCountCriteria.Name, ReturnValueCriteria.Name};

        public async Task Evaluate(Assembly assembly,
            Dictionary<string, ICollection<ProblemReport>> problems,
            Dictionary<string, double> complexities)
        {
            _assemblyTypes = assembly.GetExportedTypes();
            this._problems = problems ?? throw new ArgumentNullException(nameof(problems));
            this._complexities = complexities ?? throw new ArgumentNullException(nameof(complexities));
            //call all private evaluations
            Console.WriteLine("Starting Method Scope");
            await EvaluateMethods();
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

        private async Task EvaluateMethods()
        {
            Stopwatch sw = new Stopwatch();
           sw.Start();
           InitTaskDictionaries();
           DoEvaluation();
           await Task.WhenAll(ProcessComplexities(), ProcessProblems());
           sw.Stop();
           Console.WriteLine($"Finished Method Scope in {sw.ElapsedMilliseconds}ms");
        }

        private void DoEvaluation()
        {
            foreach(var type in _assemblyTypes)
            {
                foreach (var method in type.GetMethods())
                {
                    EvaluateCriteria(method, ParamCountCriteria.Name, method => new ParamCountCriteria(method));
                    EvaluateCriteria(method, ReturnValueCriteria.Name, method => new ReturnValueCriteria(method));
                }
            }
        }
        
        private void EvaluateCriteria<TV>(MethodInfo method, string name, Func<MethodInfo, TV> ctor) where TV : ICriteria
        {
            ICriteria criteria = ctor.Invoke(method);
            _complexityTasks[name].Add(criteria.CalculateComplexity());
            _problemTasks[name].Add(criteria.GenerateProblemReports());
        }
        
        private async Task ProcessComplexities()
        {
            foreach (var (criteria, complexityList) in _complexityTasks)
            {
                await Task.WhenAll(complexityList);
                var value = complexityList.Sum(complexity => complexity.Result);
                
                //parameter count complexity and return value complexity per method
                value /= _assemblyTypes.Sum(type => type.GetMethods().Length);

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
