// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Roslynator.Text.RegularExpressions
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SplitCapture : ICapture
    {
        public SplitCapture(string value, int index)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Index = index;
        }

        public SplitCapture(Group group)
        {
            Group = group ?? throw new ArgumentNullException(nameof(group));
            Value = group.Value;
            Index = group.Index;
        }

        public string Value { get; }

        public int Index { get; }

        public int Length => Value.Length;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{Index}  {Length}  {Value}";

        public Group Group { get; }
    }
}
