// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Formatting.CodeFixes.CSharp;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests;

public class RCS0005AddBlankLineBeforeEndRegionDirectiveTests : AbstractCSharpDiagnosticVerifier<AddBlankLineBeforeEndRegionDirectiveAnalyzer, DirectiveTriviaCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.AddBlankLineBeforeEndRegionDirective;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBeforeEndRegionDirective)]
    public async Task Test()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    #region Methods

    void M()
    {
    }
    [||]#endregion
}
", @"
class C
{
    #region Methods

    void M()
    {
    }

    #endregion
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBeforeEndRegionDirective)]
    public async Task Test_NoIndentation()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
#region Methods

void M()
{
}
[||]#endregion
}
", @"
class C
{
#region Methods

void M()
{
}

#endregion
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBeforeEndRegionDirective)]
    public async Task Test_Comment()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    #region Methods

    // x
    void M()
    {
    }
    // x
    [||]#endregion
}
", @"
class C
{
    #region Methods

    // x
    void M()
    {
    }
    // x

    #endregion
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBeforeEndRegionDirective)]
    public async Task Test_DocumentationComment()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    #region Methods

    /// <summary>
    /// 
    /// </summary>
    void M()
    {
    }
    [||]#endregion
}
", @"
class C
{
    #region Methods

    /// <summary>
    /// 
    /// </summary>
    void M()
    {
    }

    #endregion
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBeforeEndRegionDirective)]
    public async Task TestNoDiagnostic()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    #region Methods
    void M()
    {
    }

    #endregion
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBeforeEndRegionDirective)]
    public async Task TestNoDiagnostic_EmptyRegion()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    #region Methods
    #endregion
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBeforeEndRegionDirective)]
    public async Task TestNoDiagnostic_Comment()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    #region Methods
    // x
    void M()
    {
    }
    // x

    #endregion
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBeforeEndRegionDirective)]
    public async Task TestNoDiagnostic_DocumentationComment()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    #region Methods
    /// <summary>
    /// 
    /// </summary>
    void M()
    {
    }

    #endregion
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBeforeEndRegionDirective)]
    public async Task TestNoDiagnostic_TwoEndRegions()
    {
        await VerifyNoDiagnosticAsync(@"
#region
class C
{
    #region
    void M()
    {
    }

    #endregion

    #endregion
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBeforeEndRegionDirective)]
    public async Task TestNoDiagnostic_EmptyRegions()
    {
        await VerifyNoDiagnosticAsync(@"
    #region

    #region

    #endregion

    #endregion
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBeforeEndRegionDirective)]
    public async Task TestNoDiagnostic_PragmaWarningDirective()
    {
        await VerifyNoDiagnosticAsync(@"
namespace N
{
    /// <summary>
    /// x
    /// </summary>
    class C
    {
        #region R

#pragma warning disable 1591

        public int P { get; set; }

#pragma warning restore 1591

        #endregion
    }
}");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBeforeEndRegionDirective)]
    public async Task TestNoDiagnostic_IfDirectiveInsideRegion()
    {
        await VerifyNoDiagnosticAsync("""
namespace N
{
#region Some region

#if DEBUG
// Some code
#else
// Some code
#endif

#endregion Some region
}
""");
    }
}
