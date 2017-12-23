// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text.RegularExpressions;

namespace Roslynator.Metadata
{
    public class SampleDescriptor
    {
        private static readonly Regex _lfWithoutCr = new Regex(@"(?<!\r)\n");

        public SampleDescriptor(string before, string after)
        {
            if (before != null)
                Before = _lfWithoutCr.Replace(before, "\r\n");

            if (after != null)
                After = _lfWithoutCr.Replace(after, "\r\n");
        }

        public string Before { get; }
        public string After { get; }
    }
}
