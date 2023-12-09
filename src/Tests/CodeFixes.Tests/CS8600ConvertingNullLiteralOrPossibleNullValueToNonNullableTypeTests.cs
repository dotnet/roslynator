// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests;

public class CS8600ConvertingNullLiteralOrPossibleNullValueToNonNullableTypeTests : AbstractCSharpCompilerDiagnosticFixVerifier<DeclareAsNullableCodeFixProvider>
{
    public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.CS8600_ConvertingNullLiteralOrPossibleNullValueToNonNullableType;

    [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS8600_ConvertingNullLiteralOrPossibleNullValueToNonNullableType)]
    public async Task Test_LocalDeclaration()
    {
        await VerifyFixAsync(@"
using System;
using System.Reflection;
#nullable enable

public class C
{
    void M()
    {
        TypeInfo x = default!;
        FlagsAttribute a = x.GetCustomAttribute<FlagsAttribute>();
    }
}
", @"
using System;
using System.Reflection;
#nullable enable

public class C
{
    void M()
    {
        TypeInfo x = default!;
        FlagsAttribute? a = x.GetCustomAttribute<FlagsAttribute>();
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
    }

    [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS8600_ConvertingNullLiteralOrPossibleNullValueToNonNullableType)]
    public async Task Test_DeclarationExpression()
    {
        await VerifyFixAsync(@"
using System.Collections.Generic;
#nullable enable

public class C
{
    void M()
    {
        var dic = new Dictionary<string, string>();
        if (dic.TryGetValue("""", out string value)) { }
    }
}
", @"
using System.Collections.Generic;
#nullable enable

public class C
{
    void M()
    {
        var dic = new Dictionary<string, string>();
        if (dic.TryGetValue("""", out string? value)) { }
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
    }
}
