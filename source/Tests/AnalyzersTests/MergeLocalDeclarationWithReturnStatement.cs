// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

#pragma warning disable RCS1016, RCS1176, RCS1118, RCS1124

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class MergeLocalDeclarationWithReturnStatement
    {
        private static string FooReturn()
        {
            string i = "i";

            return i;
        }

        public static LambdaExpression FooReturn2()
        {
            Expression<Func<object, bool>> e = f => false;
            return e;
        }
    }
}
