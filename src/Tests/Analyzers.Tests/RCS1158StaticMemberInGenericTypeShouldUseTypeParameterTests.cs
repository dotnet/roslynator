// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Testing;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1158StaticMemberInGenericTypeShouldUseTypeParameterTests : AbstractCSharpDiagnosticVerifier<StaticMemberInGenericTypeShouldUseTypeParameterAnalyzer, EmptyCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.StaticMemberInGenericTypeShouldUseTypeParameter;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.StaticMemberInGenericTypeShouldUseTypeParameter)]
    public async Task TestNoDiagnostic_Property()
    {
        await VerifyNoDiagnosticAsync(@"
public sealed class C<T> where T : IFoo
{
    public static string Name { get; } = T.Name;
}

public interface IFoo
{
    public static abstract string Name { get; }
}");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.StaticMemberInGenericTypeShouldUseTypeParameter)]
    public async Task TestNoDiagnostic_Field()
    {
        await VerifyNoDiagnosticAsync(@"
public sealed class C<T> where T : IFoo
{
    public static string Name = typeof(T).Name;
}

public interface IFoo
{
    public static abstract string Name { get; }
}");
    }
}
