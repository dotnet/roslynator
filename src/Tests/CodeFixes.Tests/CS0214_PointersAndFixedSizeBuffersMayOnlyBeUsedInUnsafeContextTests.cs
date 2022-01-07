// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS0214_PointersAndFixedSizeBuffersMayOnlyBeUsedInUnsafeContextTests : AbstractCSharpCompilerDiagnosticFixVerifier<UnsafeCodeFixProvider>
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.CS0214_PointersAndFixedSizeBuffersMayOnlyBeUsedInUnsafeContext;

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS0201_OnlyAssignmentCallIncrementDecrementAndNewObjectExpressionsCanBeUsedAsStatement)]
        public async Task Test_SpanOfT()
        {
            await VerifyFixAsync(@"
using System;

class C
{
    void M()
    {
        var bytes = stackalloc byte[10];
    }
}
", @"
using System;

class C
{
    void M()
    {
        Span<byte> bytes = stackalloc byte[10];
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId, CodeFixIdentifiers.UseExplicitTypeInsteadOfVar, "System_Span_T"));
        }

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS0201_OnlyAssignmentCallIncrementDecrementAndNewObjectExpressionsCanBeUsedAsStatement)]
        public async Task Test_ReadOnlySpanOfT()
        {
            await VerifyFixAsync(@"
using System;

class C
{
    void M()
    {
        var bytes = stackalloc byte[10];
    }
}
", @"
using System;

class C
{
    void M()
    {
        ReadOnlySpan<byte> bytes = stackalloc byte[10];
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId, CodeFixIdentifiers.UseExplicitTypeInsteadOfVar, "System_ReadOnlySpan_T"));
        }
    }
}
