// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Text
{
    /// <summary>
    /// Represents selected lines in a <see cref="TextLineCollection"/>.
    /// </summary>
    public class TextLineCollectionSelection : Selection<TextLine>
    {
        private TextLineCollectionSelection(TextLineCollection lines, TextSpan span, SelectionResult result)
            : this(lines, span, result.FirstIndex, result.LastIndex)
        {
        }

        private TextLineCollectionSelection(TextLineCollection lines, TextSpan span, int firstIndex, int lastIndex)
            : base(span, firstIndex, lastIndex)
        {
            UnderlyingLines = lines;
        }

        /// <summary>
        /// Gets an underlying collection that contains selected lines.
        /// </summary>
        public TextLineCollection UnderlyingLines { get; }

        /// <summary>
        /// Gets an underlying collection that contains selected lines.
        /// </summary>
        protected override IReadOnlyList<TextLine> Items => UnderlyingLines;

        /// <summary>
        /// Creates a new <see cref="TextLineCollectionSelection"/> based on the specified list and span.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="span"></param>
        /// <returns></returns>
        public static TextLineCollectionSelection Create(TextLineCollection lines, TextSpan span)
        {
            if (lines == null)
                throw new ArgumentNullException(nameof(lines));

            SelectionResult result = SelectionResult.Create(lines, span);

            return new TextLineCollectionSelection(lines, span, result);
        }

        /// <summary>
        /// Creates a new <see cref="TextLineCollectionSelection"/> based on the specified list and span.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="span"></param>
        /// <param name="selectedLines"></param>
        /// <returns>True if the specified span contains at least one line; otherwise, false.</returns>
        public static bool TryCreate(TextLineCollection lines, TextSpan span, out TextLineCollectionSelection selectedLines)
        {
            selectedLines = Create(lines, span, 1, int.MaxValue);
            return selectedLines != null;
        }

        internal static bool TryCreate(TextLineCollection lines, TextSpan span, int minCount, out TextLineCollectionSelection selectedLines)
        {
            selectedLines = Create(lines, span, minCount, int.MaxValue);
            return selectedLines != null;
        }

        internal static bool TryCreate(TextLineCollection lines, TextSpan span, int minCount, int maxCount, out TextLineCollectionSelection selectedLines)
        {
            selectedLines = Create(lines, span, minCount, maxCount);
            return selectedLines != null;
        }

        private static TextLineCollectionSelection Create(TextLineCollection lines, TextSpan span, int minCount, int maxCount)
        {
            if (lines == null)
                return null;

            SelectionResult result = SelectionResult.Create(lines, span, minCount, maxCount);

            if (!result.Success)
                return null;

            return new TextLineCollectionSelection(lines, span, result);
        }
    }
}
