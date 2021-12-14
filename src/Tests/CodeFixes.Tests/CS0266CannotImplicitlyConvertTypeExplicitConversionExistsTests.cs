// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS0266CannotImplicitlyConvertTypeExplicitConversionExistsTests : AbstractCSharpCompilerDiagnosticFixVerifier<ExpressionCodeFixProvider>
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.CS0266_CannotImplicitlyConvertTypeExplicitConversionExists;

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS0266_CannotImplicitlyConvertTypeExplicitConversionExists)]
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

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS0266_CannotImplicitlyConvertTypeExplicitConversionExists)]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoFixAsync(@"
class C
{
    void M()
    {
        bool? x = null;
        bool y = false;

        y = x;
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId, additionalKey1: CodeFixIdentifiers.AddComparisonWithBooleanLiteral));
        }
    }
}
