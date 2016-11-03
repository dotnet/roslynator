// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class SimplifyLogicalNotExpression
    {
        public static void MethodName()
        {
            bool f = !true; //false

            f = /*1*/!/*2*/
                     /*3*/true; //false

            f = !false; //true
            f = !!false; //false
            f = !!true; //true

            f = /*1*/!/*2*/
                     /*3*/!/*4*/
                          /*5*/true; //true
        }
    }
}
