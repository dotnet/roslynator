// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1110DeclareTypeInsideNamespaceTests : AbstractCSharpDiagnosticVerifier<DeclareTypeInsideNamespaceAnalyzer, DeclareTypeInsideNamespaceCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.DeclareTypeInsideNamespace;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareTypeInsideNamespace)]
        public async Task TestNoDiagnostics()
        {
            await VerifyNoDiagnosticAsync(@"
namespace N;

class C
{
}
");
        }
    }
}
