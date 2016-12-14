// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator
{
    public static class SyntaxTreeExtensions
    {
        public static int GetSpanStartLine(
            this SyntaxTree syntaxTree,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return syntaxTree.GetLineSpan(span, cancellationToken).StartLine();
        }

        public static int GetSpanEndLine(
            this SyntaxTree syntaxTree,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return syntaxTree.GetLineSpan(span, cancellationToken).EndLine();
        }

        public static bool IsMultiLineSpan(
            this SyntaxTree tree,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (tree == null)
                throw new ArgumentNullException(nameof(tree));

            return tree.GetLineSpan(span, cancellationToken).IsMultiLine();
        }

        public static bool IsSingleLineSpan(
            this SyntaxTree tree,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return !IsMultiLineSpan(tree, span, cancellationToken);
        }

        public static int GetLineCount(
            this SyntaxTree tree,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return tree.GetLineSpan(span, cancellationToken).GetLineCount();
        }
    }
}
