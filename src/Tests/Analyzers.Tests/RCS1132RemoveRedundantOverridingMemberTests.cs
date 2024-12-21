// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1132RemoveRedundantOverridingMemberTests : AbstractCSharpDiagnosticVerifier<RemoveRedundantOverridingMemberAnalyzer, MemberDeclarationCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.RemoveRedundantOverridingMember;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantOverridingMember)]
    public async Task TestNoDiagnostic_Record()
    {
        await VerifyNoDiagnosticAsync("""
using System;
using System.Text;

public record IntPoint(int X, int Y)
{
    public override string ToString() => $"[{X}, {Y}]";
}

public record IntPointWithValue<T>(T value, int X, int Y) : IntPoint(X, Y)
{
    public override string ToString() => base.ToString();

    public override int GetHashCode() => base.GetHashCode();

    protected override bool PrintMembers(StringBuilder builder) => base.PrintMembers(builder);
}
""");
    }
}
