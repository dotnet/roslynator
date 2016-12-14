// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddExceptionToDocumentationCommentRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, ThrowStatementSyntax throwStatement)
        {
            ExpressionSyntax expression = throwStatement.Expression;

            if (expression?.IsMissing == false
                && context.Span.IsContainedInSpanOrBetweenSpans(throwStatement))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

                if (typeSymbol?.IsErrorType() == false
                    && typeSymbol.IsException(semanticModel))
                {
                    MemberDeclarationSyntax containingMember = await GetContainingMemberAsync(throwStatement, semanticModel, context.CancellationToken).ConfigureAwait(false);

                    if (containingMember != null)
                    {
                        SyntaxTrivia trivia = containingMember.GetSingleLineDocumentationComment();

                        if (trivia.IsSingleLineDocumentationCommentTrivia())
                        {
                            var comment = trivia.GetStructure() as DocumentationCommentTriviaSyntax;

                            if (comment?.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) == true)
                            {
                                string exceptionName = typeSymbol
                                    .ToMinimalDisplayString(semanticModel, trivia.FullSpan.End)
                                    .Replace('<', '{')
                                    .Replace('>', '}');

                                if (!ContainsException(comment, typeSymbol, semanticModel, context.CancellationToken))
                                {
                                    context.RegisterRefactoring(
                                        "Add exception to documentation comment",
                                        cancellationToken => RefactorAsync(context.Document, trivia, exceptionName, cancellationToken));
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool ContainsException(DocumentationCommentTriviaSyntax comment, ITypeSymbol exceptionSymbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            foreach (XmlElementSyntax xmlElement in comment.Exceptions())
            {
                XmlElementStartTagSyntax startTag = xmlElement.StartTag;

                if (startTag != null)
                {
                    foreach (XmlAttributeSyntax xmlAttribute in startTag.Attributes)
                    {
                        if (xmlAttribute.IsKind(SyntaxKind.XmlCrefAttribute))
                        {
                            var xmlCrefAttribute = (XmlCrefAttributeSyntax)xmlAttribute;

                            CrefSyntax cref = xmlCrefAttribute.Cref;

                            if (cref != null)
                            {
                                ISymbol symbol = semanticModel.GetSymbolInfo(cref, cancellationToken).Symbol;

                                if (exceptionSymbol.Equals(symbol))
                                    return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        internal static async Task<MemberDeclarationSyntax> GetContainingMemberAsync(
            StatementSyntax statement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ISymbol symbol = semanticModel.GetEnclosingSymbol(statement.SpanStart, cancellationToken);

            if (symbol?.IsMethod() == true)
            {
                var methodsymbol = (IMethodSymbol)symbol;

                if (methodsymbol.MethodKind == MethodKind.Ordinary)
                {
                    if (methodsymbol.PartialImplementationPart != null)
                        symbol = methodsymbol.PartialImplementationPart;
                }
                else if (methodsymbol.AssociatedSymbol != null)
                {
                    symbol = methodsymbol.AssociatedSymbol;
                }
            }

            SyntaxNode node = await symbol
                .DeclaringSyntaxReferences[0]
                .GetSyntaxAsync(cancellationToken)
                .ConfigureAwait(false);

            return node as MemberDeclarationSyntax;
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            SyntaxTrivia trivia,
            string exceptionName,
            CancellationToken cancellationToken)
        {
            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            int lineIndex = trivia.GetSpanStartLine(cancellationToken);

            string newText = new string(' ', trivia.FullSpan.Start - sourceText.Lines[lineIndex].Start)
                + $"/// <exception cref=\"{exceptionName}\"></exception>"
                + Environment.NewLine;

            SourceText newSourceText = sourceText.WithChanges(
                new TextChange(new TextSpan(trivia.FullSpan.End, 0), newText));

            return document.WithText(newSourceText);
        }
    }
}