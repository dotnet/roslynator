// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Xunit;

#pragma warning disable RCS1090

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS0508MethodReturnTypeMustMatchOverriddenMethodReturnTypeTests : AbstractCSharpCompilerCodeFixVerifier
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.MethodReturnTypeMustMatchOverriddenMethodReturnType;

        public override CodeFixProvider FixProvider { get; } = new MemberDeclarationCodeFixProvider();

        [Fact]
        public async Task TestFix()
        {
            await VerifyFixAsync(@"
class C
{
    public override object ToString()
    {
    }
}
", @"
class C
{
    public override string ToString()
    {
    }
}
", EquivalenceKey.Create(DiagnosticId));
        }
    }
}
