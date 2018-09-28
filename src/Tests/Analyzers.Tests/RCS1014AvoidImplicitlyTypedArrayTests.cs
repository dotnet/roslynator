// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1014AvoidImplicitlyTypedArrayTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AvoidImplicitlyTypedArray;

        public override DiagnosticAnalyzer Analyzer { get; } = new AvoidImplicitlyTypedArrayAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new ImplicitArrayCreationExpressionCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AvoidImplicitlyTypedArray)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var x = [|new[]|] { """" };
    }
}
", @"
class C
{
    void M()
    {
        var x = new string[] { """" };
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AvoidImplicitlyTypedArray)]
        public async Task Test2()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    string[][] _f = [|new[]|]
    {
        /**/[|new[]|] { """" },
    };
}
", @"
class C
{
    string[][] _f = new string[][]
    {
        /**/new string[] { """" },
    };
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AvoidImplicitlyTypedArray)]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        var x = new[] { new { Value = """" } };
    }
}
");
        }
    }
}
