// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable RCS1032, RCS1051, RCS1118, RCS1176

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class SimplifyConditionalExpression
    {
        public static void Foo()
        {
            bool x = false;

            bool f = false;
            bool f2 = false;

            x = f ? true : false;
            x = !f ? false : true;
            x = (f) ? true : false;
            x = !(f) ? false : true;
            x = ((f)) ? ((true)) : ((false));

            x = f ? false : true;
            x = (f) ? false : true;

            x = f == f2 ? false : true;
            x = (f == f2) ? false : true;

            x = f != f2 ? false : true;
            x = (f != f2) ? false : true;

            x = f //a
               /*b*/ ? /*c*/ true //d
               /*e*/ : /*f*/ false; //g

            x = f
                ? true
                : false;

            // n

            x = (f) ? true : true;
            x = (f) ? false : false;

            x = (f)
#if DEBUG
                ? true
                : false;
#else
                ? false
                : true;
#endif
        }
    }
}
