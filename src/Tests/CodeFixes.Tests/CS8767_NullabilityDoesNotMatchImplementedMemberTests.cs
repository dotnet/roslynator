// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests;

public class CS8767_NullabilityDoesNotMatchImplementedMemberTests : AbstractCSharpCompilerDiagnosticFixVerifier<TokenCodeFixProvider>
{
    public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.CS8767_NullabilityDoesNotMatchImplementedMember;

    [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS8767_NullabilityDoesNotMatchImplementedMember)]
    public async Task Test()
    {
        await VerifyFixAsync(@"
using System.Collections.Generic;
#nullable enable

public class Comparer : IComparer<object>
{
    private Comparer()
    {
    }

    public static readonly Comparer Instance = new();

    public int Compare(object x, object y)
    {
        return 0;
    }
}
", @"
using System.Collections.Generic;
#nullable enable

public class Comparer : IComparer<object>
{
    private Comparer()
    {
    }

    public static readonly Comparer Instance = new();

    public int Compare(object? x, object? y)
    {
        return 0;
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
    }
}
