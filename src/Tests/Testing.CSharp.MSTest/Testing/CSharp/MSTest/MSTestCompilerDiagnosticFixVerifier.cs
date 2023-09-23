// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CodeFixes;

namespace Roslynator.Testing.CSharp.MSTest;

/// <summary>
/// Represents a verifier for C# compiler diagnostics.
/// </summary>
public abstract class MSTestCompilerDiagnosticFixVerifier<TFixProvider> : CSharpCompilerDiagnosticFixVerifier<TFixProvider>
    where TFixProvider : CodeFixProvider, new()
{
    /// <summary>
    /// Initializes a new instance of <see cref="MSTestCompilerDiagnosticFixVerifier{TFixProvider}"/>
    /// </summary>
    protected MSTestCompilerDiagnosticFixVerifier() : base(MSTestAssert.Instance)
    {
    }
}
