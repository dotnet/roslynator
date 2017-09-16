// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#pragma warning disable RCS1016

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class RemoveRedundantSealedModifier
    {
        public sealed class ObjectCollection : Collection<object>
        {
            public ObjectCollection(IList<object> list) : base(list)
            {
            }

            public sealed override string ToString()
            {
                return null;
            }
        }
    }
}