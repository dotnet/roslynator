// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Roslynator.CSharp.Analyzers.Tests
{
#pragma warning disable RCS1016
    public static class MemberTypeMustMatchOverridenMemberType
    {
        private class Base
        {
            public virtual void VoidMethod()
            {
            }

            public virtual IEnumerable<string> Method()
            {
                yield break;
            }

            public virtual IEnumerable<string> Property { get; }

            public virtual IEnumerable<string> this[int index]
            {
                get { yield break; }
            }

            public virtual event EventHandler Changed;

            public virtual event EventHandler Changed2
            {
                add { }
                remove { }
            }
        }

        private class Derived : Base
        {
            public override object VoidMethod()
            {
                VoidMethod();
            }

            public override IEnumerable<object> Method()
            {
                return Method();
            }

            public override IEnumerable<object> Property
            {
                get { yield break; }
            }

            public override IEnumerable<object> this[int index]
            {
                get { yield break; }
            }

            public override event EventHandler<CancelEventArgs> Changed;

            public override event EventHandler<CancelEventArgs> Changed2
            {
                add { }
                remove { }
            }
        }
    }
}
