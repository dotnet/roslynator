// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS0126ObjectOfTypeConvertibleToTypeIsRequiredTests : AbstractCSharpCompilerDiagnosticFixVerifier<TokenCodeFixProvider>
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.CS0126_ObjectOfTypeConvertibleToTypeIsRequired;

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS0126_ObjectOfTypeConvertibleToTypeIsRequired)]
        public async Task Test()
        {
            await VerifyFixAsync(@"
using System;

class C
{
    DateTime M()
    {
        return;
    }
}
", @"
using System;

class C
{
    DateTime M()
    {
        return default;
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }
    }
}
