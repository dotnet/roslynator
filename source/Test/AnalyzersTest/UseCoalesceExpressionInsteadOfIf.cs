// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

#pragma warning disable RCS1002, RCS1004, RCS1007, RCS1126, CS0162

namespace Roslynator.CSharp.Analyzers.Test
{
    public static class UseCoalesceExpressionInsteadOfIf
    {
        public static void MethodName()
        {
            object x = null;
            object y = null;
            object z = null;

            if (x != null)
            {
                z = x;
            }
            else
            {
                z = y;
            }

            if (x != null)
                z = x;
            else
                z = y;

            if (x == null)
            {
                z = y;
            }
            else
            {
                z = x;
            }

            if (x == null)
                z = y;
            else
                z = x;
        }

        public static object MethodName2()
        {
            object x = null;
            object y = null;

            if (x != null)
            {
                return x;
            }
            else
            {
                return y;
            }

            if (x != null)
                return x;
            else
                return y;

            if (x == null)
            {
                return y;
            }
            else
            {
                return x;
            }

            if (x == null)
                return y;
            else
                return x;
        }

        public static object MethodName3()
        {
            object x = null;
            object y = null;

            if (x != null)
            {
                return x;
            }

            return y;

            if (x != null)
                return x;

            return y;

            if (x == null)
            {
                return y;
            }

            return x;

            if (x == null)
                return y;

            return x;
        }

        public static IEnumerable<object> MethodName4()
        {
            object x = null;
            object y = null;

            if (x != null)
            {
                yield return x;
            }
            else
            {
                yield return y;
            }

            if (x != null)
                yield return x;
            else
                yield return y;

            if (x == null)
            {
                yield return y;
            }
            else
            {
                yield return x;
            }

            if (x == null)
                yield return y;
            else
                yield return x;
        }
    }
}
