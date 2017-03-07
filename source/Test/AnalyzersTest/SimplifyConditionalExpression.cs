// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Test
{
    internal static class SimplifyConditionalExpression
    {
        public static void Foo()
        {
            bool f = false;
            bool x = false;

            x = f //a
               /*b*/ ? /*c*/ true //d
               /*e*/ : /*f*/ false; //g

            x = f
                ? true
                : false;

            x = f ? true : false;
            x = !f ? false : true;
            x = (f) ? true : false;
            x = !(f) ? false : true;

            x = f ? false : true;
            x = (f) ? false : true;

            x = f == f ? false : true;
            x = (f == f) ? false : true;

            x = f != f ? false : true;
            x = (f != f) ? false : true;


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
