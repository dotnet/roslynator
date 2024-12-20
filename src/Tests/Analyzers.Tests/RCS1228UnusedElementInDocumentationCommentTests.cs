// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1228UnusedElementInDocumentationCommentTests : AbstractCSharpDiagnosticVerifier<SingleLineDocumentationCommentTriviaAnalyzer, RemoveElementInDocumentationCommentCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UnusedElementInDocumentationComment;

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnusedElementInDocumentationComment)]
    public async Task Test_FirstElement()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    /// [|<returns></returns>|]
    /// <summary>
    /// </summary>
    void M()
    {
    }
}
", @"
class C
{
    /// <summary>
    /// </summary>
    void M()
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnusedElementInDocumentationComment)]
    public async Task Test_FirstElement_Pragma()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
#pragma warning disable RCS0000
    /// [|<returns></returns>|]
    /// <summary>
    /// </summary>
    void M()
    {
    }
}
", @"
class C
{
#pragma warning disable RCS0000
    /// <summary>
    /// </summary>
    void M()
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnusedElementInDocumentationComment)]
    public async Task Test_LastElement()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    /// <summary>
    /// </summary>
    /// [|<returns></returns>|]
    void M()
    {
    }
}
", @"
class C
{
    /// <summary>
    /// </summary>
    void M()
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnusedElementInDocumentationComment)]
    public async Task Test_SelfClosingTag()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    /// <summary>
    /// </summary>
    /// [|<returns />|]
    void M()
    {
    }
}
", @"
class C
{
    /// <summary>
    /// </summary>
    void M()
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnusedElementInDocumentationComment)]
    public async Task Test_ReturnsIsOnlyElement()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    /// [|<returns></returns>|]
    void M()
    {
    }
}
", @"
class C
{
    void M()
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnusedElementInDocumentationComment)]
    public async Task Test_ReturnsIsOnlyElement_LocalFunction()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        /// [|<returns></returns>|]
        void LF()
        {
        }
    }
}
", @"
class C
{
    void M()
    {
        void LF()
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnusedElementInDocumentationComment)]
    public async Task Test_ExampleElement()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    /// <summary></summary>
    /// [|<example></example>|]
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

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnusedElementInDocumentationComment)]
    public async Task Test_RemarksElement()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    /// <summary></summary>
    /// [|<remarks></remarks>|]
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

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnusedElementInDocumentationComment)]
    public async Task Test_ValueElement()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    /// <summary></summary>
    /// [|<value></value>|]
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

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnusedElementInDocumentationComment)]
    public async Task Test_ParamElement()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    /// <summary></summary>
    /// [|<param name="p1"></param>|]
    /// [|<param name="p2"></param>|]
    void M(object p1, object p2) => M(p1, p2);
}
""", """
class C
{
    /// <summary></summary>
    void M(object p1, object p2) => M(p1, p2);
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnusedElementInDocumentationComment)]
    public async Task Test_ParamElement2()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    /// <summary></summary>
    /// [|<param name="p"></param>|]
    void M() => M();
}
""", """
class C
{
    /// <summary></summary>
    void M() => M();
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnusedElementInDocumentationComment)]
    public async Task Test_ParamElement_SelfClosingTag()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    /// <summary></summary>
    /// [|<param name="p" />|]
    void M(object p) => M(p);
}
""", """
class C
{
    /// <summary></summary>
    void M(object p) => M(p);
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnusedElementInDocumentationComment)]
    public async Task Test_TypeParamElement()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    /// <summary></summary>
    /// [|<typeparam name="T"></param>|]
    void M<T>() => M<T>();
}
""", """
class C
{
    /// <summary></summary>
    void M<T>() => M<T>();
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnusedElementInDocumentationComment)]
    public async Task Test_TypeParamElement2()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    /// <summary></summary>
    /// [|<typeparam name="T"></param>|]
    void M() => M();
}
""", """
class C
{
    /// <summary></summary>
    void M() => M();
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnusedElementInDocumentationComment)]
    public async Task Test_TypeParamElement_SelfClosingTag()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    /// <summary></summary>
    /// [|<typeparam name="T" />|]
    void M<T>() => M<T>();
}
""", """
class C
{
    /// <summary></summary>
    void M<T>() => M<T>();
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnusedElementInDocumentationComment)]
    public async Task Test_Class_PrimaryConstructor()
    {
        await VerifyDiagnosticAndFixAsync("""
/// <summary>
/// x
/// </summary>
/// [|[|<param name="value2"></param>|]|]
public class Foo(string value)
{
    public string Value { get; } = value;
}
""", """
/// <summary>
/// x
/// </summary>
public class Foo(string value)
{
    public string Value { get; } = value;
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnusedElementInDocumentationComment)]
    public async Task TestNoDiagnostic_Record()
    {
        await VerifyNoDiagnosticAsync("""
/// <summary>
/// x
/// </summary>
/// <param name="Bar">bar</param>
public record Foo(string Bar);
""", options: Options.AddAllowedCompilerDiagnosticId("CS0518"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnusedElementInDocumentationComment)]
    public async Task TestNoDiagnostic_RecordStruct()
    {
        await VerifyNoDiagnosticAsync("""
/// <summary>
/// x
/// </summary>
/// <param name="Bar">bar</param>
public record struct Foo(string Bar);
""", options: Options.AddAllowedCompilerDiagnosticId("CS0518"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnusedElementInDocumentationComment)]
    public async Task TestNoDiagnostic_ClassPrimaryConstructor()
    {
        await VerifyNoDiagnosticAsync("""
/// <summary>
/// x
/// </summary>
/// <param name="value">x</param>
public class Foo(string value)
{
    public string Value { get; } = value;
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnusedElementInDocumentationComment)]
    public async Task TestNoDiagnostic_StructPrimaryConstructor()
    {
        await VerifyNoDiagnosticAsync("""
/// <summary>
/// x
/// </summary>
/// <param name="value">x</param>
public struct Foo(string value)
{
    public string Value { get; } = value;
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnusedElementInDocumentationComment)]
    public async Task TestNoDiagnostic_NonEmpty()
    {
        await VerifyNoDiagnosticAsync("""
using System;

class C
{
    /// <summary></summary>
    /// <typeparam name="T">x</typeparam>
    /// <param name="p">x</param>
    /// <remarks>x</remarks>
    /// <value>x</value>
    /// <returns>x</returns>
    /// <example>x</example>
    /// <exception cref="Exception"></exception>
    /// <exception cref="Exception" />
    /// <permission cref="foo.com"></permission>
    /// <permission cref="foo.com" />
    /// <seealso cref="foo.com"/>
    /// <seealso cref="foo.com" />
    void M()
    {
    }
}
""");
    }
}
