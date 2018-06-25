// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

#pragma warning disable RCS1090

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1227ValidateArgumentsCorrectlyTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.ValidateArgumentsCorrectly;

        public override DiagnosticAnalyzer Analyzer { get; } = new ValidateArgumentsCorrectlyAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new ValidateArgumentsCorrectlyCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ValidateArgumentsCorrectly)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Collections.Generic;

class C
{
    IEnumerable<string> M(object p, object p2)
    {
        if (p == null)
            throw new ArgumentNullException(nameof(p));

        if (p2 == null)
            throw new ArgumentNullException(nameof(p2));

        [||]string s = null;
        yield return s;
    }
}
", @"
using System;
using System.Collections.Generic;

class C
{
    IEnumerable<string> M(object p, object p2)
    {
        if (p == null)
            throw new ArgumentNullException(nameof(p));

        if (p2 == null)
            throw new ArgumentNullException(nameof(p2));

        return MIterator();
        IEnumerable<string> MIterator()
        {
            string s = null;
            yield return s;
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ValidateArgumentsCorrectly)]
        public async Task TestNoDiagnostic_NoNullCheck()
        {
            await VerifyNoDiagnosticAsync(@"
using System;
using System.Collections.Generic;

class C
{
    IEnumerable<string> M(object p)
    {
        string s = null;
        yield return s;
    }
}
");
        }
    }
}
