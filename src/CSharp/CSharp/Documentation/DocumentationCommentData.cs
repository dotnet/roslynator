// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Documentation
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal readonly struct DocumentationCommentData
    {
        private static readonly Regex _commentedEmptyLineRegex = new Regex(@"^///\s*(\r?\n|$)", RegexOptions.Multiline);

        internal DocumentationCommentData(string rawXml, DocumentationCommentOrigin origin)
        {
            RawXml = rawXml;
            Origin = origin;
        }

        public string RawXml { get; }

        public DocumentationCommentOrigin Origin { get; }

        public bool Success
        {
            get { return !string.IsNullOrEmpty(RawXml); }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return (Success) ? $"{Origin} {RawXml}" : "Uninitalized"; }
        }

        public SyntaxTrivia GetDocumentationCommentTrivia(SemanticModel semanticModel, int position)
        {
            string triviaText = AddSlashes(RawXml.TrimEnd());

            SyntaxTrivia trivia = ParseLeadingTrivia(triviaText).Single();

            var commentTrivia = (DocumentationCommentTriviaSyntax)trivia.GetStructure();

            var rewriter = new DocumentationCommentTriviaRewriter(position, semanticModel);

            // Remove T: from cref attribute and replace `1 with {T}
            commentTrivia = (DocumentationCommentTriviaSyntax)rewriter.VisitDocumentationCommentTrivia(commentTrivia);

            // Remove <filterpriority> element
            commentTrivia = RemoveFilterPriorityElement(commentTrivia);

            string text = commentTrivia.ToFullString();

            // Remove /// from empty lines
            text = _commentedEmptyLineRegex.Replace(text, "");

            return ParseLeadingTrivia(text).Single();
        }

        private static DocumentationCommentTriviaSyntax RemoveFilterPriorityElement(DocumentationCommentTriviaSyntax commentTrivia)
        {
            SyntaxList<XmlNodeSyntax> content = commentTrivia.Content;

            for (int i = content.Count - 1; i >= 0; i--)
            {
                XmlNodeSyntax xmlNode = content[i];

                if (xmlNode.IsKind(SyntaxKind.XmlElement))
                {
                    var xmlElement = (XmlElementSyntax)xmlNode;

                    if (xmlElement.IsLocalName("filterpriority", StringComparison.OrdinalIgnoreCase))
                        content = content.RemoveAt(i);
                }
            }

            return commentTrivia.WithContent(content);
        }

        private static string AddSlashes(string innerXml)
        {
            StringBuilder sb = StringBuilderCache.GetInstance();

            string indent = null;

            using (var sr = new StringReader(innerXml))
            {
                string s;

                while ((s = sr.ReadLine()) != null)
                {
                    if (s.Length > 0)
                    {
                        indent = indent ?? Regex.Match(s, "^ *").Value;

                        sb.Append("/// ");
                        s = Regex.Replace(s, $"^{indent}", "");

                        sb.AppendLine(s);
                    }
                }
            }

            return StringBuilderCache.GetStringAndFree(sb);
        }
    }
}
