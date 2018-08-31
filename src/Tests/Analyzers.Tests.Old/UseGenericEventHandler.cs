// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;

#pragma warning disable RCS1079

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class UseGenericEventHandler
    {
        public class Foo
        {
            public event FooEventHandler EventName;

            public event FooEventHandler EventName2
            {
                add { }
                remove { }
            }
        }

        public interface FooInterface
        {
            event FooEventHandler Changed;
        }

        public class FooImplementation : FooInterface
        {
            public event FooEventHandler Changed;
        }

        public class FooImplementation2 : FooInterface
        {
            event FooEventHandler FooInterface.Changed
            {
                add { throw new NotImplementedException(); }
                remove { throw new NotImplementedException(); }
            }
        }

        //n

        public class Foo2
        {
            public event EventHandler EventName;
        }

        public class FooEventArgs : EventArgs
        {
        }

        public delegate void FooEventHandler(object sender, FooEventArgs args);

        public interface INotifyPropertyChangedEx : INotifyPropertyChanged
        {
        }

        public class BaseClass : INotifyPropertyChangedEx
        {
            public event PropertyChangedEventHandler PropertyChanged;
        }

        public class BaseClass2 : INotifyPropertyChangedEx
        {
            event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
            {
                add { throw new NotImplementedException(); }
                remove { throw new NotImplementedException(); }
            }
        }
    }
}
