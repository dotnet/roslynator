// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;

#pragma warning disable RCS1223

namespace Roslynator.Testing;

/// <summary>
/// Represents code fix provider that does not provide any code fixes.
/// Use this code fix provider in <see cref="DiagnosticVerifier{TAnalyzer, TFixProvider}"/> when testing analyzers that do not provide any code fixes.
/// </summary>
public sealed class EmptyCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => throw new NotSupportedException();

    public override Task RegisterCodeFixesAsync(CodeFixContext context) => throw new NotSupportedException();

    public override FixAllProvider GetFixAllProvider() => throw new NotSupportedException();
}
