// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.CodeFixes;
using Xunit;
using static Roslynator.Tests.CSharp.CSharpDiagnosticVerifier;

namespace Roslynator.Analyzers.Tests
{
    public static class RCS1114RemoveRedundantDelegateCreationTests
    {
        private static DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.RemoveRedundantDelegateCreation;

        private static DiagnosticAnalyzer Analyzer { get; } = new RemoveRedundantDelegateCreationAnalyzer();

        private static CodeFixProvider CodeFixProvider { get; } = new AssignmentExpressionCodeFixProvider();

        [Fact]
        public static void TestDiagnosticWithCodeFix_EventHandler()
        {
            VerifyDiagnosticAndFix(@"
using System;

class Foo
{
    void M()
    {
        Changed += <<<new EventHandler(Foo_Changed)>>>;
        Changed -= <<<new EventHandler(Foo_Changed)>>>;
    }

    protected virtual void Foo_Changed(object sender, EventArgs e) { }

    public event EventHandler Changed;
}
", @"
using System;

class Foo
{
    void M()
    {
        Changed += Foo_Changed;
        Changed -= Foo_Changed;
    }

    protected virtual void Foo_Changed(object sender, EventArgs e) { }

    public event EventHandler Changed;
}
", Descriptor, Analyzer, CodeFixProvider);
        }

        [Fact]
        public static void TestDiagnosticWithCodeFix_EventHandlerOfT()
        {
            VerifyDiagnosticAndFix(@"
using System;

class Foo
{
    void M()
    {
        Changed += <<<new EventHandler<FooEventArgs>(Foo_Changed)>>>;
        Changed -= <<<new EventHandler<FooEventArgs>(Foo_Changed)>>>;
    }

    protected virtual void Foo_Changed(object sender, FooEventArgs e) { }

    public event EventHandler<FooEventArgs> Changed;

    public class FooEventArgs : EventArgs
    {
    }
}
", @"
using System;

class Foo
{
    void M()
    {
        Changed += Foo_Changed;
        Changed -= Foo_Changed;
    }

    protected virtual void Foo_Changed(object sender, FooEventArgs e) { }

    public event EventHandler<FooEventArgs> Changed;

    public class FooEventArgs : EventArgs
    {
    }
}
", Descriptor, Analyzer, CodeFixProvider);
        }

        [Fact]
        public static void TestDiagnosticWithCodeFix_CustomEventHandler()
        {
            VerifyDiagnosticAndFix(@"
using System;

class Foo
{
    void M()
    {
        Changed += <<<new FooEventHandler(Foo_Changed)>>>;
        Changed -= <<<new FooEventHandler(Foo_Changed)>>>;
    }

    protected virtual void Foo_Changed(object sender, FooEventArgs e) { }

    public event FooEventHandler Changed;

    public delegate void FooEventHandler(object sender, FooEventArgs args);

    public class FooEventArgs : EventArgs
    {
    }
}
", @"
using System;

class Foo
{
    void M()
    {
        Changed += Foo_Changed;
        Changed -= Foo_Changed;
    }

    protected virtual void Foo_Changed(object sender, FooEventArgs e) { }

    public event FooEventHandler Changed;

    public delegate void FooEventHandler(object sender, FooEventArgs args);

    public class FooEventArgs : EventArgs
    {
    }
}
", Descriptor, Analyzer, CodeFixProvider);
        }

        [Fact]
        public static void TestDiagnosticWithCodeFix_TEventArgs()
        {
            VerifyDiagnosticAndFix(@"
using System;

class Foo<TEventArgs>
{
    void M()
    {
        Changed += <<<new EventHandler<TEventArgs>(Foo_Changed)>>>;
        Changed -= <<<new EventHandler<TEventArgs>(Foo_Changed)>>>;
    }

    protected virtual void Foo_Changed(object sender, TEventArgs e) { }

    public event EventHandler<TEventArgs> Changed;
}
", @"
using System;

class Foo<TEventArgs>
{
    void M()
    {
        Changed += Foo_Changed;
        Changed -= Foo_Changed;
    }

    protected virtual void Foo_Changed(object sender, TEventArgs e) { }

    public event EventHandler<TEventArgs> Changed;
}
", Descriptor, Analyzer, CodeFixProvider);
        }
    }
}
