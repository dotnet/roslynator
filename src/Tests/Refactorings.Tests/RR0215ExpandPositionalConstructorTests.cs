// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0215ExpandPositionalConstructorTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.ExpandPositionalConstructor;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ConvertSwitchExpressionToSwitchStatement)]
        public async Task Test()
        {
            await VerifyRefactoringAsync(@"
public record R([||]string P, object O);

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", @"
public record R
{
    public R(string p, object o)
    {
        P = p;
        O = o;
    }

    public string P { get; init; }
    public object O { get; init; }
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ConvertSwitchExpressionToSwitchStatement)]
        public async Task Test_RecordStruct()
        {
            await VerifyRefactoringAsync(@"
public record struct R([||]string P, object O);

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", @"
public record struct R
{
    public R(string p, object o)
    {
        P = p;
        O = o;
    }

    public string P { get; set; }
    public object O { get; set; }
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ConvertSwitchExpressionToSwitchStatement)]
        public async Task Test_ReadOnlyRecordStruct()
        {
            await VerifyRefactoringAsync(@"
public readonly record struct R([||]string P, object O);

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", @"
public readonly record struct R
{
    public R(string p, object o)
    {
        P = p;
        O = o;
    }

    public string P { get; init; }
    public object O { get; init; }
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ConvertSwitchExpressionToSwitchStatement)]
        public async Task Test_AttributeWithoutTarget()
        {
            await VerifyRefactoringAsync(@"
using System;

public record R([||]string P, [Foo] object O);

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FooAttribute : Attribute { }

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", @"
using System;

public record R
{
    public R(string p, [Foo] object o)
    {
        P = p;
        O = o;
    }

    public string P { get; init; }
    public object O { get; init; }
}

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FooAttribute : Attribute { }

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", options: Options.AddAllowedCompilerDiagnosticId("CS0612"), equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ConvertSwitchExpressionToSwitchStatement)]
        public async Task Test_AttributeWithTarget()
        {
            await VerifyRefactoringAsync(@"
using System;

public record R([||][property: Obsolete] string P, object O);

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", @"
using System;

public record R
{
    public R(string p, object o)
    {
        P = p;
        O = o;
    }

    [Obsolete]
    public string P { get; init; }
    public object O { get; init; }
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", options: Options.AddAllowedCompilerDiagnosticId("CS0612"), equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ConvertSwitchExpressionToSwitchStatement)]
        public async Task Test_AttributeWithTarget_Multiline()
        {
            await VerifyRefactoringAsync(@"
using System;

namespace N
{
    public record R(
        [||][property: Obsolete] string P,
        object O);
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", @"
using System;

namespace N
{
    public record R
    {
        public R(
            string p,
            object o)
        {
            P = p;
            O = o;
        }

        [Obsolete]
        public string P { get; init; }
        public object O { get; init; }
    }
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", options: Options.AddAllowedCompilerDiagnosticId("CS0612"), equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ConvertSwitchExpressionToSwitchStatement)]
        public async Task Test_WithOpenCloseBraces()
        {
            await VerifyRefactoringAsync(@"
public record R([|string P, object O|])
{
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", @"
public record R
{
    public R(string p, object o)
    {
        P = p;
        O = o;
    }

    public string P { get; init; }
    public object O { get; init; }
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ConvertSwitchExpressionToSwitchStatement)]
        public async Task Test_Multiline()
        {
            await VerifyRefactoringAsync(@"
namespace N
{
    public record R(
        [||]string P,
        object O);
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", @"
namespace N
{
    public record R
    {
        public R(
            string p,
            object o)
        {
            P = p;
            O = o;
        }

        public string P { get; init; }
        public object O { get; init; }
    }
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ConvertSwitchExpressionToSwitchStatement)]
        public async Task Test_BaseRecord()
        {
            await VerifyRefactoringAsync(@"
public record C([||]string P, object O) : B(P);

public record B(string P);

namespace System.Runtime.CompilerServices { internal static class IsExternalInit { } }
", @"
public record C : B
{
    public C(string p, object o) : base(p)
    {
        P = p;
        O = o;
    }

    public object O { get; init; }
}

public record B(string P);

namespace System.Runtime.CompilerServices { internal static class IsExternalInit { } }
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }
    }
}
