// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1259RemoveEmptySyntaxTests : AbstractCSharpDiagnosticVerifier<RemoveEmptySyntaxAnalyzer, RemoveEmptySyntaxCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.RemoveEmptySyntax;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptySyntax)]
    public async Task Test_Destructor()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
    }

    [|~C()
    {
#if DEBUG
#endif
    }|]
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

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptySyntax)]
    public async Task TestNoDiagnostic_Destructor_IfDirective()
    {
        await VerifyNoDiagnosticAsync(@"
#define A
class C
{
    ~C()
    {
#if A
    }
#endif
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptySyntax)]
    public async Task TestNoDiagnostic_Destructor_NotEmpty()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
    }

    ~C()
    {
        M();
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptySyntax)]
    public async Task Test_ElseClause()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool f = false;

        if (f)
        {
        }
        [|else
        {
        }|]
    }
}
", @"
class C
{
    void M()
    {
        bool f = false;

        if (f)
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptySyntax)]
    public async Task TestNoDiagnostic_ElseClause_ElseIf()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool f = false;

        if (f)
        {
        }
        else if (f)
        {
        }
}
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptySyntax)]
    public async Task TestNoDiagnostic_ElseClause_NonEmptyElse()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool f = false;

        if (f)
        {
        }
        else
        {
            M();
        }
}
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptySyntax)]
    public async Task TestNoDiagnostic_ElseClause_IfElseEmbeddedInIfWithElse()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool f = false;

        if (f)
            if (f) M(); else { }
        else
        {
            M();
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptySyntax)]
    public async Task TestNoDiagnostic_ElseClause_IfElseEmbeddedInIfWithElse2()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool f = false;

        if (f)
            if (f) M(); else if (f) M(); else { }
        else
        {
            M();
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptySyntax)]
    public async Task Test_FinallyClause_TryCatchFinally()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        try
        {
        }
        catch
        {
        }
        [|finally
        {
        }|]
    }
}
", @"
class C
{
    void M()
    {
        try
        {
        }
        catch
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptySyntax)]
    public async Task Test_FinallyClause_TryFinally()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        try
        {
            //x
            M();
            M2();
        }
        [|finally
        {
        }|]
    }

    string M2() => null;
}
", @"
class C
{
    void M()
    {
        //x
        M();
        M2();
    }

    string M2() => null;
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptySyntax)]
    public async Task TestNoDiagnostic_FinallyClause_NonEmptyFinally()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        try
        {
        }
        catch
        {
        }
        finally
        {
            string foo = null;
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptySyntax)]
    public async Task Test_ObjectInitializer()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var x = new C [|{ }|];
    }
}
", @"
class C
{
    void M()
    {
        var x = new C();
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptySyntax)]
    public async Task Test_ObjectInitializer2()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Text;

class C
{
    void M()
    {
        var x = new StringBuilder();

        x = new StringBuilder() [|{ }|];

        x = new StringBuilder [|{ }|];

        x = new StringBuilder()
        [|{
        }|];

        x = new StringBuilder
        [|{
        }|];

        x = new StringBuilder() /**/ [|{ }|]; //x

        x = new StringBuilder /**/ [|{ }|]; //x

        x = new StringBuilder() //x
        [|{
        }|]; //x

        x = new StringBuilder //x
        [|{
        }|]; //x
    }
}
", @"
using System.Text;

class C
{
    void M()
    {
        var x = new StringBuilder();

        x = new StringBuilder();

        x = new StringBuilder();

        x = new StringBuilder();

        x = new StringBuilder();

        x = new StringBuilder() /**/  ; //x

        x = new StringBuilder() /**/  ; //x

        x = new StringBuilder() //x

        ; //x

        x = new StringBuilder() //x

        ; //x
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptySyntax)]
    public async Task TestNoDiagnostic_ObjectInitializer_ExpressionTree()
    {
        await VerifyNoDiagnosticAsync(@"
using System;
using System.Linq.Expressions;

class C
{
    public void M<T>(Expression<Func<T>> e)
    {
        M(() => new C { });
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptySyntax)]
    public async Task Test_Namespace()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Linq.Expressions;

namespace N1
{
    class C
    {
    }
}

[|namespace N2
{
}|]
", @"
using System;
using System.Linq.Expressions;

namespace N1
{
    class C
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptySyntax)]
    public async Task Test_RegionDirective()
    {
        await VerifyDiagnosticAndFixAsync(@"
namespace N1
{
    class C
    {
[|#region|]
#endregion
    }
}
", @"
namespace N1
{
    class C
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptySyntax)]
    public async Task Test_EmptyStatement()
    {
        await VerifyDiagnosticAndFixAsync(@"
namespace N1
{
    class C
    {
        void M()
        {
            M();
            [|;|]
        }
    }
}
", @"
namespace N1
{
    class C
    {
        void M()
        {
            M();
            
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptySyntax)]
    public async Task TestNoDiagnostic_EmptyStatement()
    {
        await VerifyNoDiagnosticAsync(@"
namespace N1
{
    class C
    {
        void M(bool p)
        {
            if (p)
                ;
        }
    }
}
", options: Options.AddAllowedCompilerDiagnosticId("CS0642"));
    }
}
