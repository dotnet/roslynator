// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

#pragma warning disable RCS1090

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1224MakeMethodExtensionMethodTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.MakeMethodExtensionMethod;

        public override DiagnosticAnalyzer Analyzer { get; } = new MakeMethodExtensionMethodAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new MemberDeclarationCodeFixProvider();

        [Fact]
        public async Task Test_ImplictlyInternal()
        {
            await VerifyDiagnosticAndFixAsync(@"
static class FooExtensions
{
    public static string [|M|](string s) => s;
}
", @"
static class FooExtensions
{
    public static string M(this string s) => s;
}
");
        }

        [Fact]
        public async Task Test_Internal()
        {
            await VerifyDiagnosticAndFixAsync(@"
internal static class FooExtensions
{
    internal static string [|M|](string s) => s;
}
", @"
internal static class FooExtensions
{
    internal static string M(this string s) => s;
}
");
        }

        [Fact]
        public async Task Test_Public()
        {
            await VerifyDiagnosticAndFixAsync(@"
public static class FooExtensions
{
    public static string [|M|](string s) => s;
}
", @"
public static class FooExtensions
{
    public static string M(this string s) => s;
}
");
        }

        [Fact]
        public async Task TestNoDiagnostic_NoSuffix()
        {
            await VerifyNoDiagnosticAsync(@"
public static class Foo
{
    public static string M(string s) => s;
}
");
        }

        [Fact]
        public async Task TestNoDiagnostic_NotStatic()
        {
            await VerifyNoDiagnosticAsync(@"
public class FooExtensions
{
    public string M(string s) => s;
}
");
        }

        [Fact]
        public async Task TestNoDiagnostic_NestedClass()
        {
            await VerifyNoDiagnosticAsync(@"
public static class Foo
{
    public static class FooExtensions
    {
        public static string M(string s) => s;
    }
}
");
        }

        [Fact]
        public async Task TestNoDiagnostic_PrivateMethod()
        {
            await VerifyNoDiagnosticAsync(@"
public static class FooExtensions
{
    private static string M(string s) => s;
}
");
        }

        [Fact]
        public async Task TestNoDiagnostic_ExtensionMethod()
        {
            await VerifyNoDiagnosticAsync(@"
public static class FooExtensions
{
    public static string M(this string s) => s;
}
");
        }
    }
}
