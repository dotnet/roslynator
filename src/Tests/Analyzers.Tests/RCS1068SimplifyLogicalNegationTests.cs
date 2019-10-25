// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1068SimplifyLogicalNegationTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.SimplifyLogicalNegation;

        public override DiagnosticAnalyzer Analyzer { get; } = new SimplifyLogicalNegationAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new SimplifyLogicalNegationCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyLogicalNegation)]
        public async Task Test_NotTrue()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = [|!true|];
    }
}
", @"
class C
{
    void M()
    {
        bool x = false;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyLogicalNegation)]
        public async Task Test_NotTrue2()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = [|!(true)|];
    }
}
", @"
class C
{
    void M()
    {
        bool x = false;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyLogicalNegation)]
        public async Task Test_NotFalse()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = [|!false|];
    }
}
", @"
class C
{
    void M()
    {
        bool x = true;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyLogicalNegation)]
        public async Task Test_NotNot()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
            bool f1 = false;
            bool f2 = false;

            f1 = [|!!(f2)|];
    }
}
", @"
class C
{
    void M()
    {
            bool f1 = false;
            bool f2 = false;

            f1 = f2;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyLogicalNegation)]
        public async Task Test_NotNot2()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
            bool f1 = false;
            bool f2 = false;

            f1 = [|!(!(f2))|];
    }
}
", @"
class C
{
    void M()
    {
            bool f1 = false;
            bool f2 = false;

            f1 = f2;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyLogicalNegation)]
        public async Task Test_EqualsExpression()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
            bool f1 = false;
            bool f2 = false;

            f1 = [|!(f1 == f2)|];
    }
}
", @"
class C
{
    void M()
    {
            bool f1 = false;
            bool f2 = false;

            f1 = f1 != f2;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyLogicalNegation)]
        public async Task Test_NotEqualsExpression()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
            bool f1 = false;
            bool f2 = false;

            f1 = [|!(f1 != f2)|];
    }
}
", @"
class C
{
    void M()
    {
            bool f1 = false;
            bool f2 = false;

            f1 = f1 == f2;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyLogicalNegation)]
        public async Task Test_LessThanExpression()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
            int i1 = 1;
            int i2 = 2;

            bool x = [|!(i1 < i2)|];
    }
}
", @"
class C
{
    void M()
    {
            int i1 = 1;
            int i2 = 2;

            bool x = i1 >= i2;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyLogicalNegation)]
        public async Task Test_LessThanOrEqualsExpression()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
            int i1 = 1;
            int i2 = 2;

            bool x = [|!(i1 <= i2)|];
    }
}
", @"
class C
{
    void M()
    {
            int i1 = 1;
            int i2 = 2;

            bool x = i1 > i2;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyLogicalNegation)]
        public async Task Test_GreaterThanExpression()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
            int i1 = 1;
            int i2 = 2;

            bool x = [|!(i1 > i2)|];
    }
}
", @"
class C
{
    void M()
    {
            int i1 = 1;
            int i2 = 2;

            bool x = i1 <= i2;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyLogicalNegation)]
        public async Task Test_GreaterThanOrEqualsExpression()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
            int i1 = 1;
            int i2 = 2;

            bool x = [|!(i1 >= i2)|];
    }
}
", @"
class C
{
    void M()
    {
            int i1 = 1;
            int i2 = 2;

            bool x = i1 < i2;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyLogicalNegation)]
        public async Task TestNoDiagnostic_NotEqualsOperator()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    public static bool operator ==(C left, C right)
    {
        return false;
    }

    public static bool operator !=(C left, C right)
    {
        return !(left == right);
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyLogicalNegation)]
        public async Task TestNoDiagnostic_EqualsOperator()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    public static bool operator ==(C left, C right)
    {
        return !(left != right);
    }

    public static bool operator !=(C left, C right)
    {
        return false;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyLogicalNegation)]
        public async Task TestNoDiagnostic_NullableOfT()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
            bool x = false;
            int? i1 = 1;
            int i2 = 2;

            x = !(i1 < i2);
            x = !(i1 <= i2);
            x = !(i1 > i2);
            x = !(i1 >= i2);
    }

    void M2()
    {
            bool x = false;
            int i1 = 1;
            int? i2 = 2;

            x = !(i1 < i2);
            x = !(i1 <= i2);
            x = !(i1 > i2);
            x = !(i1 >= i2);
    }
}
");
        }
    }
}
