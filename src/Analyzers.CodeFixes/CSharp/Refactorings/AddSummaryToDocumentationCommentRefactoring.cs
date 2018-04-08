// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddSummaryToDocumentationCommentRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            DocumentationCommentTriviaSyntax documentationComment,
            CancellationToken cancellationToken)
        {
            SyntaxList<XmlNodeSyntax> content = documentationComment.Content;

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            TextLine line = sourceText.Lines[documentationComment.GetFullSpanStartLine(cancellationToken)];

            string indent = StringUtility.GetLeadingWhitespaceExceptNewLine(line.ToString());

            TextChange textChange;

            if (content.Count == 1
                && content[0].IsKind(SyntaxKind.XmlText))
            {
                string text = content[0].ToString().Trim();

                string newText = CreateSummaryElement(indent, text);

                textChange = new TextChange(documentationComment.FullSpan, newText);
            }
            else
            {
                string newText = CreateSummaryElement(indent);

                textChange = new TextChange(new TextSpan(documentationComment.FullSpan.Start, 0), newText);
            }

            return await document.WithTextChangeAsync(textChange).ConfigureAwait(false);
        }

        private static string CreateSummaryElement(string indent, string text = null)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/// <summary>");
            sb.Append(indent);
            sb.Append("/// ");
            sb.AppendLine(text);
            sb.Append(indent);
            sb.AppendLine("/// </summary>");

            if (text == null)
                sb.Append(indent);

            return sb.ToString();
        }
    }
}