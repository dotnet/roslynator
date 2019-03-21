// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1163UnusedParameterTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UnusedParameter;

        public override DiagnosticAnalyzer Analyzer { get; } = new UnusedParameter.UnusedParameterAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new UnusedParameterCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnusedParameter)]
        public async Task Test_Method()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M()
    {
        object _ = null;

        Action<string> action = [|f|] => M();
    }
}
", @"
using System;

class C
{
    void M()
    {
        object _ = null;

        Action<string> action = __ => M();
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnusedParameter)]
        public async Task TestNoDiagnostic_StackAllocArrayCreationExpression()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    unsafe void M(int length)
    {
        var memory = stackalloc byte[length];
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnusedParameter)]
        public async Task TestNoDiagnostic_PartialMethod()
        {
            await VerifyNoDiagnosticAsync(@"
partial class C
{
    partial void M(object p);

    partial void M(object p)
    {
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnusedParameter)]
        public async Task TestNoDiagnostic_MethodReferencedAsMethodGroup()
        {
            await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    private void M(object p)
    {
        Action<object> action = M;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnusedParameter)]
        public async Task TestNoDiagnostic_ContainsOnlyThrowNew()
        {
            await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    public C(object p)
    {
        throw new NotImplementedException();
    }

    public C(object p, object p2) => throw new NotImplementedException();

    public string this[int index] => throw new NotImplementedException();

    public string this[string index]
    {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
    }

    public string this[string index, string index2]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public void Bar(object p) { throw new NotImplementedException(); }

    public object Bar(object p1, object p2) => throw new NotImplementedException();

    /// <summary>
    /// ...
    /// </summary>
    /// <param name=""p1""></param>
    /// <param name=""p2""></param>
    public void Bar2(object p1, object p2) { throw new NotImplementedException(); }

    public static C operator +(C left, C right)
    {
        throw new NotImplementedException();
    }

    public static explicit operator C(string value)
    {
        throw new NotImplementedException();
    }
}
");
        }
    }
}
