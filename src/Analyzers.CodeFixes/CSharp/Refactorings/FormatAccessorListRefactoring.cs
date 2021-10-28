// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatAccessorListRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            AccessorListSyntax accessorList,
            CancellationToken cancellationToken)
        {
            if (accessorList.Accessors.All(f => f.BodyOrExpressionBody() == null))
            {
                SyntaxNode parent = accessorList.Parent;

                switch (parent)
                {
                    case PropertyDeclarationSyntax propertyDeclaration:
                        {
                            TextSpan span = TextSpan.FromBounds(
                                propertyDeclaration.Identifier.Span.End,
                                accessorList.CloseBraceToken.SpanStart);

                            PropertyDeclarationSyntax newNode = propertyDeclaration.RemoveWhitespace(span);

                            newNode = newNode.WithFormatterAnnotation();

                            return await document.ReplaceNodeAsync(propertyDeclaration, newNode, cancellationToken).ConfigureAwait(false);
                        }
                    case IndexerDeclarationSyntax indexerDeclaration:
                        {
                            TextSpan span = TextSpan.FromBounds(
                                indexerDeclaration.ParameterList.CloseBracketToken.Span.End,
                                accessorList.CloseBraceToken.SpanStart);

                            IndexerDeclarationSyntax newNode = indexerDeclaration.RemoveWhitespace(span);

                            newNode = newNode.WithFormatterAnnotation();

                            return await document.ReplaceNodeAsync(indexerDeclaration, newNode, cancellationToken).ConfigureAwait(false);
                        }
                    default:
                        {
                            SyntaxDebug.Fail(parent);
                            return document;
                        }
                }
            }
            else
            {
                AccessorListSyntax newAccessorList = GetNewAccessorList(accessorList);

                newAccessorList = AddNewLineAfterFirstAccessorIfNecessary(accessorList, newAccessorList, cancellationToken);

                newAccessorList = newAccessorList.WithFormatterAnnotation();

                return await document.ReplaceNodeAsync(accessorList, newAccessorList, cancellationToken).ConfigureAwait(false);
            }
        }

        private static AccessorListSyntax AddNewLineAfterFirstAccessorIfNecessary(AccessorListSyntax accessorList, AccessorListSyntax newAccessorList, CancellationToken cancellationToken)
        {
            SyntaxList<AccessorDeclarationSyntax> accessors = newAccessorList.Accessors;

            if (accessors.Count > 1)
            {
                AccessorDeclarationSyntax accessor = accessors[0];

                SyntaxTriviaList trailingTrivia = accessor.GetTrailingTrivia();

                if (accessorList.SyntaxTree.IsSingleLineSpan(trailingTrivia.Span, cancellationToken))
                    return newAccessorList.ReplaceNode(accessor, accessor.AppendToTrailingTrivia(NewLine()));
            }

            return newAccessorList;
        }

        private static AccessorListSyntax GetNewAccessorList(AccessorListSyntax accessorList)
        {
            if (accessorList.IsSingleLine(includeExteriorTrivia: false))
            {
                SyntaxTriviaList triviaList = accessorList
                    .CloseBraceToken
                    .LeadingTrivia
                    .Add(NewLine());

                return accessorList
                    .RemoveWhitespace()
                    .WithCloseBraceToken(accessorList.CloseBraceToken.WithLeadingTrivia(triviaList));
            }
            else
            {
                return accessorList.ReplaceNodes(
                    accessorList.Accessors,
                    (f, g) =>
                    {
                        if (ShouldBeFormatted(f))
                        {
                            return f.RemoveWhitespace(f.Span);
                        }
                        else
                        {
                            return g;
                        }
                    });
            }
        }

        private static bool ShouldBeFormatted(AccessorDeclarationSyntax accessor)
        {
            BlockSyntax body = accessor.Body;

            if (body != null)
            {
                SyntaxList<StatementSyntax> statements = body.Statements;

                if (statements.Count <= 1
                    && accessor.SyntaxTree.IsMultiLineSpan(TextSpan.FromBounds(accessor.Keyword.SpanStart, accessor.Span.End))
                    && (!statements.Any() || statements[0].IsSingleLine()))
                {
                    return accessor
                        .DescendantTrivia(accessor.Span, descendIntoTrivia: true)
                        .All(f => f.IsWhitespaceOrEndOfLineTrivia());
                }
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = accessor.ExpressionBody;

                if (expressionBody != null
                    && accessor.SyntaxTree.IsMultiLineSpan(TextSpan.FromBounds(accessor.Keyword.SpanStart, accessor.Span.End))
                    && expressionBody.Expression?.IsSingleLine() == true)
                {
                    return accessor
                        .DescendantTrivia(accessor.Span, descendIntoTrivia: true)
                        .All(f => f.IsWhitespaceOrEndOfLineTrivia());
                }
            }

            return false;
        }
    }
}
