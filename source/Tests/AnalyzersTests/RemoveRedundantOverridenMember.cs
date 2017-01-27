// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Roslynator.CSharp.Analyzers.Tests
{
#pragma warning disable RCS1016, RCS1085
    public static class RemoveRedundantOverridenMember
    {
        public class Base
        {
            private string _property;

            private readonly Collection<string> _items;

            public virtual string Method()
            {
                return null;
            }

            public virtual void VoidMethod()
            {
            }

            public virtual string Property
            {
                get { return _property; }
                set { _property = value; }
            }

            public virtual string ReadOnlyProperty
            {
                get { return _property; }
            }

            public virtual string this[int index]
            {
                get { return _items[index]; }
                set { _items[index] = value; }
            }
        }

        public class Derived : Base
        {
            public override string Method()
            {
                return base.Method();
            }

            public override void VoidMethod()
            {
                base.VoidMethod();
            }

            public override string Property
            {
                get { return base.Property; }
                set { base.Property = value; }
            }

            public override string ReadOnlyProperty
            {
                get { return base.ReadOnlyProperty; }
            }

            public override string this[int index]
            {
                get { return base[index]; }
                set { base[index] = value; }
            }
        }

        public class Derived2 : Base
        {
            private string _property;
            private readonly Collection<string> _items;

            public override string Method()
            {
                return null;
            }

            public override void VoidMethod()
            {
                VoidMethod();
            }

            public override string Property
            {
                get { return _property; }
                set { _property = value; }
            }

            public override string this[int index]
            {
                get { return _items[index]; }
                set { _items[index] = value; }
            }
        }

        public class Derived3 : Base
        {
            public sealed override string Method()
            {
                return base.Method();
            }

            public sealed override void VoidMethod()
            {
                base.VoidMethod();
            }

            public sealed override string Property
            {
                get { return base.Property; }
                set { base.Property = value; }
            }

            public sealed override string this[int index]
            {
                get { return base[index]; }
                set { base[index] = value; }
            }
        }
    }
#pragma warning restore RCS1016, RCS1085
}
