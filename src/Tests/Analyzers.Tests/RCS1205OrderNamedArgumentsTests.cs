// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1205OrderNamedArgumentsTests : AbstractCSharpDiagnosticVerifier<OrderNamedArgumentsAnalyzer, BaseArgumentListCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.OrderNamedArguments;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OrderNamedArguments)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(string a, string b, string c)
    {
        M([|c: ""c"", a: ""a"", b: ""b""|]);
    }
}
", @"
class C
{
    void M(string a, string b, string c)
    {
        M(a: ""a"", b: ""b"", c: ""c"");
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OrderNamedArguments)]
        public async Task Test_OptionalArguments()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(string a, string b, string c, string d = null)
    {
        M([|c: ""c"", a: ""a"", b: ""b""|]);
    }
}
", @"
class C
{
    void M(string a, string b, string c, string d = null)
    {
        M(a: ""a"", b: ""b"", c: ""c"");
    }
}
");
        }
    }
}
