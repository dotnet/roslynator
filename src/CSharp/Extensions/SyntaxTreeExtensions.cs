// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator
{
    /// <summary>
    /// A set of extension methods for <see cref="SyntaxTree"/>.
    /// </summary>
    public static class SyntaxTreeExtensions
    {
        /// <summary>
        /// Returns zero-based index of the start line of the specified span.
        /// </summary>
        /// <param name="syntaxTree"></param>
        /// <param name="span"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static int GetStartLine(
            this SyntaxTree syntaxTree,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (syntaxTree == null)
                throw new ArgumentNullException(nameof(syntaxTree));

            return syntaxTree.GetLineSpan(span, cancellationToken).StartLine();
        }

        /// <summary>
        /// Returns zero-based index of the end line of the specified span.
        /// </summary>
        /// <param name="syntaxTree"></param>
        /// <param name="span"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static int GetEndLine(
            this SyntaxTree syntaxTree,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (syntaxTree == null)
                throw new ArgumentNullException(nameof(syntaxTree));

            return syntaxTree.GetLineSpan(span, cancellationToken).EndLine();
        }

        /// <summary>
        /// Returns true if the specified <see cref="TextSpan"/> spans over multiple lines.
        /// </summary>
        /// <param name="syntaxTree"></param>
        /// <param name="span"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static bool IsMultiLineSpan(
            this SyntaxTree syntaxTree,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (syntaxTree == null)
                throw new ArgumentNullException(nameof(syntaxTree));

            return syntaxTree.GetLineSpan(span, cancellationToken).IsMultiLine();
        }

        /// <summary>
        /// Returns true if the specified <see cref="TextSpan"/> does not span over multiple lines.
        /// </summary>
        /// <param name="syntaxTree"></param>
        /// <param name="span"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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
