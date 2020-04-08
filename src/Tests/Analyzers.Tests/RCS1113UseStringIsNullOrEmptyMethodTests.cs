// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1113UseStringIsNullOrEmptyMethodTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseStringIsNullOrEmptyMethod;

        public override DiagnosticAnalyzer Analyzer { get; } = new UseStringIsNullOrEmptyMethodAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new BinaryExpressionCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseStringIsNullOrEmptyMethod)]
        public async Task Test_LogicalOr()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = default;
        if ([|s == null || s.Length == 0|]) { }
    }
}
", @"
class C
{
    void M()
    {
        string s = default;
        if (string.IsNullOrEmpty(s)) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseStringIsNullOrEmptyMethod)]
        public async Task Test_LogicalAnd()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = default;
        if ([|s != null && s.Length != 0|]) { }
    }
}
", @"
class C
{
    void M()
    {
        string s = default;
        if (!string.IsNullOrEmpty(s)) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseStringIsNullOrEmptyMethod)]
        public async Task TestNoDiagnostics()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        string s = default, s2 = default;

        if (s2 == null || s.Length == 0) { }

        if (s != null || s.Length == 0) { }

        if (s == s2 || s.Length == 0) { }

        if (s == null && s.Length == 0) { }

        if (s == null || s2.Length == 0) { }

        if (s == null || s.Length != 0) { }

        if (s == null || s.Length == 1) { }

        if (s == null || s == ""x"") { }

        if (s != null && s2 != string.Empty) { }

        if (s2 != null && s != string.Empty) { }

        if (s != null && s2 != """") { }

        if (s2 != null && s != """") { }

        if (s != null && s != ""x"") { }
    }
}
");
        }
    }
}
