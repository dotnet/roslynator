// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Syntax
{
    /// <summary>
    /// Provides information about a region.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct RegionInfo
    {
        private RegionInfo(RegionDirectiveTriviaSyntax directive, EndRegionDirectiveTriviaSyntax endDirective)
        {
            Directive = directive;
            EndDirective = endDirective;
        }

        /// <summary>
        /// #region directive.
        /// </summary>
        public RegionDirectiveTriviaSyntax Directive { get; }

        /// <summary>
        /// #endregion directive.
        /// </summary>
        public EndRegionDirectiveTriviaSyntax EndDirective { get; }

        /// <summary>
        /// Determines whether this struct was initialized with an actual syntax.
        /// </summary>
        public bool Success
        {
            get { return Directive != null; }
        }

        /// <summary>
        /// The absolute span of this region, not including its leading and trailing trivia.
        /// </summary>
        public TextSpan Span
        {
            get
            {
                return (Success)
                    ? TextSpan.FromBounds(Directive.SpanStart, EndDirective.Span.End)
                    : default(TextSpan);
            }
        }

        /// <summary>
        /// The absolute span of this region, including its leading and trailing trivia.
        /// </summary>
        public TextSpan FullSpan
        {
            get
            {
                return (Success)
                    ? TextSpan.FromBounds(Directive.FullSpan.Start, EndDirective.FullSpan.End)
                    : default(TextSpan);
            }
        }

        /// <summary>
        /// Determines whether this region is empty, i.e. contains only white-space.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                if (!Success)
                    return false;

                SyntaxTrivia trivia = Directive.ParentTrivia;

                return trivia.TryGetContainingList(out SyntaxTriviaList list)
                    && object.ReferenceEquals(EndDirective, FindEndRegionDirective(list, list.IndexOf(trivia)));
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return SyntaxInfoHelpers.ToDebugString(Success, this, Directive); }
        }

        private static EndRegionDirectiveTriviaSyntax FindEndRegionDirective(SyntaxTriviaList list, int index)
        {
            for (int i = index + 1; i < list.Count; i++)
            {
                SyntaxTrivia trivia = list[i];

                switch (trivia.Kind())
                {
                    case SyntaxKind.WhitespaceTrivia:
                    case SyntaxKind.EndOfLineTrivia:
                        {
                            continue;
                        }
                    case SyntaxKind.EndRegionDirectiveTrivia:
                        {
                            if (trivia.HasStructure)
                                return (EndRegionDirectiveTriviaSyntax)trivia.GetStructure();

                            return null;
                        }
                    default:
                        {
                            return null;
                        }
                }
            }

            return null;
        }

        internal static RegionInfo Create(SyntaxNode node)
        {
            switch (node?.Kind())
            {
                case SyntaxKind.RegionDirectiveTrivia:
                    return Create((RegionDirectiveTriviaSyntax)node);
                case SyntaxKind.EndRegionDirectiveTrivia:
                    return Create((EndRegionDirectiveTriviaSyntax)node);
            }

            return default;
        }

        internal static RegionInfo Create(RegionDirectiveTriviaSyntax regionDirective)
        {
            if (regionDirective == null)
                return default;

            List<DirectiveTriviaSyntax> list = regionDirective.GetRelatedDirectives();

            if (list.Count != 2)
                return default;

            if (list[1].Kind() != SyntaxKind.EndRegionDirectiveTrivia)
                return default;

            return new RegionInfo(regionDirective, (EndRegionDirectiveTriviaSyntax)list[1]);
        }

        internal static RegionInfo Create(EndRegionDirectiveTriviaSyntax endRegionDirective)
        {
            if (endRegionDirective == null)
                return default;

            List<DirectiveTriviaSyntax> list = endRegionDirective.GetRelatedDirectives();

            if (list.Count != 2)
                return default;

            if (list[0].Kind() != SyntaxKind.RegionDirectiveTrivia)
                return default;

            return new RegionInfo((RegionDirectiveTriviaSyntax)list[0], endRegionDirective);
        }
    }
}
