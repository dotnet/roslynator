// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable RCS1104, RCS1118, RCS1176

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class FormatConditionalExpressionOperatorsOnNextLine
    {
        public static void Foo()
        {
            bool f = false;

            bool x = false;
            bool y = false;

            f = (f) ?
                x :
                y;

            f = (f)?
                x:
                y;

            f = (f) ?    
                x :    
                y;

            f = (f) ?
                x
                : y;

            f = (f)
                ? x :
                y;
        }
    }
}
