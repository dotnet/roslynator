// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Formatting.CodeFixes.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class AddEmptyLineBetweenUsingDirectivesWithDifferentRootNamespaceTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AddEmptyLineBetweenUsingDirectivesWithDifferentRootNamespace;

        public override DiagnosticAnalyzer Analyzer { get; } = new EmptyLineBetweenUsingDirectiveAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new SyntaxTriviaCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineBetweenUsingDirectivesWithDifferentRootNamespace)]
        public async Task Test_EmptyLine()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;[||]
using Microsoft.CodeAnalysis;[||]
using System.Threading;

class C
{
}
", @"
using System;

using Microsoft.CodeAnalysis;

using System.Threading;

class C
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineBetweenUsingDirectivesWithDifferentRootNamespace)]
        public async Task TestNoDiagnostic_SameRootNamespace()
        {
            await VerifyNoDiagnosticAsync(@"
using System;
using System.Linq;

class C
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineBetweenUsingDirectivesWithDifferentRootNamespace)]
        public async Task TestNoDiagnostic_UsingStatic()
        {
            await VerifyNoDiagnosticAsync(@"
using Microsoft.CodeAnalysis;
using static System.IO.Path;

class C
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineBetweenUsingDirectivesWithDifferentRootNamespace)]
        public async Task TestNoDiagnostic_Alias()
        {
            await VerifyNoDiagnosticAsync(@"
using Microsoft.CodeAnalysis;
using I = System.Int32;

class C
{
}
");
        }
    }
}
