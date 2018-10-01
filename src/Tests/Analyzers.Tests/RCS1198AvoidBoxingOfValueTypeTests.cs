// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1198AvoidBoxingOfValueTypeTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AvoidBoxingOfValueType;

        public override DiagnosticAnalyzer Analyzer { get; } = new AvoidBoxingOfValueTypeAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new AvoidBoxingOfValueTypeCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AvoidBoxingOfValueType)]
        public async Task Test_Interpolation()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    public TimeSpan P { get; }

    void M()
    {
        var c = new C();

        var x = $""{[|c?.P.TotalMilliseconds|]}"";
    }
}
", @"
using System;

class C
{
    public TimeSpan P { get; }

    void M()
    {
        var c = new C();

        var x = $""{(c?.P.TotalMilliseconds).ToString()}"";
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AvoidBoxingOfValueType)]
        public async Task Test_Interpolation_NullableType()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        int? i = null;

        string s = $""{[|i|]}"";
    }
}
", @"
class C
{
    void M()
    {
        int? i = null;

        string s = $""{i?.ToString()}"";
    }
}
");
        }
    }
}
