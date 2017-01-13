// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Analyzers.Tests
{
#pragma warning disable RCS1085 
    internal static class AvoidMultilineExpressionBodyRefactoring
    {
        private class Entity
        {
            public Entity() => FooMethod
                ();

            ~Entity() => FooMethod
                ();

            public string FooMethod() => FooMethod
                ();

            public void FooVoidMethod() => FooMethod
                ();

            public string FooProperty => string
                .Empty;

            private string _fooProperty2;

            public string FooProperty2
            {
                get => this
                    ._fooProperty2;
                set => _fooProperty2
                    = value;
            }

            public string this[int index] => FooMethod
                ();

            public string this[string index]
            {
                get => this
                    ._fooProperty2;
                set => _fooProperty2
                    = value;
            }

            public static explicit operator Entity(string value) => new
                Entity();

            public static explicit operator string(Entity value) => string
                .Empty;

            private EventHandler _event;

            public event EventHandler Event
            {
                add => _event 
                    += value;

                remove => _event 
                    -= value;
            }
        }
    }
#pragma warning restore RCS1085
}
