// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CodeFixes;
using Roslynator.Testing.CSharp.Xunit;

namespace Roslynator.Testing.CSharp;

public abstract class AbstractCSharpCompilerDiagnosticFixVerifier<TFixProvider> : XunitCompilerDiagnosticFixVerifier<TFixProvider>
    where TFixProvider : CodeFixProvider, new()
{
    public override CSharpTestOptions Options => DefaultCSharpTestOptions.Value;
}
