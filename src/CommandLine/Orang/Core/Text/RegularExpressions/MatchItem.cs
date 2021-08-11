// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Roslynator.Text.RegularExpressions
{
    internal sealed class MatchItem
    {
        internal MatchItem(Match match, GroupDefinitionCollection groups)
        {
            Match = match;

            var items = new List<GroupItem>(groups.Count);

            foreach (GroupDefinition group in groups)
                items.Add(new GroupItem(Match.Groups[group.Number], group, this));

            GroupItems = new GroupItemCollection(items);
        }

        public string Value => Match.Value;

        public int Index => Match.Index;

        public int Length => Match.Length;

        public int EndIndex => Match.Index + Match.Length;

        public bool Success => Match.Success;

        public Match Match { get; }

        public GroupCollection Groups => Match.Groups;

        public GroupItemCollection GroupItems { get; }

        public override string ToString() => Value;
    }
}
