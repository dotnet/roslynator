// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

#pragma warning disable RCS1016, RCS1048, RCS1081, RCS1163, RCS1176

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class MarkFieldAsReadOnly
    {
        public partial class Foo
        {
            private static string _sf;
            private int _f;
            private StringSplitOptions _options;

            //n

            private string _f2, _f3;
            private static string _staticAssignedInInstanceConstructor;
            private SpinLock _spinLock;
            private string _assigned;
            private string _inSimpleLambda;
            private string _inParenthesizedLambda;
            private string _inAnonymousMethod;
            private string _inLocalFunction;

            static Foo()
            {
                _sf = null;
            }

            public Foo()
            {
                _f = 0;
                _options = 0;
                _f2 = null;
                _spinLock = new SpinLock();
                _staticAssignedInInstanceConstructor = null;
            }

            private void Bar()
            {
                _assigned = null;
                _f3 = null;
            }
        }

        private class BaseClassName
        {
        }

        private class ClassName<T> : BaseClassName
        {
            private BaseClassName _f;

            public ClassName<TResult> MethodName<TResult>()
            {
                return new ClassName<TResult>() { _f = this };
            }
        }

        public partial class Foo
        {
            public Foo(object parameter)
            {
                var items = new List<string>();

                IEnumerable<string> q = items.Select(f =>
                {
                    _inSimpleLambda = null;
                    return f;
                });

                IEnumerable<string> q2 = items.Select((f) =>
                {
                    _inParenthesizedLambda = null;
                    return f;
                });

                IEnumerable<string> q3 = items.Select(delegate(string f)
                {
                    _inAnonymousMethod = null;
                    return f;
                });

                LocalFunction();

                void LocalFunction()
                {
                    _inLocalFunction = null;
                }
            }
        }
    }
}
