// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1150CallStringConcatInsteadOfStringJoinTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.CallStringConcatInsteadOfStringJoin;

        public override DiagnosticAnalyzer Analyzer { get; } = new InvocationExpressionAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new InvocationExpressionCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.CallStringConcatInsteadOfStringJoin)]
        public async Task Test_EmptyStringLiteral()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = string.[|Join|]("""", default(object), default(object));
    }
}
", @"
class C
{
    void M()
    {
        string s = string.Concat(default(object), default(object));
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.CallStringConcatInsteadOfStringJoin)]
        public async Task Test_EmptyStringLiteral2()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = string.[|Join|]("""", ""a"", ""b"");
    }
}
", @"
class C
{
    void M()
    {
        string s = string.Concat(""a"", ""b"");
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.CallStringConcatInsteadOfStringJoin)]
        public async Task Test_StringEmpty()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = string.[|Join|](string.Empty, new string[] { """" });
    }
}
", @"
class C
{
    void M()
    {
        string s = string.Concat(new string[] { """" });
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.CallStringConcatInsteadOfStringJoin)]
        public async Task Test_EmptyStringConstant()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    const string EmptyString = """";

    void M()
    {
        string s = string.[|Join|](EmptyString, new object[] { """" });
    }
}
", @"
class C
{
    const string EmptyString = """";

    void M()
    {
        string s = string.Concat(new object[] { """" });
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.CallStringConcatInsteadOfStringJoin)]
        public async Task TestNoDiagnostic_NonEmptySeparator()
        {
            await VerifyNoDiagnosticAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class C
{
    void M()
    {
        string s = string.Join(""x"", new object[] { """" });
    }
}
");
        }
    }
}
