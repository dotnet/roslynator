// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1060DeclareEachTypeInSeparateFileTests : AbstractCSharpDiagnosticVerifier<DeclareEachTypeInSeparateFileAnalyzer, ExtractMemberToNewDocumentCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor => DiagnosticRules.DeclareEachTypeInSeparateFile;

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.DeclareEachTypeInSeparateFile)]
    public async Task Test_Namespace()
    {
        await VerifyDiagnosticAndFixAsync("""
namespace N
{
    public class [|C1|]
    {
    }

    public class [|C2|]
    {
    }
}
""", """
namespace N
{
    public class C2
    {
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.DeclareEachTypeInSeparateFile)]
    public async Task Test_FileScopedNamespace()
    {
        await VerifyDiagnosticAndFixAsync("""
namespace N;

public class [|C1|]
{
}

public class [|C2|]
{
}
""", """
namespace N;

public class C2
{
}
""");
    }
}
