// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS0037CannotConvertNullToTypeBecauseItIsNonNullableValueTypeTests : AbstractCSharpCompilerDiagnosticFixVerifier<RemovePropertyOrFieldInitializerCodeFixProvider>
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.CS0037_CannotConvertNullToTypeBecauseItIsNonNullableValueType;

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS0037_CannotConvertNullToTypeBecauseItIsNonNullableValueType)]
        public async Task Test_RemovePropertyInitializer()
        {
            await VerifyFixAsync(@"
using System;

class C
{
    public StringSplitOptions Options { get; set } = null;
}
", @"
using System;

class C
{
    public StringSplitOptions Options { get; set }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId, CodeFixIdentifiers.RemovePropertyOrFieldInitializer));
        }

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS0037_CannotConvertNullToTypeBecauseItIsNonNullableValueType)]
        public async Task Test_RemovePropertyInitializer_NullForgivingOperator()
        {
            await VerifyFixAsync(@"
using System;

class C
{
    public StringSplitOptions Options { get; set } = null!;
}
", @"
using System;

class C
{
    public StringSplitOptions Options { get; set }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId, CodeFixIdentifiers.RemovePropertyOrFieldInitializer));
        }

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS0037_CannotConvertNullToTypeBecauseItIsNonNullableValueType)]
        public async Task Test_RemoveFieldInitializer()
        {
            await VerifyFixAsync(@"
using System;

class C
{
    public StringSplitOptions Options = null!;
}
", @"
using System;

class C
{
    public StringSplitOptions Options;
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId, CodeFixIdentifiers.RemovePropertyOrFieldInitializer));
        }
    }
}
