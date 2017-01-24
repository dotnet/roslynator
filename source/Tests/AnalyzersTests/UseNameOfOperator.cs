// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Roslynator.CSharp.Analyzers.Tests
{
#pragma warning disable RCS1016, RCS1048
    internal static class UseNameOfOperator
    {
        private class Foo
        {
            public Foo(object parameter)
            {
                if (parameter == null)
                    throw new ArgumentNullException("parameter", "message");
            }

            private static void FooMethod(object parameter)
            {
                if (parameter == null)
                    throw new ArgumentNullException("parameter", "message");

                CheckParam("parameter");

                var items = new List<string>();

                IEnumerable<string> q = items.Select(item =>
                {
                    if (item == null)
                        throw new ArgumentNullException("item", "message");

                    return item;
                });

                IEnumerable<string> q2 = items.Select((item) =>
                {
                    if (item == null)
                        throw new ArgumentNullException("item", "message");

                    return item;
                });

                IEnumerable<string> q3 = items.Select(delegate(string item)
                {
                    if (item == null)
                        throw new ArgumentNullException("item", "message");

                    return item;
                });

                IEnumerable<string> LocalFunction(object parameter2)
                {
                    if (parameter == null)
                        throw new ArgumentNullException("parameter2", "message");

                    yield break;
                }
            }

            private static void CheckParam(string parameterName)
            {
            }

            private object this[int index]
            {
                get { return Items[index]; }
                set { Items[index] = value; }
            }

            private Collection<object> Items { get; } = new Collection<object>();

            private class Bar : INotifyPropertyChanged
            {
                private string _value;

                public event PropertyChangedEventHandler PropertyChanged;

                public string Value
                {
                    get { return _value; }
                    set
                    {
                        if (_value != value)
                            PropertyChanged(this, new PropertyChangedEventArgs("Value"));

                        _value = value;
                    }
                }
            }
        }
    }
#pragma warning restore RCS1016, RCS1048
}
