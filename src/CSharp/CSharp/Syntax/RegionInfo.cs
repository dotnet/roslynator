// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Syntax
{
    /// <summary>
    /// Provides information about a region.
    /// </summary>
    public readonly struct RegionInfo : IEquatable<RegionInfo>
    {
        private RegionInfo(RegionDirectiveTriviaSyntax directive, EndRegionDirectiveTriviaSyntax endDirective)
        {
            Directive = directive;
            EndDirective = endDirective;
        }

        private static RegionInfo Default { get; } = new RegionInfo();

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

            return Default;
        }

        internal static RegionInfo Create(RegionDirectiveTriviaSyntax regionDirective)
        {
            if (regionDirective == null)
                return Default;

            List<DirectiveTriviaSyntax> list = regionDirective.GetRelatedDirectives();

            if (list.Count != 2)
                return Default;

            if (list[1].Kind() != SyntaxKind.EndRegionDirectiveTrivia)
                return Default;

            return new RegionInfo(regionDirective, (EndRegionDirectiveTriviaSyntax)list[1]);
        }

        internal static RegionInfo Create(EndRegionDirectiveTriviaSyntax endRegionDirective)
        {
            if (endRegionDirective == null)
                return Default;

            List<DirectiveTriviaSyntax> list = endRegionDirective.GetRelatedDirectives();

            if (list.Count != 2)
                return Default;

            if (list[0].Kind() != SyntaxKind.RegionDirectiveTrivia)
                return Default;

            return new RegionInfo((RegionDirectiveTriviaSyntax)list[0], endRegionDirective);
        }

        /// <summary>
        /// Returns the string representation of the underlying syntax, not including its leading and trailing trivia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Directive?.ToString() ?? "";
        }

        /// <summary>
        /// Determines whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            return obj is RegionInfo other && Equals(other);
        }

        /// <summary>
        /// Determines whether this instance is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(RegionInfo other)
        {
            return EqualityComparer<RegionDirectiveTriviaSyntax>.Default.Equals(Directive, other.Directive);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return EqualityComparer<RegionDirectiveTriviaSyntax>.Default.GetHashCode(Directive);
        }

        public static bool operator ==(RegionInfo info1, RegionInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(RegionInfo info1, RegionInfo info2)
        {
            return !(info1 == info2);
        }
    }
}
