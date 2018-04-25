// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

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
                Changed += new EventHandler<ConsoleCancelEventArgs>(OnChanged);
                Changed -= new EventHandler<ConsoleCancelEventArgs>(OnChanged);
            }

            protected virtual void OnChanged(object sender, ConsoleCancelEventArgs e)
            {
            }

            public event EventHandler<ConsoleCancelEventArgs> Changed;

            protected virtual void OnEventName(ConsoleCancelEventArgs e)
            {
                Changed?.Invoke(this, e);
            }
        }
    }
}
