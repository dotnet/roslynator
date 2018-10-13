// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS8050OnlyAutoImplementedPropertiesCanHaveInitializersTests : AbstractCSharpCompilerDiagnosticFixVerifier
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.OnlyAutoImplementedPropertiesCanHaveInitializers;

        public override CodeFixProvider FixProvider { get; } = new RemovePropertyOrFieldInitializerCodeFixProvider();

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.OnlyAutoImplementedPropertiesCanHaveInitializers)]
        public async Task Test()
        {
            await VerifyFixAsync(@"
class C
{
    private string _p;

    public string P
    {
        get { return _p; }
        set { _p = value; }
    } = "";
}
", @"
class C
{
    private string _p;

    public string P
    {
        get { return _p; }
        set { _p = value; }
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId, CodeFixIdentifiers.RemovePropertyOrFieldInitializer));
        }
    }
}
