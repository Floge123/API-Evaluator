using Prototype;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Criteria
{
    class ParamCountCriteria : ICriteria
    {

        private const int FLAG_OK = 4;

        private Type typeInfo;
        private MethodInfo methodInfo;
        private readonly int paramCount;

        public static string Name { get { return "Parameter Count of Method"; } }

        public ParamCountCriteria(Type typeinfo, MethodInfo methodInfo)
        {
            this.typeInfo = typeinfo;
            this.methodInfo = methodInfo;
            this.paramCount = methodInfo.GetParameters().Length;
        }

        public double CalculateScore()
        {
            double score = 0;
            for (int i = 1; i <= paramCount; i++)
            {
                score += 2.5 * i;
            }
            return score;
        }

        public ProblemReport GenerateProblemReport()
        {
            if (paramCount > FLAG_OK)
            {
                return new ProblemReport(
                    typeInfo.Name, methodInfo.ToString(),
                    $"Method has more than {FLAG_OK} parameters. Has {paramCount}.",
                    Name, "Reduce number of parameters or provide overload with less parameters."
                );
            }
            return null;
        }
    }
}
