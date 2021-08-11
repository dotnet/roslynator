// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text.RegularExpressions;

namespace Roslynator.Text.RegularExpressions
{
    internal readonly struct ReplaceItem
    {
        internal ReplaceItem(Match match, string value, int index)
        {
            Match = match;
            Value = value;
            Index = index;
        }

        public override string ToString() => Value;

        public Match Match { get; }

        public string Value { get; }

        public int Index { get; }

        public int Length => Value.Length;
    }
}
