// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Roslynator.Text.RegularExpressions
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class RegexCapture : ICapture
    {
        public RegexCapture(Capture capture)
        {
            Capture = capture ?? throw new ArgumentNullException(nameof(capture));
        }

        public Capture Capture { get; }

        public string Value => Capture.Value;

        public int Index => Capture.Index;

        public int Length => Capture.Length;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{Index}  {Length}  {Value}";
    }
}
