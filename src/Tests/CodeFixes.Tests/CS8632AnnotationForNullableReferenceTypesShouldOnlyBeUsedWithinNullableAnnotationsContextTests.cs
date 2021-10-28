// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS8632AnnotationForNullableReferenceTypesShouldOnlyBeUsedWithinNullableAnnotationsContextTests : AbstractCSharpCompilerDiagnosticFixVerifier<TokenCodeFixProvider>
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.CS8632_AnnotationForNullableReferenceTypesShouldOnlyBeUsedWithinNullableAnnotationsContext;

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS8632_AnnotationForNullableReferenceTypesShouldOnlyBeUsedWithinNullableAnnotationsContext)]
        public async Task Test_Method()
        {
            await VerifyFixAsync(@"
class C
{
    public string? M()
    {
        return null;
    }
}
", @"
class C
{
    public string M()
    {
        return null;
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }
    }
}
