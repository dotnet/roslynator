// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Formatting.CodeFixes.CSharp;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests;

public class RCS0060BlankLineAfterFileScopedNamespaceDeclarationTests : AbstractCSharpDiagnosticVerifier<BlankLineAfterFileScopedNamespaceDeclarationAnalyzer, FileScopedNamespaceDeclarationCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.BlankLineAfterFileScopedNamespaceDeclaration;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.BlankLineAfterFileScopedNamespaceDeclaration)]
    public async Task Test_AddEmptyLine()
    {
        await VerifyDiagnosticAndFixAsync(@"
namespace A.B;
[||]class C
{
}
", @"
namespace A.B;

class C
{
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineAfterFileScopedNamespaceDeclaration, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.BlankLineAfterFileScopedNamespaceDeclaration)]
    public async Task Test_AddEmptyLine2()
    {
        await VerifyDiagnosticAndFixAsync(@"
namespace A.B;  
[||]class C
{
}
", @"
namespace A.B;  

class C
{
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineAfterFileScopedNamespaceDeclaration, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.BlankLineAfterFileScopedNamespaceDeclaration)]
    public async Task Test_AddEmptyLine3()
    {
        await VerifyDiagnosticAndFixAsync(@"
namespace A.B;  //x
[||]class C
{
}
", @"
namespace A.B;  //x

class C
{
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineAfterFileScopedNamespaceDeclaration, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.BlankLineAfterFileScopedNamespaceDeclaration)]
    public async Task Test_AddEmptyLine4()
    {
        await VerifyDiagnosticAndFixAsync(@"
namespace A.B;//x
[||]class C
{
}
", @"
namespace A.B;//x

class C
{
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineAfterFileScopedNamespaceDeclaration, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.BlankLineAfterFileScopedNamespaceDeclaration)]
    public async Task Test_AddEmptyLine5()
    {
        await VerifyDiagnosticAndFixAsync(@"
namespace A.B;[||]class C
{
}
", @"
namespace A.B;

class C
{
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineAfterFileScopedNamespaceDeclaration, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.BlankLineAfterFileScopedNamespaceDeclaration)]
    public async Task Test_AddEmptyLine6()
    {
        await VerifyDiagnosticAndFixAsync(@"
namespace A.B;  [||]class C
{
}
", @"
namespace A.B;  

class C
{
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineAfterFileScopedNamespaceDeclaration, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.BlankLineAfterFileScopedNamespaceDeclaration)]
    public async Task Test_AddEmptyLine7()
    {
        await VerifyDiagnosticAndFixAsync(@"
namespace A.B;
[||]  class C
{
}
", @"
namespace A.B;

  class C
{
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineAfterFileScopedNamespaceDeclaration, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.BlankLineAfterFileScopedNamespaceDeclaration)]
    public async Task Test_AddEmptyLine8()
    {
        await VerifyDiagnosticAndFixAsync(@"
namespace A.B;
[||]/// <summary>
/// 
/// </summary>
class C
{
}
", @"
namespace A.B;

/// <summary>
/// 
/// </summary>
class C
{
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineAfterFileScopedNamespaceDeclaration, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.BlankLineAfterFileScopedNamespaceDeclaration)]
    public async Task Test_AddEmptyLine9()
    {
        await VerifyDiagnosticAndFixAsync(@"
namespace A.B;
[||]// x
class C
{
}
", @"
namespace A.B;

// x
class C
{
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineAfterFileScopedNamespaceDeclaration, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.BlankLineAfterFileScopedNamespaceDeclaration)]
    public async Task Test_RemoveEmptyLine_UsingAfter()
    {
        await VerifyDiagnosticAndFixAsync(@"
namespace N;
[||]using System;

public class C
{
}
", @"
namespace N;

using System;

public class C
{
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineAfterFileScopedNamespaceDeclaration, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.BlankLineAfterFileScopedNamespaceDeclaration)]
    public async Task Test_RemoveEmptyLine()
    {
        await VerifyDiagnosticAndFixAsync(@"
namespace A.B;
[||]
class C
{
}
", @"
namespace A.B;
class C
{
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineAfterFileScopedNamespaceDeclaration, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.BlankLineAfterFileScopedNamespaceDeclaration)]
    public async Task Test_RemoveEmptyLine2()
    {
        await VerifyDiagnosticAndFixAsync(@"
namespace A.B;  
[||]
class C
{
}
", @"
namespace A.B;  
class C
{
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineAfterFileScopedNamespaceDeclaration, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.BlankLineAfterFileScopedNamespaceDeclaration)]
    public async Task Test_RemoveEmptyLine3()
    {
        await VerifyDiagnosticAndFixAsync(@"
namespace A.B;  //x
[||]
class C
{
}
", @"
namespace A.B;  //x
class C
{
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineAfterFileScopedNamespaceDeclaration, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.BlankLineAfterFileScopedNamespaceDeclaration)]
    public async Task Test_RemoveEmptyLine4()
    {
        await VerifyDiagnosticAndFixAsync(@"
namespace A.B;//x
[||]
class C
{
}
", @"
namespace A.B;//x
class C
{
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineAfterFileScopedNamespaceDeclaration, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.BlankLineAfterFileScopedNamespaceDeclaration)]
    public async Task Test_RemoveEmptyLine5()
    {
        await VerifyDiagnosticAndFixAsync(@"
namespace A.B;
[||]

class C
{
}
", @"
namespace A.B;
class C
{
}
", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineAfterFileScopedNamespaceDeclaration, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.BlankLineAfterFileScopedNamespaceDeclaration)]
    public async Task TestNoDiagnostic_EmptyFileWithComment()
    {
        await VerifyNoDiagnosticAsync(@"
namespace A.B;

// x", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineAfterFileScopedNamespaceDeclaration, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.BlankLineAfterFileScopedNamespaceDeclaration)]
    public async Task TestNoDiagnostic_EmptyFileWithComment2()
    {
        await VerifyNoDiagnosticAsync(@"
namespace A.B;
// x", options: Options.AddConfigOption(ConfigOptionKeys.BlankLineAfterFileScopedNamespaceDeclaration, true));
    }
}
