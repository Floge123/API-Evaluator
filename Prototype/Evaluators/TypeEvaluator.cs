using Prototype.Criteria;
using Prototype.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Prototype.Evaluators
{
    public class TypeEvaluator : IEvaluator
    {
        private Type[] _assemblyTypes;
        private Dictionary<string, ICollection<ProblemReport>> _problems;
        private Dictionary<string, double> _complexities;
        private Dictionary<string, IList<Task<double>>> _complexityTasks;
        private Dictionary<string, IList<Task<ICollection<ProblemReport>>>> _problemTasks;
        private readonly IList<string> _criteria = new List<string> {TypeCountCriteria.Name, NamespaceTypeCountCriteria.Name, NamespaceCountCriteria.Name};

        public async Task Evaluate(Assembly assembly,
            Dictionary<string, ICollection<ProblemReport>> problems,
            Dictionary<string, double> complexities)
        {
            _assemblyTypes = assembly.GetExportedTypes();
            this._problems = problems ?? throw new ArgumentNullException(nameof(problems));
            this._complexities = complexities ?? throw new ArgumentNullException(nameof(complexities));
            //call all private evaluations
            Console.WriteLine("Starting Type");
            await EvaluateCounts();
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

        private async Task EvaluateCounts()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            InitTaskDictionaries();
            DoEvaluation();
            await Task.WhenAll(ProcessComplexities(), ProcessProblems());
            sw.Stop();
            Console.WriteLine($"Finished Type in {sw.ElapsedMilliseconds}ms");
        }
        
        private void DoEvaluation()
        {
            EvaluateCriteria(_assemblyTypes, TypeCountCriteria.Name, 
                assemblyTypes => new TypeCountCriteria(assemblyTypes));
            EvaluateCriteria(_assemblyTypes, NamespaceTypeCountCriteria.Name, 
                assemblyTypes => new NamespaceTypeCountCriteria(assemblyTypes));
            EvaluateCriteria(_assemblyTypes, NamespaceCountCriteria.Name,
                assemblyTypes => new NamespaceCountCriteria(assemblyTypes));
        }
        
        private void EvaluateCriteria<TV>(Type[] type, string name, Func<Type[], TV> ctor) where TV : ICriteria
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
