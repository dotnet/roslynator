// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Roslynator.CSharp.Analyzers.Tests
{
#pragma warning disable RCS1016, RCS1018
    public static class RemoveImplementationFromAbstractOrInterfaceMember
    {
        public interface Foo
        {
            string MethodName()
            {
                return "";
            }

            string PropertyName
            {
                get { }
                set { }
            }

            object this[int index]
            {
                get {  }
                set {  }
            }

            event EventHandler EventName
            {
                add { }
                remove { }
            }
        }

        public abstract class Bar
        {
            public abstract string MethodName()
            {
                return "";
            }

            public abstract string PropertyName
            {
                get {  }
                set {  }
            }

            public abstract object this[int index]
            {
                get {  }
                set {  }
            }

            public abstract event EventHandler EventName
            {
                add { }
                remove { }
            }
        }
    }
}
