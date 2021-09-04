// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;

namespace Roslynator.Spelling
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class SpellingCapture
    {
        public SpellingCapture(string value, int index, string containingValue = null, int containingValueIndex = -1)
        {
            Value = value;
            Index = index;
            ContainingValue = containingValue;
            ContainingValueIndex = containingValueIndex;
        }

        public string Value { get; }

        public int Index { get; }

        public int Length => Value.Length;

        public string ContainingValue { get; }

        public int ContainingValueIndex { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{Index}  {Length}  {Value}";
    }
}
