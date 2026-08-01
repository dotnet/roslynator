// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1194ImplementExceptionConstructorsTests : AbstractCSharpDiagnosticVerifier<ImplementExceptionConstructorsAnalyzer, ClassDeclarationCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.ImplementExceptionConstructors;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ImplementExceptionConstructors)]
    public async Task Test()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;

class [|C|] : Exception
{
}
", @"
using System;

class C : Exception
{
    public C() : base()
    {
    }

    public C(string message) : base(message)
    {
    }

    public C(string message, Exception innerException) : base(message, innerException)
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ImplementExceptionConstructors)]
    public async Task Test_PrimaryConstructor()
    {
        await VerifyNoDiagnosticAsync(@"
using System;

class MyException (string message) : Exception(message);
");
    }
}
