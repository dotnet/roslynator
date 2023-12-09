// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests;

public class CS8610_NullabilityOfReferenceTypesInTypeOfParameterDoesNotMatchOverriddenMemberTests : AbstractCSharpCompilerDiagnosticFixVerifier<TokenCodeFixProvider>
{
    public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.CS8610_NullabilityOfReferenceTypesInTypeOfParameterDoesNotMatchOverriddenMember;

    [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS8610_NullabilityOfReferenceTypesInTypeOfParameterDoesNotMatchOverriddenMember)]
    public async Task Test_Method()
    {
        await VerifyFixAsync(@"
using System.IO;
#nullable enable

abstract class C : TextWriter
{
    public override void WriteLine(string format, params object[] arg)
    {
    }
}
", @"
using System.IO;
#nullable enable

abstract class C : TextWriter
{
    public override void WriteLine(string format, params object?[] arg)
    {
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
    }
}
