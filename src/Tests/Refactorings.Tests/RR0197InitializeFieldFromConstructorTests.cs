// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CodeRefactorings;
using Roslynator.CSharp.Refactorings;
using Xunit;
using static Roslynator.Tests.CSharp.CSharpCodeRefactoringVerifier;

namespace Roslynator.Refactorings.Tests
{
    public static class RR0197InitializeFieldFromConstructorTests
    {
        private const string RefactoringId = RefactoringIdentifiers.InitializeFieldFromConstructor;

        private static CodeRefactoringProvider CodeRefactoringProvider { get; } = new RoslynatorCodeRefactoringProvider();

        [Fact]
        public static void TestCodeRefactoring()
        {
            VerifyRefactoring(
@"
class Foo : FooBase
{
<<<    private string bar;

    private string _bar2, _bar3;>>>

    public Foo()
    {
    }

    public Foo(object parameter)
    {
        M(parameter);
    }

    public Foo(object parameter1, object parameter2)
        : this(parameter1, parameter2, null)
    {
        M(bar);
    }

    public Foo(object parameter1, object parameter2, object bar)
        : base(parameter1, parameter2)
    {
        M(bar);
    }

    public void M(object parameter) { }
}

class FooBase
{
    public FooBase() { }
    public FooBase(object parameter1, object bar) { }
}
", @"
class Foo : FooBase
{
    private string bar;

    private string _bar2, _bar3;

    public Foo(string bar, string bar2, string bar3)
    {
        this.bar = bar;
        _bar2 = bar2;
        _bar3 = bar3;
    }

    public Foo(object parameter, string bar, string bar2, string bar3)
    {
        M(parameter);
        this.bar = bar;
        _bar2 = bar2;
        _bar3 = bar3;
    }

    public Foo(object parameter1, object parameter2, string bar, string bar2, string bar3)
        : this(parameter1, parameter2, null, bar, bar2, bar3)
    {
        M(bar);
        this.bar = bar;
        _bar2 = bar2;
        _bar3 = bar3;
    }

    public Foo(object parameter1, object parameter2, object bar, string bar2, string bar22, string bar3)
        : base(parameter1, parameter2)
    {
        M(bar);
        this.bar = bar2;
        _bar2 = bar22;
        _bar3 = bar3;
    }

    public void M(object parameter) { }
}

class FooBase
{
    public FooBase() { }
    public FooBase(object parameter1, object bar) { }
}
", CodeRefactoringProvider, RefactoringId);
        }
    }
}
