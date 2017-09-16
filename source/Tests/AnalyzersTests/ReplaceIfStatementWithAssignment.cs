// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class ReplaceIfStatementWithAssignment
    {
        public static void Foo()
        {
            bool f = false;

            bool x = false;

            if (f)
            {
                x = true;
            }
            else
            {
                x = false;
            }

            if (f)
                x = true;
            else
            {
                x = false;
            }

            if (f)
            {
                x = true;
            }
            else
                x = false;

            if (f)
                x = true;
            else
                x = false;

            if (f)
            {
                x = false;
            }
            else
            {
                x = true;
            }

            if (f)
            {
            }
            else if (f)
            {
                x = true;
            }
            else
            {
                x = false;
            }

            if (f)
            {
                x = true;
            }
            else
            {
                x = true;
            }

            if (f)
            {
                x = false;
            }
            else
            {
                x = false;
            }
        }
    }
}
