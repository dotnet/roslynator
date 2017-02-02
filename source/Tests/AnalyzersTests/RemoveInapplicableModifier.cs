// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class RemoveInapplicableModifier
    {
        private interface InterfaceName123
        {
            public string Value { get; private set; }
        }

        public static void MethodName()
        {
        }

        public event EventHandler EventName
        {
            private add { }
            private remove { }
        }
    }
}