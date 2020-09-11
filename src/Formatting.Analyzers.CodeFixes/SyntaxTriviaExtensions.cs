// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Roslynator.Formatting.CSharp;

namespace Roslynator.CSharp
{
    internal static class SyntaxTriviaExtensions
    {
        public static TNode PrependEndOfLineToLeadingTrivia<TNode>(this TNode node) where TNode : SyntaxNode
        {
            SyntaxTrivia endOfLine = SyntaxTriviaAnalysis.DetermineEndOfLine(node);

            return node.PrependToLeadingTrivia(endOfLine);
        }

        public static TNode AppendEndOfLineToLeadingTrivia<TNode>(this TNode node) where TNode : SyntaxNode
        {
            SyntaxTrivia endOfLine = SyntaxTriviaAnalysis.DetermineEndOfLine(node);

            return node.AppendToLeadingTrivia(endOfLine);
        }

        public static TNode PrependEndOfLineToTrailingTrivia<TNode>(this TNode node) where TNode : SyntaxNode
        {
            SyntaxTrivia endOfLine = SyntaxTriviaAnalysis.DetermineEndOfLine(node);

            return node.PrependToTrailingTrivia(endOfLine);
        }

        public static TNode AppendEndOfLineToTrailingTrivia<TNode>(this TNode node) where TNode : SyntaxNode
        {
            SyntaxTrivia endOfLine = SyntaxTriviaAnalysis.DetermineEndOfLine(node);

            return node.AppendToTrailingTrivia(endOfLine);
        }

        public static SyntaxToken PrependEndOfLineToLeadingTrivia(this SyntaxToken token)
        {
            SyntaxTrivia endOfLine = SyntaxTriviaAnalysis.DetermineEndOfLine(token);

            return token.PrependToLeadingTrivia(endOfLine);
        }

        public static SyntaxToken AppendEndOfLineToLeadingTrivia(this SyntaxToken token)
        {
            SyntaxTrivia endOfLine = SyntaxTriviaAnalysis.DetermineEndOfLine(token);

            return token.AppendToLeadingTrivia(endOfLine);
        }

        public static SyntaxToken PrependEndOfLineToTrailingTrivia(this SyntaxToken token)
        {
            SyntaxTrivia endOfLine = SyntaxTriviaAnalysis.DetermineEndOfLine(token);

            return token.PrependToTrailingTrivia(endOfLine);
        }

        public static SyntaxToken AppendEndOfLineToTrailingTrivia(this SyntaxToken token)
        {
            SyntaxTrivia endOfLine = SyntaxTriviaAnalysis.DetermineEndOfLine(token);

            return token.AppendToTrailingTrivia(endOfLine);
        }
    }
}

