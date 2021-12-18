// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0062InlineMethodTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.InlineMethod;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.InlineMethod)]
        public async Task Test()
        {
            await VerifyRefactoringAsync(@"
namespace N
{
    class C
    {
        void M()
        {
            [||]M2();
        }

        void M2()
        {
            var x = typeof(N.B);
        }

        object B => null;
    }

    static class B
    {
    }
}", @"
namespace N
{
    class C
    {
        void M()
        {
            var x = typeof(N.B);
        }

        void M2()
        {
            var x = typeof(N.B);
        }

        object B => null;
    }

    static class B
    {
    }
}", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.InlineMethod)]
        public async Task Test_ReplaceTypeArgumentWithPredefinedType()
        {
            await VerifyRefactoringAsync(@"
namespace N
{
    class C
    {
        void M()
        {
            new object().[||]EM();
        }
    }

    class C<T>
    {
    }

    static class E
    {
        public static C<T> EM<T>(this T _) => new C<T>();
    }
}
", @"
namespace N
{
    class C
    {
        void M()
        {
            new C<object>();
        }
    }

    class C<T>
    {
    }

    static class E
    {
        public static C<T> EM<T>(this T _) => new C<T>();
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.InlineMethod)]
        public async Task Test_ReplaceTypeArgumentWithQualifiedName()
        {
            await VerifyRefactoringAsync(@"
namespace N
{
    class C
    {
        void M()
        {
            new N2.C().[||]EM();
        }
    }

    class C<T>
    {
        public C(T parameter)
        {
        }
    }

    static class E
    {
        public static C<T> EM<T>(this T t) => new C<T>(t);
    }
}

namespace N2
{
    class C
    {
    }
}
", @"
namespace N
{
    class C
    {
        void M()
        {
            new C<N2.C>(new N2.C());
        }
    }

    class C<T>
    {
        public C(T parameter)
        {
        }
    }

    static class E
    {
        public static C<T> EM<T>(this T t) => new C<T>(t);
    }
}

namespace N2
{
    class C
    {
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.InlineMethod)]
        public async Task Test_IdentifierNameInsideTypeOf()
        {
            await VerifyRefactoringAsync(@"
using System;
using System.Runtime.CompilerServices;
using System.Reflection;

class C
{
    private static bool M(Type type)
    {
        return type.Has[||]Attribute<CompilerGeneratedAttribute>() || type.IsNotPublic;
    }
}

public static class E
{
    public static bool HasAttribute<T>(this MemberInfo mi) where T : Attribute
    {
        return Attribute.IsDefined(mi, typeof(T));
    }
}
", @"
using System;
using System.Runtime.CompilerServices;
using System.Reflection;

class C
{
    private static bool M(Type type)
    {
        return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute)) || type.IsNotPublic;
    }
}

public static class E
{
    public static bool HasAttribute<T>(this MemberInfo mi) where T : Attribute
    {
        return Attribute.IsDefined(mi, typeof(T));
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.InlineMethod)]
        public async Task TestNoRefactoring()
        {
            await VerifyNoRefactoringAsync(@"
class C
{
    void M()
    {
        var c = this;

        C a = c?.[||]A();
    }

    C A()
    {
        return B;
    }

    C B
    {
        get { return null; }
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }
    }
}
