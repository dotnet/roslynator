// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1141AddParamElementToDocumentationCommentTests : AbstractCSharpDiagnosticVerifier<SingleLineDocumentationCommentTriviaAnalyzer, SingleLineDocumentationCommentTriviaCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.AddParamElementToDocumentationComment;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddParamElementToDocumentationComment)]
    public async Task Test()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    ///[| <summary>
    /// 
    /// </summary>
    /// <param name="p"></param>
    /// <param name="p3"></param>
    /// <param name="p5"></param>
|]    void M(object p, object p2, object p3, object p4, object p5)
    {
    }
}
""", """
class C
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="p"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="p4"></param>
    /// <param name="p5"></param>
    void M(object p, object p2, object p3, object p4, object p5)
    {
    }
}
""");
    }

#if ROSLYN_5_0
    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddParamElementToDocumentationComment)]
    public async Task TestNoDiagnostic_StaticExtensionBlock()
    {
        await VerifyNoDiagnosticAsync("""
public static class C
{
    /// <summary>
    /// Extensions for <see cref="System.IO.Stream" />
    /// </summary>
    extension(System.IO.Stream)
    {
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddParamElementToDocumentationComment)]
    public async Task Test_NamedExtensionBlock()
    {
        await VerifyDiagnosticAndFixAsync("""
public static class C
{
    ///[| <summary>
    /// Extensions for <see cref="System.IO.Stream" />
    /// </summary>
|]    extension(System.IO.Stream stream)
    {
    }
}
""", """
public static class C
{
    /// <summary>
    /// Extensions for <see cref="System.IO.Stream" />
    /// </summary>
    /// <param name="stream"></param>
    extension(System.IO.Stream stream)
    {
    }
}
""");
    }
#endif
}
