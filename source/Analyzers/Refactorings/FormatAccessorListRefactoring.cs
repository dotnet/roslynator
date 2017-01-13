// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatAccessorListRefactoring
    {
        private static DiagnosticDescriptor DiagnosticDescriptor
        {
            get { return DiagnosticDescriptors.FormatAccessorList; }
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, AccessorListSyntax accessorList)
        {
            SyntaxList<AccessorDeclarationSyntax> accessors = accessorList.Accessors;

            if (accessors.Any(f => f.BodyOrExpressionBody() != null))
            {
                if (accessorList.IsSingleLine(includeExteriorTrivia: false))
                {
                    context.ReportDiagnostic(DiagnosticDescriptor, accessorList.GetLocation());
                }
                else
                {
                    foreach (AccessorDeclarationSyntax accessor in accessors)
                    {
                        if (ShouldBeFormatted(accessor))
                            context.ReportDiagnostic(DiagnosticDescriptor, accessor.GetLocation());
                    }
                }
            }
            else if (accessorList.IsParentKind(SyntaxKind.PropertyDeclaration)
                && accessors.All(f => !f.AttributeLists.Any())
                && !accessorList.IsSingleLine(includeExteriorTrivia: false))
            {
                var propertyDeclaration = (PropertyDeclarationSyntax)accessorList.Parent;

                if (!propertyDeclaration.Identifier.IsMissing
                    && !accessorList.CloseBraceToken.IsMissing)
                {
                    TextSpan span = TextSpan.FromBounds(
                        propertyDeclaration.Identifier.Span.End,
                        accessorList.CloseBraceToken.Span.Start);

                    if (propertyDeclaration
                        .DescendantTrivia(span)
                        .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                    {
                        context.ReportDiagnostic(DiagnosticDescriptor, accessorList.GetLocation());
                    }
                }
            }
        }

        private static bool ShouldBeFormatted(AccessorDeclarationSyntax accessor)
        {
            BlockSyntax body = accessor.Body;

            if (body != null)
            {
                SyntaxList<StatementSyntax> statements = body.Statements;

                if (statements.Count <= 1
                    && accessor.SyntaxTree.IsMultiLineSpan(TextSpan.FromBounds(accessor.Keyword.Span.Start, accessor.Span.End))
                    && (statements.Count == 0 || statements[0].IsSingleLine()))
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
                    && accessor.SyntaxTree.IsMultiLineSpan(TextSpan.FromBounds(accessor.Keyword.Span.Start, accessor.Span.End))
                    && expressionBody.Expression?.IsSingleLine() == true)
                {
                    return accessor
                       .DescendantTrivia(accessor.Span, descendIntoTrivia: true)
                       .All(f => f.IsWhitespaceOrEndOfLineTrivia());
                }
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            AccessorListSyntax accessorList,
            CancellationToken cancellationToken)
        {
            if (accessorList.Accessors.All(f => f.BodyOrExpressionBody() == null))
            {
                var propertyDeclaration = (PropertyDeclarationSyntax)accessorList.Parent;

                TextSpan span = TextSpan.FromBounds(
                    propertyDeclaration.Identifier.Span.End,
                    accessorList.CloseBraceToken.Span.Start);

                PropertyDeclarationSyntax newPropertyDeclaration = Remover.RemoveWhitespaceOrEndOfLine(propertyDeclaration, span)
                    .WithFormatterAnnotation();

                return await document.ReplaceNodeAsync(propertyDeclaration, newPropertyDeclaration, cancellationToken).ConfigureAwait(false);
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
                AccessorDeclarationSyntax accessor = accessors.First();

                SyntaxTriviaList trailingTrivia = accessor.GetTrailingTrivia();

                if (accessorList.SyntaxTree.IsSingleLineSpan(trailingTrivia.Span, cancellationToken))
                    return newAccessorList.ReplaceNode(accessor, accessor.AppendTrailingTrivia(CSharpFactory.NewLineTrivia()));
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
                    .Add(CSharpFactory.NewLineTrivia());

                return Remover.RemoveWhitespaceOrEndOfLine(accessorList)
                    .WithCloseBraceToken(accessorList.CloseBraceToken.WithLeadingTrivia(triviaList));
            }
            else
            {
                return accessorList.ReplaceNodes(accessorList.Accessors, (f, g) =>
                {
                    if (ShouldBeFormatted(f))
                    {
                        return Remover.RemoveWhitespaceOrEndOfLine(f, f.Span);
                    }
                    else
                    {
                        return g;
                    }
                });
            }
        }
    }
}
