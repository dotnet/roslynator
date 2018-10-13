// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS1061TypeDoesNotContainDefinitionAndNoExtensionMethodCouldBeFoundTests : AbstractCSharpCompilerDiagnosticFixVerifier
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.TypeDoesNotContainDefinitionAndNoExtensionMethodCouldBeFound;

        public override CodeFixProvider FixProvider { get; } = new TypeDoesNotContainDefinitionCodeFixProvider();

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.TypeDoesNotContainDefinitionAndNoExtensionMethodCouldBeFound)]
        public async Task Test_RemoveAwaitKeyword()
        {
            await VerifyFixAsync(@"
using System.Threading.Tasks;

#pragma warning disable CS1998

class C
{
    void M()
    {
        async Task<string> GetAsync()
        {
            return await Foo();
        }

        async Task DoAsync()
        {
            await Foo();
        }

        string Foo() => null;
    }
}
", @"
using System.Threading.Tasks;

#pragma warning disable CS1998

class C
{
    void M()
    {
        async Task<string> GetAsync()
        {
            return Foo();
        }

        async Task DoAsync()
        {
            Foo();
        }

        string Foo() => null;
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.TypeDoesNotContainDefinitionAndNoExtensionMethodCouldBeFound)]
        public async Task Test_LengthToCount()
        {
            await VerifyFixAsync(@"
using System.Collections.Generic;
using System.Collections.ObjectModel;

class C
{
    void M()
    {
        int i = 0;

        var list = new List<object>();
        var collection = new Collection<object>();

        i = list.Length;
        i = collection.Length;
    }
}
", @"
using System.Collections.Generic;
using System.Collections.ObjectModel;

class C
{
    void M()
    {
        int i = 0;

        var list = new List<object>();
        var collection = new Collection<object>();

        i = list.Count;
        i = collection.Count;
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.TypeDoesNotContainDefinitionAndNoExtensionMethodCouldBeFound)]
        public async Task Test_LengthToCount_ConditionalAccess()
        {
            await VerifyFixAsync(@"
using System.Collections.Generic;
using System.Collections.ObjectModel;

class C
{
    void M()
    {
        int? ni = 0;

        var list = new List<object>();
        var collection = new Collection<object>();

        ni = list?.Length;
        ni = collection?.Length;
    }
}
", @"
using System.Collections.Generic;
using System.Collections.ObjectModel;

class C
{
    void M()
    {
        int? ni = 0;

        var list = new List<object>();
        var collection = new Collection<object>();

        ni = list?.Count;
        ni = collection?.Count;
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }
    }
}
