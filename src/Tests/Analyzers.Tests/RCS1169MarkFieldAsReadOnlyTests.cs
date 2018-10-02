// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Analysis.MakeMemberReadOnly;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1169MarkFieldAsReadOnlyTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.MarkFieldAsReadOnly;

        public override DiagnosticAnalyzer Analyzer { get; } = new MarkFieldAsReadOnlyAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new MemberDeclarationCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.MarkFieldAsReadOnly)]
        public async Task TestNoDiagnostic_ReturnRef()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    int _f;

    ref int M()
    {
        return ref _f;
    }
}
");
        }
    }
}
