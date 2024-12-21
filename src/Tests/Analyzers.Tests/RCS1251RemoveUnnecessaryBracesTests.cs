// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1251RemoveUnnecessaryBracesTests : AbstractCSharpDiagnosticVerifier<RemoveUnnecessaryBracesAnalyzer, RemoveUnnecessaryBracesCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.RemoveUnnecessaryBraces;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnnecessaryBraces)]
    public async Task Test_Record()
    {
        await VerifyDiagnosticAndFixAsync(@"
namespace N
{
    record R(string Value)
    [|{|]
    }
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit; }
", @"
namespace N
{
    record R(string Value);
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit; }
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnnecessaryBraces)]
    public async Task Test_RecordStruct()
    {
        await VerifyDiagnosticAndFixAsync(@"
namespace N
{
    record struct R(string Value)
    [|{|]
    }
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit; }
", @"
namespace N
{
    record struct R(string Value);
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit; }
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnnecessaryBraces)]
    public async Task Test_Class()
    {
        await VerifyDiagnosticAndFixAsync(@"
namespace N
{
    class C
    [|{|]
    }
}
", @"
namespace N
{
    class C;
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnnecessaryBraces)]
    public async Task Test_Class_WithBaseClass()
    {
        await VerifyDiagnosticAndFixAsync(@"
namespace N
{
    class C : object
    [|{|]
    }
}
", @"
namespace N
{
    class C : object;
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnnecessaryBraces)]
    public async Task Test_Class_WhereConstraint()
    {
        await VerifyDiagnosticAndFixAsync(@"
namespace N
{
    class C<T> where T : new()
    [|{|]
    }
}
", @"
namespace N
{
    class C<T> where T : new();
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnnecessaryBraces)]
    public async Task Test_Struct()
    {
        await VerifyDiagnosticAndFixAsync(@"
namespace N
{
    struct C
    [|{|]
    }
}
", @"
namespace N
{
    struct C;
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnnecessaryBraces)]
    public async Task Test_Interface()
    {
        await VerifyDiagnosticAndFixAsync(@"
namespace N
{
    interface C
    [|{|]
    }
}
", @"
namespace N
{
    interface C;
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnnecessaryBraces)]
    public async Task Test_Class_CSharp11()
    {
        await VerifyNoDiagnosticAsync(@"
namespace N
{
    class C
    {
    }
}
", options: WellKnownCSharpTestOptions.Default_CSharp11);
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnnecessaryBraces)]
    public async Task Test_NoDiagnostic()
    {
        await VerifyNoDiagnosticAsync(@"
namespace N
{
    record struct R
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnnecessaryBraces)]
    public async Task TestNoDiagnostic_Class_WithParameterList()
    {
        await VerifyNoDiagnosticAsync(@"
namespace N
{
    class C(string P)
    {
    }
}
", options: Options.AddAllowedCompilerDiagnosticId("CS9113"));
    }
}
