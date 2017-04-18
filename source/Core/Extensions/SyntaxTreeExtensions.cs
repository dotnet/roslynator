// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator
{
    public static class SyntaxTreeExtensions
    {
        public static int GetStartLine(
            this SyntaxTree syntaxTree,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (syntaxTree == null)
                throw new ArgumentNullException(nameof(syntaxTree));

            return syntaxTree.GetLineSpan(span, cancellationToken).StartLine();
        }

        public static int GetEndLine(
            this SyntaxTree syntaxTree,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (syntaxTree == null)
                throw new ArgumentNullException(nameof(syntaxTree));

            return syntaxTree.GetLineSpan(span, cancellationToken).EndLine();
        }

        public static bool IsMultiLineSpan(
            this SyntaxTree syntaxTree,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (syntaxTree == null)
                throw new ArgumentNullException(nameof(syntaxTree));

            return syntaxTree.GetLineSpan(span, cancellationToken).IsMultiLine();
        }

        public static bool IsSingleLineSpan(
            this SyntaxTree syntaxTree,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (syntaxTree == null)
                throw new ArgumentNullException(nameof(syntaxTree));

            return syntaxTree.GetLineSpan(span, cancellationToken).IsSingleLine();
        }

        internal static int GetLineCount(
            this SyntaxTree syntaxTree,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (syntaxTree == null)
                throw new ArgumentNullException(nameof(syntaxTree));

            return syntaxTree.GetLineSpan(span, cancellationToken).GetLineCount();
        }
    }
}
