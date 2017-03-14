// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Test
{
#pragma warning disable RCS1002, RCS1016
    public static class UseCoalesceExpression
    {
        private static void IfStatement()
        {
            string x = null;
            string y = null;

            // a
            if (x == null)
            {
                // b
                x = (true) ? "" : "";
            }

            if (y == null)
                y = "";
        }

        private static void LocalDeclarationStatement()
        {
            string x = GetValueOrDefault();

            if (x == null)
            {
                x = (true) ? "" : "";
            }

            string y = GetValueOrDefault();

            if (y == null)
                y = "";
        }

        private static void ExpressionStatement()
        {
            string x = null;
            string y = null;

            x = GetValueOrDefault();

            if (x == null)
            {
                x = (true) ? "" : "";
            }

            y = GetValueOrDefault();

            // ...
            if (y == null)
                y = "";
        }

        private static void SingleIfStatement(string value)
        {
            if (value == null)
            {
                value = (true) ? "" : "";
            }
        }

        private static void SingleIfStatement2(string value)
        {
            if (value == null)
                value = "";
        }

        private static string GetValueOrDefault()
        {
            return null;
        }
    }
}
