// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests;

public class RR0201ConvertInterpolatedStringToStringFormatTests : AbstractCSharpRefactoringVerifier
{
    public override string RefactoringId { get; } = RefactoringIdentifiers.ConvertInterpolatedStringToStringFormat;

    [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ConvertInterpolatedStringToStringFormat)]
    public async Task Test()
    {
        await VerifyRefactoringAsync("""
class C
{
    void M(string name, double value)
    {
        var s = [||]$"name: {name,0:f}, value: {value}";
    }
}
""", """
class C
{
    void M(string name, double value)
    {
        var s = string.Format("name: {0,0:f}, value: {1}", name, value);
    }
}
""", equivalenceKey: EquivalenceKey.Create(RefactoringId));
    }

    [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ConvertInterpolatedStringToStringFormat)]
    public async Task Test_InsideInterpolation()
    {
        await VerifyRefactoringAsync("""
class C
{
    void M(string name)
    {
        var s = $"prefix {[||]name} suffix";
    }
}
""", """
class C
{
    void M(string name)
    {
        var s = string.Format("prefix {0} suffix", name);
    }
}
""", equivalenceKey: EquivalenceKey.Create(RefactoringId));
    }

    [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ConvertInterpolatedStringToStringFormat)]
    public async Task Test_OnLiteralText()
    {
        await VerifyRefactoringAsync("""
class C
{
    void M(string name)
    {
        var s = $"prefix[||] {name} suffix";
    }
}
""", """
class C
{
    void M(string name)
    {
        var s = string.Format("prefix {0} suffix", name);
    }
}
""", equivalenceKey: EquivalenceKey.Create(RefactoringId));
    }

    [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ConvertInterpolatedStringToStringFormat)]
    public async Task Test_VerbatimInterpolatedString()
    {
        await VerifyRefactoringAsync("""
class C
{
    void M(string name)
    {
        var s = [||]$@"path\{name}\file";
    }
}
""", """
class C
{
    void M(string name)
    {
        var s = string.Format(@"path\{0}\file", name);
    }
}
""", equivalenceKey: EquivalenceKey.Create(RefactoringId));
    }
}
