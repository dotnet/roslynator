// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Roslynator
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Filter
    {
        internal static Filter EntireInput { get; } = new(@"\A.*\z", RegexOptions.Singleline);

        public Filter(
            string pattern,
            bool isNegative = false) : this(pattern, RegexOptions.None, isNegative: isNegative)
        {
        }

        public Filter(
            string pattern,
            RegexOptions options,
            bool isNegative = false) : this(new Regex(pattern, options), isNegative: isNegative)
        {
        }

        public Filter(
            Regex regex,
            bool isNegative = false,
            int groupNumber = -1,
            Func<string, bool> predicate = null)
        {
            Regex = regex ?? throw new ArgumentNullException(nameof(regex));

            Debug.Assert(groupNumber < 0 || regex.GetGroupNumbers().Contains(groupNumber), groupNumber.ToString());

            GroupNumber = groupNumber;
            IsNegative = isNegative;
            Predicate = predicate;
        }

        public Regex Regex { get; }

        public bool IsNegative { get; }

        public int GroupNumber { get; }

        public Func<string, bool> Predicate { get; }

        public string GroupName => Regex.GroupNameFromNumber(GroupNumber);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{nameof(IsNegative)} = {IsNegative}  {Regex}";

        public Match Match(string input)
        {
            Match match = Regex.Match(input);

            return Match(match);
        }

        internal Match Match(Match match)
        {
            if (Predicate != null)
            {
                if (GroupNumber < 1)
                {
                    while (match.Success)
                    {
                        if (Predicate.Invoke(match.Value))
                            return (IsNegative) ? null : match;

                        match = match.NextMatch();
                    }
                }
                else
                {
                    while (match.Success)
                    {
                        Group group = match.Groups[GroupNumber];

                        if (group.Success
                            && Predicate.Invoke(group.Value))
                        {
                            return (IsNegative) ? null : match;
                        }

                        match = match.NextMatch();
                    }
                }
            }
            else if (GroupNumber < 1)
            {
                if (match.Success)
                    return (IsNegative) ? null : match;
            }
            else
            {
                Group group = match.Groups[GroupNumber];

                if (group.Success)
                    return (IsNegative) ? null : match;
            }

            return (IsNegative)
                ? System.Text.RegularExpressions.Match.Empty
                : null;
        }

        internal bool IsMatch(string input)
        {
            return Match(input) != null;
        }

        internal bool IsMatch(Match match)
        {
            return Match(match) != null;
        }
    }
}
