// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1102MakeClassStaticTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.MakeClassStatic;

        public override DiagnosticAnalyzer Analyzer { get; } = new MakeClassStaticAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new ClassDeclarationCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.MakeClassStatic)]
        public async Task TestNoDiagnostic_SealedClass()
        {
            await VerifyNoDiagnosticAsync(@"
sealed class Foo
{
    const string K = null;
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.MakeClassStatic)]
        public async Task TestNoDiagnostic_ImplementsInterface()
        {
            await VerifyNoDiagnosticAsync(@"
class Foo : IFoo
{
    const string K = null;
}

interface IFoo
{
}
");
        }
    }
}
