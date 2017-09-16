// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class RemoveRedundantDelegateCreation
    {
        private class Foo
        {
            public void Bar()
            {
                Changed += new EventHandler(OnChanged);
                Changed -= new EventHandler(OnChanged);
            }

            protected virtual void OnChanged(object sender, EventArgs e)
            {
                throw new NotImplementedException();
            }

            public event EventHandler Changed;

            protected virtual void OnEventName(EventArgs e)
            {
                Changed?.Invoke(this, e);
            }
        }

        private class Foo2
        {
            public void Bar()
            {
                Changed += new EventHandler<CancelEventArgs>(OnChanged);
                Changed -= new EventHandler<CancelEventArgs>(OnChanged);
            }

            protected virtual void OnChanged(object sender, CancelEventArgs e)
            {
            }

            public event EventHandler<CancelEventArgs> Changed;

            protected virtual void OnEventName(CancelEventArgs e)
            {
                Changed?.Invoke(this, e);
            }
        }
    }
}
