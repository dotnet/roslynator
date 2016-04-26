// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
#if DEBUG
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NewLineCodeFixProvider))]
    [Shared]
    public class NewLineCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.UseLinefeedAsNewLine,
                    DiagnosticIdentifiers.UseCarriageReturnAndLinefeedAsNewLine);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            SyntaxTrivia trivia = root.FindTrivia(context.Span.Start);

            if (trivia.IsKind(SyntaxKind.None))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case  DiagnosticIdentifiers.UseLinefeedAsNewLine:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Use linefeed as newline",
                                cancellationToken => ReplaceNewLineAsync(context.Document, trivia, context.Span, "\n", cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case  DiagnosticIdentifiers.UseCarriageReturnAndLinefeedAsNewLine:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Use carriage return + linefeed as newline",
                                cancellationToken => ReplaceNewLineAsync(context.Document, trivia, context.Span, "\r\n", cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }

        private static async Task<Document> ReplaceNewLineAsync(
            Document document,
            SyntaxTrivia trivia,
            TextSpan span,
            string newLine,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            switch (trivia.Kind())
            {
                case SyntaxKind.EndOfLineTrivia:
                    {
                        root = root.ReplaceTrivia(trivia, SyntaxFactory.EndOfLine(newLine));
                        break;
                    }
                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                    {
                        SyntaxToken token = root.FindToken(span.Start, findInsideTrivia: true);
                        root = root.ReplaceToken(token, SyntaxFactory.XmlTextNewLine(token.LeadingTrivia, newLine, newLine, token.TrailingTrivia));
                        break;
                    }
                case SyntaxKind.MultiLineCommentTrivia:
                    {
                        string s = trivia.ToString();

                        s = s.Substring(0, span.Start - trivia.SpanStart)
                            + newLine
                            + s.Substring(span.End - trivia.SpanStart, trivia.Span.End - span.End);

                        root = root.ReplaceTrivia(trivia, SyntaxFactory.SyntaxTrivia(trivia.Kind(), s));
                        break;
                    }
                default:
                    {
                        Debug.Assert(false, trivia.Kind().ToString());
                        break;
                    }
            }

            return document.WithSyntaxRoot(root);
        }
    }
#endif
}

