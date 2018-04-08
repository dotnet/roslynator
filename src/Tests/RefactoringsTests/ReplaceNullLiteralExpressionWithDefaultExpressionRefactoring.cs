// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal static class ReplaceNullLiteralExpressionWithDefaultExpressionRefactoring
    {
        private static class Foo
        {
            public static object Bar(StringBuilder stringBuilder = null)
            {
                Bar(null);

                return null;
            }
        }
    }
}
