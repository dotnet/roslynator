// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class RemoveRedundantParentheses
    {
        [Obsolete((""))]
        public static void MethodName(string value)
        {
            bool f = /*1*/(/*2*/
                           /*3*/false/*4*/
                                     /*5*/)/*6*/;

            while ((true))
            {
            }

            do
            {
            } while ((true));

            using ((true))
            {
            }

            lock ((true))
            {
            }

            if ((true))
            {
            }

            switch ((true))
            {
            }

            return (true);

            yield return (true);

            (true);

            MethodName((""));

            (f) = (false);
            (f) += (false);
            (f) -= (false);
            (f) *= (false);
            (f) /= (false);
            (f) %= (false);
            (f) &= (false);
            (f) ^= (false);
            (f) |= (false);
            (f) <<= (false);
            (f) >>= (false);
        }
    }
}
