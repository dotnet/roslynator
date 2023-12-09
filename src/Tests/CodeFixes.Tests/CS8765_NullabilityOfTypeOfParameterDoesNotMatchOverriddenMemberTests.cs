// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests;

public class CS8765_NullabilityOfTypeOfParameterDoesNotMatchOverriddenMemberTests : AbstractCSharpCompilerDiagnosticFixVerifier<TokenCodeFixProvider>
{
    public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.CS8765_NullabilityOfTypeOfParameterDoesNotMatchOverriddenMember;

    [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS8765_NullabilityOfTypeOfParameterDoesNotMatchOverriddenMember)]
    public async Task Test_Method_Parameters2()
    {
        await VerifyFixAsync(@"
using System.IO;
#nullable enable

abstract class C : TextWriter
{
    public override void Write(string format, object arg0, object arg1)
    {
    }
}
", @"
using System.IO;
#nullable enable

abstract class C : TextWriter
{
    public override void Write(string format, object? arg0, object? arg1)
    {
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
    }

    [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS8765_NullabilityOfTypeOfParameterDoesNotMatchOverriddenMember)]
    public async Task Test_Method_Parameters3()
    {
        await VerifyFixAsync(@"
using System.IO;
#nullable enable

abstract class C : TextWriter
{
    public override void Write(string format, object arg0, object arg1, object arg2)
    {
    }
}
", @"
using System.IO;
#nullable enable

abstract class C : TextWriter
{
    public override void Write(string format, object? arg0, object? arg1, object? arg2)
    {
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
    }
}
