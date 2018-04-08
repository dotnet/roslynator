// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text.RegularExpressions;

#pragma warning disable RCS1023, RCS1033, RCS1049

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal class UseBitwiseOperationInsteadOfCallingHasFlag
    {
        public void Do()
        {
            RegexOptions options = RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase;

            if (options.HasFlag(RegexOptions.IgnoreCase)) { }

            if (!options.HasFlag(RegexOptions.IgnoreCase)) { }

            if (options.HasFlag(RegexOptions.IgnoreCase) == true) { }

            if (options.HasFlag(RegexOptions.IgnoreCase) == false) { }

            if ( /**/ Options.HasFlag(RegexOptions.IgnoreCase /**/ ).Equals(true)) { }
        }

        public RegexOptions Options { get; }
    }
}
