using System;
using System.Collections.Generic;
using System.Reflection;
using Prototype.ExtensionMethods;

namespace Prototype.Criteria
{
    /// <summary>
    /// When searching for a specific member, the prefix of that method should help narrow down the search.
    /// This criteria measures the amount of members with the same prefix, which in result would slow down the search
    /// for the correct member.
    /// The prefx here is defined as the first 3 letters of the member.
    /// </summary>
    public class MemberPrefixCriteria : ICriteria
    {
        /// <summary>
        /// A type should have no more than 10 members with the same prefix
        /// </summary>
        private const int FLAG_OK = 10;

        private Type type;
        private Dictionary<string, ICollection<MemberInfo>> memberPrefixes = new();

        public static string Name { get { return "Complexity of Member prefix"; } }
        public MemberPrefixCriteria(Type type)
        {
            this.type = type;
            foreach (var member in type.GetMembers())
            {
                //get all prefixes with the memberInfo
                if (member.Name.Length <= 3) continue;
                memberPrefixes.AddOrCreate(member.Name.Substring(0, 3), member);
            }
            foreach (var members in memberPrefixes)
            {
                if (members.Value.Count == 1)
                {
                    //if only one member with prefix found, ignore prefix
                    memberPrefixes.Remove(members.Key);
                }
            }
        }

        /// <summary>
        /// The complexity of the search for the correct member increases by n/2 with
        /// n being the number of members with the same prefix.
        /// Add up this complexity for all prefixes with more than 1 member.
        /// </summary>
        /// <returns>complexity of member prefixes</returns>
        public double CalculateComplexity()
        {
            double complexity = 0.0;
            foreach (var members in memberPrefixes)
            {
                complexity += members.Value.Count / 2;
            }
            return complexity;
        }

        public ICollection<ProblemReport> GenerateProblemReports()
        {
            ICollection<ProblemReport> problemReports = new List<ProblemReport>();
            foreach (var members in memberPrefixes)
            {
                if (members.Value.Count > FLAG_OK)
                {
                    problemReports.Add(new ProblemReport(
                        type.Name, $"Prefix {members.Key} -> {members.Value.ValuesToString()}",
                        $"Type has more than {FLAG_OK} members with prefix {members.Key}. Has {members.Value.Count}.",
                        Name, "Rename members or remove some to reduce count of members with same prefix."
                    ));
                }
            }
            return problemReports;
        }
    }
}
