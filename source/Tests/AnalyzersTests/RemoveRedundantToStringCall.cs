// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class RemoveRedundantToStringCall
    {
        public static void Foo()
        {
            object o = null;

            string s = null;

            Entity e = null;

            s = s.ToString();

            s = $"{o.ToString()}{s.ToString()}{e.ToString()}";
        }

        public abstract class Entity
        {
            new public string ToString()
            {
                return null;
            }
        }
    }
}
