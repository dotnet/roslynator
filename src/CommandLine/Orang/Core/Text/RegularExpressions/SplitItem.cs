// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Globalization;
using System.Text.RegularExpressions;

namespace Roslynator.Text.RegularExpressions
{
    internal abstract class SplitItem
    {
        internal SplitItem()
        {
        }

        public abstract string Value { get; }

        public abstract int Index { get; }

        public abstract int Length { get; }

        public abstract string Name { get; }

        public abstract int Number { get; }

        public abstract bool IsGroup { get; }

        public override string ToString() => Value;

        public static SplitItem Create(string value)
        {
            return new MatchSplitItem(value);
        }

        public static SplitItem Create(string value, int index, int name)
        {
            return new MatchSplitItem(value, index, name);
        }

        public static SplitItem Create(Group group, in GroupDefinition groupDefinition)
        {
            return new GroupSplitItem(group, groupDefinition);
        }

        private sealed class MatchSplitItem : SplitItem
        {
            public MatchSplitItem(string value)
            {
                Value = value;
                Number = 0;
            }

            public MatchSplitItem(string value, int index, int number)
            {
                Value = value;
                Index = index;
                Number = number;
            }

            public override string Value { get; }

            public override int Index { get; }

            public override int Length => Value.Length;

            public override string Name => Number.ToString(CultureInfo.CurrentCulture);

            public override int Number { get; }

            public override bool IsGroup => false;
        }

        private sealed class GroupSplitItem : SplitItem
        {
            public GroupSplitItem(Group group, in GroupDefinition groupDefinition)
            {
                Group = group;
                GroupDefinition = groupDefinition;
            }

            public Group Group { get; }

            public override string Value => Group.Value;

            public override int Index => Group.Index;

            public override int Length => Group.Length;

            public override string Name => GroupDefinition.Name;

            public override int Number => GroupDefinition.Number;

            public GroupDefinition GroupDefinition { get; }

            public override bool IsGroup => true;
        }
    }
}
