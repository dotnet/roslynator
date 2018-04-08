// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Documentation
{
    internal readonly struct DocumentationCommentInserter
    {
        private DocumentationCommentInserter(SyntaxTriviaList leadingTrivia, int index, string indent)
        {
            LeadingTrivia = leadingTrivia;
            Index = index;
            Indent = indent;
        }

        public static DocumentationCommentInserter Default { get; } = new DocumentationCommentInserter(default(SyntaxTriviaList), 0, "");

        public SyntaxTriviaList LeadingTrivia { get; }

        public int Index { get; }

        public string Indent { get; }

        public static DocumentationCommentInserter Create(MemberDeclarationSyntax memberDeclaration)
        {
            return Create(memberDeclaration.GetLeadingTrivia());
        }

        public static DocumentationCommentInserter Create(SyntaxTriviaList leadingTrivia)
        {
            if (leadingTrivia.Any())
            {
                int index = leadingTrivia.Count;

                while (index >= 1
                    && leadingTrivia[index - 1].IsWhitespaceTrivia())
                {
                    index--;
                }

                string indent = string.Concat(leadingTrivia.Skip(index));

                return new DocumentationCommentInserter(leadingTrivia, index, indent);
            }

            return Default;
        }

        public SyntaxTriviaList Insert(SyntaxTrivia comment, bool indent = false)
        {
            if (indent)
            {
                return IndentAndInsert(comment.ToFullString());
            }
            else
            {
                return LeadingTrivia.Insert(Index, comment);
            }
        }

        public SyntaxTriviaList InsertRange(SyntaxTriviaList comment, bool indent = false)
        {
            if (indent)
            {
                return IndentAndInsert(comment.ToFullString());
            }
            else
            {
                return LeadingTrivia.InsertRange(Index, comment);
            }
        }

        private SyntaxTriviaList IndentAndInsert(string commentText)
        {
            string text = Regex.Replace(commentText, @"^(?!\z)", Indent, RegexOptions.Multiline);

            SyntaxTriviaList trivia = SyntaxFactory.ParseLeadingTrivia(text);

            return LeadingTrivia.InsertRange(Index, trivia);
        }
    }
}
