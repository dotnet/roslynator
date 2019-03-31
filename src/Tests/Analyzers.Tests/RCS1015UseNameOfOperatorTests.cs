// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Roslynator.CSharp.Tests;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1015UseNameOfOperatorTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseNameOfOperator;

        public override DiagnosticAnalyzer Analyzer { get; } = new UseNameOfOperatorAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new UseNameOfOperatorCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseNameOfOperator)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    public C(object parameter)
    {
        if (parameter == null)
        {
            throw new ArgumentNullException(
                [|""parameter""|],
                ""message"");
        }
    }
}
", @"
using System;

class C
{
    public C(object parameter)
    {
        if (parameter == null)
        {
            throw new ArgumentNullException(
                nameof(parameter),
                ""message"");
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseNameOfOperator)]
        public async Task TestNoDiagnostic_LanguageVersion()
        {
            await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    public C(object parameter)
    {
        if (parameter == null)
        {
            throw new ArgumentNullException(
                ""parameter"",
                ""message"");
        }
    }
}
", options: CSharpCodeVerificationOptions.DefaultWithCSharp5);
        }
    }
}
