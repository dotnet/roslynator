// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Testing;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1093FileContainsNoCodeTests : AbstractCSharpDiagnosticVerifier<FileContainsNoCodeAnalyzer, EmptyCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.FileContainsNoCode;

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.FileContainsNoCode)]
    public async Task Test()
    {
        await VerifyDiagnosticAsync(@"[||]// copyright ...

");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.FileContainsNoCode)]
    public async Task TestNoDiagnostic_FileScopedNamespaceDeclaration()
    {
        await VerifyNoDiagnosticAsync(@"// copyright ...
namespace N;
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.FileContainsNoCode)]
    public async Task TestNoDiagnostic_IfDirective()
    {
        await VerifyNoDiagnosticAsync(@"// copyright ...

#if FOO
class C
{
}
#endif

");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.FileContainsNoCode)]
    public async Task TestNoDiagnostic_PragmaWarningDirective()
    {
        await VerifyNoDiagnosticAsync(@"#pragma warning disable RCS1093 // Remove file with no code.
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.IO;

//public class TestClass
//{
//}");
    }
}
