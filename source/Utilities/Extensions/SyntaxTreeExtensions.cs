// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator
{
    public static class SyntaxTreeExtensions
    {
        public static bool IsMultiLineSpan(this SyntaxTree tree, TextSpan span)
        {
            if (tree == null)
                throw new ArgumentNullException(nameof(tree));

            return tree.GetLineSpan(span).IsMultiLine();
        }

        public static bool IsSingleLineSpan(this SyntaxTree tree, TextSpan span)
        {
            return !IsMultiLineSpan(tree, span);
        }
    }
}
