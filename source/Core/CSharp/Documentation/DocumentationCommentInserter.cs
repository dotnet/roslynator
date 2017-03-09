// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;

namespace Roslynator.CSharp.Documentation
{
    internal struct DocumentationCommentInserter
    {
        private DocumentationCommentInserter(SyntaxTriviaList leadingTrivia, int index, string indent)
        {
            LeadingTrivia = leadingTrivia;
            Index = index;
            Indent = indent;
        }

        public SyntaxTriviaList LeadingTrivia { get; }
        public int Index { get; }
        public string Indent { get; }

        public static DocumentationCommentInserter Create(MemberDeclarationSyntax memberDeclaration)
        {
            return Create(memberDeclaration.GetLeadingTrivia());
        }

        public static DocumentationCommentInserter Create(SyntaxTriviaList leadingTrivia)
        {
            int index = 0;

            string indent = "";

            if (leadingTrivia.Any())
            {
                index = leadingTrivia.Count - 1;

                for (int i = leadingTrivia.Count - 1; i >= 0; i--)
                {
                    if (leadingTrivia[i].IsWhitespaceTrivia())
                    {
                        index = i;
                    }
                    else
                    {
                        break;
                    }
                }

                indent = string.Concat(leadingTrivia.Skip(index));
            }

            return new DocumentationCommentInserter(leadingTrivia, index, indent);
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
