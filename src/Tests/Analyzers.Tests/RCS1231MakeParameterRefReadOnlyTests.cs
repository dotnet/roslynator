// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1231MakeParameterRefReadOnlyTests : AbstractCSharpFixVerifier
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

        [Fact]
        public async Task TestNoDiagnostic_DuplicateParameterName()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;

readonly struct C
{
    void M(C c, C c)
    {
    }
}
", options: Options.AddAllowedCompilerDiagnosticId("CS0100"));
        }

        [Fact]
        public async Task TestNoDiagnostic_MethodReferencedAsMethodGroup()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    readonly struct B
    {
        public int P { get; }
    }

    bool M(B p) => p.P > 0;

    bool M(List<B> p) => p.Any(M);
}
");
        }

        [Fact]
        public async Task TestNoDiagnostic_LocalFunctionReferencedAsMethodGroup()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    readonly struct B
    {
        public int P { get; }
    }

    bool M(List<B> p)
    {
        return p.Any(M);

        bool M(B p2) => p2.P > 0;
    }
}
");
        }
    }
}
