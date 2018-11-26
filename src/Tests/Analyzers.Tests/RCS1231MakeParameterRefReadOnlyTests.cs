// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1231MakeParameterRefReadOnlyTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.MakeParameterRefReadOnly;

        public override DiagnosticAnalyzer Analyzer { get; } = new MakeParameterRefReadOnlyAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new MakeParameterRefReadOnlyCodeFixProvider();

        [Fact]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

readonly struct C
{
    void M(C [|c|])
    {
        IEnumerable<object> LF()
        {
            yield return null;
        }
    }
}
", @"
using System.Collections.Generic;

readonly struct C
{
    void M(in C c)
    {
        IEnumerable<object> LF()
        {
            yield return null;
        }
    }
}
");
        }

        [Fact]
        public async Task TestNoDiagnostic_Assigned()
        {
            await VerifyNoDiagnosticAsync(@"
readonly struct C
{
    void M(C c)
    {
        c = default(C);
    }
}
");
        }

        [Fact]
        public async Task TestNoDiagnostic_ReferencedInLocalFunction()
        {
            await VerifyNoDiagnosticAsync(@"
readonly struct C
{
    void M(C c)
    {
        void LF()
        {
            var x = c;
        }
    }
}
");
        }

        [Fact]
        public async Task TestNoDiagnostic_ReferencedInLambda()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Linq;

readonly struct C
{
    void M(C c)
    {
        var items = Enumerable.Empty<C>().Select(f => c);
    }
}
");
        }

        [Fact]
        public async Task TestNoDiagnostic_Iterator()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;

readonly struct C
{
    IEnumerable<object> M(C c)
    {
        yield return null;
    }
}
");
        }
    }
}
