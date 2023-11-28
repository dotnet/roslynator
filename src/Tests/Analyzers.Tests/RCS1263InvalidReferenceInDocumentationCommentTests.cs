// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1263InvalidReferenceInDocumentationCommentTests : AbstractCSharpDiagnosticVerifier<SingleLineDocumentationCommentTriviaAnalyzer, XmlNodeCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.InvalidReferenceInDocumentationComment;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.InvalidReferenceInDocumentationComment)]
    public async Task Test_ParamElement()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    /// <summary></summary>
    /// <param name=""[|p|]""></param>
    void M(object p1, object p2) => M(p1, p2);
}
", @"
class C
{
    /// <summary></summary>
    void M(object p1, object p2) => M(p1, p2);
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.InvalidReferenceInDocumentationComment)]
    public async Task Test_ParamElement2()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    /// <summary></summary>
    /// <param name=""[|p|]""></param>
    void M() => M();
}
", @"
class C
{
    /// <summary></summary>
    void M() => M();
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.InvalidReferenceInDocumentationComment)]
    public async Task Test_ParamElement_Empty()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    /// <summary></summary>
    /// <param name=""[|p|]"" />
    void M(object p1, object p2) => M(p1, p2);
}
", @"
class C
{
    /// <summary></summary>
    void M(object p1, object p2) => M(p1, p2);
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.InvalidReferenceInDocumentationComment)]
    public async Task Test_TypeParamElement()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    /// <summary></summary>
    /// <typeparam name=""[|T|]""></param>
    void M<T1, T2>() => M<T1, T2>();
}
", @"
class C
{
    /// <summary></summary>
    void M<T1, T2>() => M<T1, T2>();
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.InvalidReferenceInDocumentationComment)]
    public async Task Test_TypeParamElement2()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    /// <summary></summary>
    /// <typeparam name=""[|T|]""></param>
    void M() => M();
}
", @"
class C
{
    /// <summary></summary>
    void M() => M();
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.InvalidReferenceInDocumentationComment)]
    public async Task Test_TypeParamElement_Empty()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    /// <summary></summary>
    /// <typeparam name=""[|T|]"" />
    void M<T1, T2>() => M<T1, T2>();
}
", @"
class C
{
    /// <summary></summary>
    void M<T1, T2>() => M<T1, T2>();
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.InvalidReferenceInDocumentationComment)]
    public async Task TestNoDiagnostic_ParamElement()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    /// <summary></summary>
    /// <param name=""p1""></param>
    /// <param name=""p2""></param>
    void M(object p1, object p2) => M(p1, p2);
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.InvalidReferenceInDocumentationComment)]
    public async Task TestNoDiagnostic_TypeParamElement()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    /// <summary></summary>
    /// <typeparam name=""T1"" />
    /// <typeparam name=""T2"" />
    void M<T1, T2>() => M<T1, T2>();
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.InvalidReferenceInDocumentationComment)]
    public async Task TestNoDiagnostic_Record()
    {
        await VerifyNoDiagnosticAsync(@"
/// <summary>
/// x
/// </summary>
/// <param name=""Bar"">bar</param>
public record Foo(string Bar);
", options: Options.AddAllowedCompilerDiagnosticId("CS0518"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.InvalidReferenceInDocumentationComment)]
    public async Task TestNoDiagnostic_RecordStruct()
    {
        await VerifyNoDiagnosticAsync(@"
/// <summary>
/// x
/// </summary>
/// <param name=""Bar"">bar</param>
public record struct Foo(string Bar);
", options: Options.AddAllowedCompilerDiagnosticId("CS0518"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.InvalidReferenceInDocumentationComment)]
    public async Task TestNoDiagnostic_ClassPrimaryConstructor()
    {
        await VerifyNoDiagnosticAsync(@"
/// <summary>
/// x
/// </summary>
/// <param name=""value"">x</param>
public class Foo(string value)
{
    public string Value { get; } = value;
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.InvalidReferenceInDocumentationComment)]
    public async Task TestNoDiagnostic_StructPrimaryConstructor()
    {
        await VerifyNoDiagnosticAsync(@"
/// <summary>
/// x
/// </summary>
/// <param name=""value"">x</param>
public struct Foo(string value)
{
    public string Value { get; } = value;
}
");
    }
}
