// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Roslynator.Text.RegularExpressions
{
    internal sealed class GroupItem
    {
        internal GroupItem(Group group, in GroupDefinition groupDefinition, MatchItem matchItem)
        {
            Group = group;
            GroupDefinition = groupDefinition;
            MatchItem = matchItem;

            var captureItems = new List<CaptureItem>(Captures.Count);

            foreach (Capture capture in Captures)
                captureItems.Add(new CaptureItem(capture, this));

            CaptureItems = new CaptureItemCollection(captureItems);
        }

        public string Value => Group.Value;

        public int Index => Group.Index;

        public int Length => Group.Length;

        public int EndIndex => Group.Index + Group.Length;

        public bool Success => Group.Success;

        public MatchItem MatchItem { get; }

        public CaptureItemCollection CaptureItems { get; }

        public CaptureCollection Captures => Group.Captures;

        public Group Group { get; }

        public GroupDefinition GroupDefinition { get; }

        public int Number => GroupDefinition.Number;

        public string Name => GroupDefinition.Name;

        public override string ToString() => Group.ToString();
    }
}
