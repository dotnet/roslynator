// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS8403MethodWithIteratorBlockMustBeAsyncToReturnIAsyncEnumerableOfTTests : AbstractCSharpCompilerDiagnosticFixVerifier<TokenCodeFixProvider>
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.CS8403_MethodWithIteratorBlockMustBeAsyncToReturnIAsyncEnumerableOfT;

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS8403_MethodWithIteratorBlockMustBeAsyncToReturnIAsyncEnumerableOfT)]
        public async Task Test_Method()
        {
            await VerifyFixAsync(@"
using System.Collections.Generic;
using System.Threading.Tasks;

class C
{
    IAsyncEnumerable<string> DoAsync()
    {
        yield return await Task.FromResult("""");
    }
}
", @"
using System.Collections.Generic;
using System.Threading.Tasks;

class C
{
    async IAsyncEnumerable<string> DoAsync()
    {
        yield return await Task.FromResult("""");
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId, CodeFixIdentifiers.AddAsyncModifier));
        }

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS8403_MethodWithIteratorBlockMustBeAsyncToReturnIAsyncEnumerableOfT)]
        public async Task Test_LocalFunction()
        {
            await VerifyFixAsync(@"
using System.Collections.Generic;
using System.Threading.Tasks;

class C
{
    void M()
    {
        IAsyncEnumerable<string> DoAsync()
        {
            yield return await Task.FromResult("""");
        }
    }
}
", @"
using System.Collections.Generic;
using System.Threading.Tasks;

class C
{
    void M()
    {
        async IAsyncEnumerable<string> DoAsync()
        {
            yield return await Task.FromResult("""");
        }
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId, CodeFixIdentifiers.AddAsyncModifier));
        }
    }
}
