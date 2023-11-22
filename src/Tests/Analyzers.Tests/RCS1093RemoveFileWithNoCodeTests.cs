// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Testing;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1093RemoveFileWithNoCodeTests : AbstractCSharpDiagnosticVerifier<RemoveFileWithNoCodeAnalyzer, EmptyCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.RemoveFileWithNoCode;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveFileWithNoCode)]
    public async Task Test()
    {
        await VerifyDiagnosticAsync(@"[||]// copyright ...

");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveFileWithNoCode)]
    public async Task Test_UsingDirective()
    {
        await VerifyDiagnosticAsync(@"[||]// copyright ...

using System;

");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveFileWithNoCode)]
    public async Task Test_FileScopedNamespaceDeclaration()
    {
        await VerifyDiagnosticAsync(@"[||]// copyright ...

namespace N;

");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveFileWithNoCode)]
    public async Task Test_IfDirective()
    {
        await VerifyNoDiagnosticAsync(@"// copyright ...

#if FOO
class C
{
}
#endif

");
    }
}
