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

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareEachTypeInSeparateFile)]
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

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareEachTypeInSeparateFile)]
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

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareEachTypeInSeparateFile)]
    public async Task Test_FirstClassWithFileKeyword_NoDiagnostic()
    {
        await VerifyNoDiagnosticAsync("""
namespace N
{
    file class C1;
    public class C2;
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareEachTypeInSeparateFile)]
    public async Task Test_LastClassWithFileKeyword_NoDiagnostic()
    {
        await VerifyNoDiagnosticAsync("""
namespace N
{
    public class C1;
    file class C2;
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareEachTypeInSeparateFile)]
    public async Task Test_PartialClassSameFile_NoDiagnostic()
    {
        await VerifyNoDiagnosticAsync("""
using System;

namespace N
{
    public partial class Foo
    {
    }

    public partial class Foo : IDisposable
    {
        public void Dispose()
        {
        }
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareEachTypeInSeparateFile)]
    public async Task Test_PartialClassWithDistinctType_Reports()
    {
        await VerifyDiagnosticAsync("""
namespace N
{
    public partial class [|Foo|]
    {
    }

    public class [|Bar|]
    {
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareEachTypeInSeparateFile)]
    public async Task Test_FileScopedNamespace_PartialClassSameFile_NoDiagnostic()
    {
        await VerifyNoDiagnosticAsync("""
using System;

namespace N;

public partial class Foo
{
}

public partial class Foo : IDisposable
{
    public void Dispose()
    {
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareEachTypeInSeparateFile)]
    public async Task Test_PartialGenericClassSameFile_NoDiagnostic()
    {
        await VerifyNoDiagnosticAsync("""
namespace N
{
    public partial class Foo<T>
    {
    }

    public partial class Foo<T>
    {
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareEachTypeInSeparateFile)]
    public async Task Test_PartialClassesWithDifferentArity_Reports()
    {
        await VerifyDiagnosticAsync("""
namespace N
{
    public partial class [|Foo|]
    {
    }

    public partial class [|Foo|]<T>
    {
    }

    public partial class [|Foo|]<T, U>
    {
    }
}
""");
    }
}
