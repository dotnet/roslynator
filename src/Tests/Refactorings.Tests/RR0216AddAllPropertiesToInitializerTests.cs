// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0216AddAllPropertiesToInitializerTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.AddAllPropertiesToInitializer;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddAllPropertiesToInitializer)]
        public async Task Test_EmptyObjectInitializer()
        {
            await VerifyRefactoringAsync(@"
using System;

class C
{
    public string P1 { get; set; }
    public int? P3 { get; set; }
    public int P2 { get; set; }
    public DateTime P4 { get; set; }

    void M()
    {
        var x = new C() {[||] };
    }
}
", @"
using System;

class C
{
    public string P1 { get; set; }
    public int? P3 { get; set; }
    public int P2 { get; set; }
    public DateTime P4 { get; set; }

    void M()
    {
        var x = new C() { P1 = , P2 = , P3 = , P4 = };
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddAllPropertiesToInitializer)]
        public async Task Test_EmptyRecordInitializer()
        {
            await VerifyRefactoringAsync(@"
using System;

record C
{
    public string P1 { get; set; }
    public int? P3 { get; set; }
    public int P2 { get; set; }
    public DateTime P4 { get; set; }

    void M()
    {
        var x = this with {[||] };
    }
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", @"
using System;

record C
{
    public string P1 { get; set; }
    public int? P3 { get; set; }
    public int P2 { get; set; }
    public DateTime P4 { get; set; }

    void M()
    {
        var x = this with { P1 = , P2 = , P3 = , P4 = };
    }
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddAllPropertiesToInitializer)]
        public async Task Test_ObjectInitializer_Accessibility()
        {
            await VerifyRefactoringAsync(@"
class C
{
    public string P0 { get; set; }
    public string P1 { get; set; }
    public string P2 { get; protected set; }
    public string P3 { get; private set; }
    public string P4 { get; }
}

class C2 : C
{
    void M()
    {
        var x = new C2() { P0 = null, [||] };
    }
}
", @"
class C
{
    public string P0 { get; set; }
    public string P1 { get; set; }
    public string P2 { get; protected set; }
    public string P3 { get; private set; }
    public string P4 { get; }
}

class C2 : C
{
    void M()
    {
        var x = new C2() { P0 = null, P1 = , P2 = };
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddAllPropertiesToInitializer)]
        public async Task Test_ObjectInitializer_Span()
        {
            await VerifyRefactoringAsync(@"
class C
{
    public string P0 { get; set; }
    public string P1 { get; set; }
    public string P2 { get; set; }
}

class C2 : C
{
    void M()
    {
        var x = new C2() { P0 = null [||]};
    }
}
", @"
class C
{
    public string P0 { get; set; }
    public string P1 { get; set; }
    public string P2 { get; set; }
}

class C2 : C
{
    void M()
    {
        var x = new C2() { P0 = null, P1 = , P2 = };
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddAllPropertiesToInitializer)]
        public async Task Test_ObjectInitializer_Span2()
        {
            await VerifyRefactoringAsync(@"
class C
{
    public string P0 { get; set; }
    public string P1 { get; set; }
}

class C2 : C
{
    void M()
    {
        var x = new C2()
        {[||]
        };
    }
}
", @"
class C
{
    public string P0 { get; set; }
    public string P1 { get; set; }
}

class C2 : C
{
    void M()
    {
        var x = new C2()
        {
            P0 = ,
            P1 =
        };
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddAllPropertiesToInitializer)]
        public async Task TestNoRefactoring_AllObjectPropertiesInitialized()
        {
            await VerifyNoRefactoringAsync(@"
class C
{
    public string P1 { get; set; }
    public int P2 { get; set; }

    void M()
    {
        var x = new C() { P1 = null, P2 = 0 [||] };
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddAllPropertiesToInitializer)]
        public async Task TestNoRefactoring_AnonymousType()
        {
            await VerifyNoRefactoringAsync(@"
class C
{
    void M()
    {
        var x = new {[||] P = 0 };
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }
    }
}
