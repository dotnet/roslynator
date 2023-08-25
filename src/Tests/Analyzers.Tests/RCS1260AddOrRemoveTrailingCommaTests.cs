// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1260AddOrRemoveTrailingCommaTests : AbstractCSharpDiagnosticVerifier<AddOrRemoveTrailingCommaAnalyzer, AddOrRemoveTrailingCommaCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.AddOrRemoveTrailingComma;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveTrailingComma)]
    public async Task Test_EnumDeclaration_Include()
    {
        await VerifyDiagnosticAndFixAsync(@"
enum Foo
{
    A,
    B[||]
}
", @"
enum Foo
{
    A,
    B,
}
", options: Options.AddConfigOption(ConfigOptionKeys.TrailingCommaStyle, ConfigOptionValues.TrailingCommaStyle_Include));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveTrailingComma)]
    public async Task Test_EnumDeclaration_Omit()
    {
        await VerifyDiagnosticAndFixAsync(@"
enum Foo
{
    A,
    B[|,|]
}
", @"
enum Foo
{
    A,
    B
}
", options: Options.AddConfigOption(ConfigOptionKeys.TrailingCommaStyle, ConfigOptionValues.TrailingCommaStyle_Omit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveTrailingComma)]
    public async Task Test_SingleLineEnumDeclaration_Include()
    {
        await VerifyDiagnosticAndFixAsync(@"
enum Foo { A, B[||] }
", @"
enum Foo { A, B, }
", options: Options.AddConfigOption(ConfigOptionKeys.TrailingCommaStyle, ConfigOptionValues.TrailingCommaStyle_Include));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveTrailingComma)]
    public async Task Test_SingleLineEnumDeclaration_Omit()
    {
        await VerifyDiagnosticAndFixAsync(@"
enum Foo { A, B[|,|] }
", @"
enum Foo { A, B }
", options: Options.AddConfigOption(ConfigOptionKeys.TrailingCommaStyle, ConfigOptionValues.TrailingCommaStyle_Omit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveTrailingComma)]
    public async Task Test_SingleLineEnumDeclaration_OmitWhenSingleLine()
    {
        await VerifyDiagnosticAndFixAsync(@"
enum Foo { A, B[|,|] }
", @"
enum Foo { A, B }
", options: Options.AddConfigOption(ConfigOptionKeys.TrailingCommaStyle, ConfigOptionValues.TrailingCommaStyle_OmitWhenSingleLine));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveTrailingComma)]
    public async Task Test_ArrayInitializer_Include()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var arr = new[]
        {
            """",
            """"[||]
        };
    }
}
", @"
class C
{
    void M()
    {
        var arr = new[]
        {
            """",
            """",
        };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TrailingCommaStyle, ConfigOptionValues.TrailingCommaStyle_Include));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveTrailingComma)]
    public async Task Test_ArrayInitializer_Omit()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var arr = new[]
        {
            """",
            """"[|,|]
        };
    }
}
", @"
class C
{
    void M()
    {
        var arr = new[]
        {
            """",
            """"
        };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TrailingCommaStyle, ConfigOptionValues.TrailingCommaStyle_Omit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveTrailingComma)]
    public async Task Test_SingleLineArrayInitializer_Include()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var arr = new[] { """", """"[||] };
    }
}
", @"
class C
{
    void M()
    {
        var arr = new[] { """", """", };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TrailingCommaStyle, ConfigOptionValues.TrailingCommaStyle_Include));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveTrailingComma)]
    public async Task Test_SingleLineArrayInitializer_Omit()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var arr = new[] { """", """"[|,|] };
    }
}
", @"
class C
{
    void M()
    {
        var arr = new[] { """", """" };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TrailingCommaStyle, ConfigOptionValues.TrailingCommaStyle_Omit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveTrailingComma)]
    public async Task Test_SingleLineArrayInitializer_OmitWhenSingleLine()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var arr = new[] { """", """"[|,|] };
    }
}
", @"
class C
{
    void M()
    {
        var arr = new[] { """", """" };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TrailingCommaStyle, ConfigOptionValues.TrailingCommaStyle_OmitWhenSingleLine));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveTrailingComma)]
    public async Task Test_AnonymousObjectCreationExpression_Include()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var obj = new
        {
            A = """",
            B = """"[||]
        };
    }
}
", @"
class C
{
    void M()
    {
        var obj = new
        {
            A = """",
            B = """",
        };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TrailingCommaStyle, ConfigOptionValues.TrailingCommaStyle_Include));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveTrailingComma)]
    public async Task Test_AnonymousObjectCreationExpression_Omit()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var obj = new
        {
            A = """",
            B = """"[|,|]
        };
    }
}
", @"
class C
{
    void M()
    {
        var obj = new
        {
            A = """",
            B = """"
        };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TrailingCommaStyle, ConfigOptionValues.TrailingCommaStyle_Omit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveTrailingComma)]
    public async Task Test_SingleLineAnonymousObjectCreationExpression_Include()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var obj = new { A = """", B = """"[||] };
    }
}
", @"
class C
{
    void M()
    {
        var obj = new { A = """", B = """", };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TrailingCommaStyle, ConfigOptionValues.TrailingCommaStyle_Include));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveTrailingComma)]
    public async Task Test_SingleLineAnonymousObjectCreationExpression_Omit()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var obj = new { A = """", B = """"[|,|] };
    }
}
", @"
class C
{
    void M()
    {
        var obj = new { A = """", B = """" };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TrailingCommaStyle, ConfigOptionValues.TrailingCommaStyle_Omit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveTrailingComma)]
    public async Task Test_SingleLineAnonymousObjectCreationExpression_OmitWhenSingleLine()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var obj = new { A = """", B = """"[|,|] };
    }
}
", @"
class C
{
    void M()
    {
        var obj = new { A = """", B = """" };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TrailingCommaStyle, ConfigOptionValues.TrailingCommaStyle_OmitWhenSingleLine));
    }
}
