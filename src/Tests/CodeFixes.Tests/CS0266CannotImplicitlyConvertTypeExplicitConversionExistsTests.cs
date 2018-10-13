// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS0266CannotImplicitlyConvertTypeExplicitConversionExistsTests : AbstractCSharpCompilerDiagnosticFixVerifier
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.CannotImplicitlyConvertTypeExplicitConversionExists;

        public override CodeFixProvider FixProvider { get; } = new ExpressionCodeFixProvider();

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CannotImplicitlyConvertTypeExplicitConversionExists)]
        public async Task Test_ChangeTypeAccordingToInitializer()
        {
            await VerifyFixAsync(@"
using System.Collections.Generic;

public class Foo
{
    public void Bar()
    {
        Foo x = GetValues();
    }

    public IEnumerable<Foo> GetValues()
    {
        yield break;
    }
}
", @"
using System.Collections.Generic;

public class Foo
{
    public void Bar()
    {
        IEnumerable<Foo> x = GetValues();
    }

    public IEnumerable<Foo> GetValues()
    {
        yield break;
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId, additionalKey1: CodeFixIdentifiers.ChangeTypeAccordingToInitializer));
        }
    }
}
