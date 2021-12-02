using System;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.Criteria
{
    /// <summary>
    /// This criteria is relevant, when the user has to search for a specific member.
    /// This happens, when the user doesn't know the name of the member or has to guess the right name.
    /// An IDE should display all methods and the user can scroll through them, this criteria measures the complexity
    /// of this search.
    /// </summary>
    public class MemberCountCriteria : ICriteria
    {
        /// <summary>
        /// According to the rule of 30, a Type should have no more than 30 members on average.
        /// </summary>
        private const int FlagOk = 30;

        private readonly Type type;
        private readonly int memberCount;

        public static string Name => "Complexity of Member Count";

        public MemberCountCriteria(Type type)
        {
            this.type = type;
            //filter out the get/set methods for properties and constructors
            var m = (from t in type.GetMembers()
                    where !(t.Name.StartsWith("get_") 
                            || t.Name.StartsWith("set_") 
                            || t.Name.Equals(".ctor")
                            || t.Name.StartsWith("op_"))
                    select t).ToList();
            memberCount = m.Count;
        }

        /// <summary>
        /// In 1/3 of the cases, this criteria is relevant, so that the user has to search for members.
        /// With every member, the complexity of this search increases by 1/6.
        /// </summary>
        /// <returns>complexity of the member count</returns>
        public double CalculateComplexity()
        {
            return memberCount * (1.0 / 3.0) * (1.0 / 6.0);
        }

        public ICollection<ProblemReport> GenerateProblemReports()
        {
            var problemReports = new List<ProblemReport>();
            if (memberCount > FlagOk)
            {
                problemReports.Add(new ProblemReport(
                    type.Name, "",
                    $"Type has more than {FlagOk} members. Has {memberCount}.",
                    Name, "Reduce number of public members. Too many choices are" +
                    "overwhelming, when looking for correct member."
                ));
            }
            return problemReports;
        }
    }
}
