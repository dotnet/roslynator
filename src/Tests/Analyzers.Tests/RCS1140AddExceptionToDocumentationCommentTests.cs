// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1140AddExceptionToDocumentationCommentTests : AbstractCSharpDiagnosticVerifier<AddExceptionToDocumentationCommentAnalyzer, AddExceptionToDocumentationCommentCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.AddExceptionToDocumentationComment;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddExceptionToDocumentationComment)]
    public async Task Test_Example_From_Documentation()
    {
        await VerifyDiagnosticAndFixAsync("""
using System;

class C
{
    /// <summary>
    /// ...
    /// </summary>
    /// <param name="parameter"></param>
    public void Foo(object parameter)
    {
        if (parameter == null)
            [|throw new ArgumentNullException(nameof(parameter));|]
    }
}

""", """
using System;

class C
{
    /// <summary>
    /// ...
    /// </summary>
    /// <param name="parameter"></param>
    /// <exception cref="ArgumentNullException"><paramref name="parameter"/> is <c>null</c>.</exception>
    public void Foo(object parameter)
    {
        if (parameter == null)
            throw new ArgumentNullException(nameof(parameter));
    }
}

""");
    }
}
