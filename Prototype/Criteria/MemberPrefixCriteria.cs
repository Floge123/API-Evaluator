using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prototype.ExtensionMethods;

namespace Prototype.Criteria
{
    /// <summary>
    /// When searching for a specific member, the prefix of that method should help narrow down the search.
    /// This criteria measures the amount of members with the same prefix, which in result would slow down the search
    /// for the correct member.
    /// The prefix here is defined as the first 3 letters of the member.
    /// </summary>
    public class MemberPrefixCriteria : ICriteria
    {
        /// <summary>
        /// A type should have no more than 10 members with the same prefix
        /// </summary>
        private const int FlagOk = 10;

        private readonly Type type;
        private readonly Dictionary<string, ICollection<MemberInfo>> memberPrefixes = new();

        public static string Name => "Complexity of Member prefix";

        public MemberPrefixCriteria(Type type)
        {
            this.type = type;
            var members = (from t in type.GetMembers()
                where !(t.Name.StartsWith("get_") 
                        || t.Name.StartsWith("set_") 
                        || t.Name.Equals(".ctor")
                        || t.Name.StartsWith("op_"))
                select t).ToList();
            foreach (var member in members.Where(member => member.Name.Length > 3))
            {
                memberPrefixes.AddOrCreate(member.Name[..3], member);
            }
            foreach (var (key, value) in memberPrefixes)
            {
                if (value.Count == 1)
                {
                    //if only one member with prefix found, ignore prefix
                    memberPrefixes.Remove(key);
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
            return memberPrefixes.Sum(members => members.Value.Count / 2.0);
        }

        public ICollection<ProblemReport> GenerateProblemReports()
        {
            var problemReports = new List<ProblemReport>();
            foreach (var (key, value) in memberPrefixes)
            {
                if (value.Count > FlagOk)
                {
                    problemReports.Add(new ProblemReport(
                        type.Name, $"Prefix {key} -> {value.ValuesToString()}",
                        $"Type has more than {FlagOk} members with prefix {key}. Has {value.Count}.",
                        Name, "Rename members or remove some to reduce count of members with same prefix."
                    ));
                }
            }
            return problemReports;
        }
    }
}
