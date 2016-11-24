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
                    && SyntaxAnalyzer.IsException(typeSymbol, semanticModel))
                {
                    MemberDeclarationSyntax member = GetContainingMember(throwStatement);

                    if (member != null)
                    {
                        SyntaxTrivia trivia = member.GetSingleLineDocumentationComment();

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

        public static MemberDeclarationSyntax GetContainingMember(this StatementSyntax statement)
        {
            foreach (SyntaxNode ancestor in statement.Ancestors())
            {
                switch (ancestor.Kind())
                {
                    case SyntaxKind.MethodDeclaration:
                    case SyntaxKind.OperatorDeclaration:
                    case SyntaxKind.ConversionOperatorDeclaration:
                    case SyntaxKind.ConstructorDeclaration:
                    case SyntaxKind.DestructorDeclaration:
                    case SyntaxKind.PropertyDeclaration:
                    case SyntaxKind.EventDeclaration:
                    case SyntaxKind.IndexerDeclaration:
                        return (MemberDeclarationSyntax)ancestor;
                    case SyntaxKind.IncompleteMember:
                    case SyntaxKind.SimpleLambdaExpression:
                    case SyntaxKind.ParenthesizedLambdaExpression:
                    case SyntaxKind.AnonymousMethodExpression:
                        return null;
                }
            }

            return null;
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