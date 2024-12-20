// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1164UnusedTypeParameterTests : AbstractCSharpDiagnosticVerifier<UnusedParameter.UnusedParameterAnalyzer, UnusedParameterCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UnusedTypeParameter;

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnusedTypeParameter)]
    public async Task TestNoDiagnostic_DependencyPropertyEventArgs()
    {
        await VerifyNoDiagnosticAsync(@"
using System;

public static class TestClass
{
    public static void RunDelegate<T>(object arg)
    {
        RunDelegate(arg, (T arg) =>
        {
            if (arg is string stringArg)
            {
                Console.WriteLine(stringArg);
            }
        }
        );
    }

    private static void RunDelegate(
        object arg,
        Delegate _delegate
        )
    {
        _delegate.DynamicInvoke(arg);
    }
}
");
    }
}
