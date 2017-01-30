// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
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
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatAccessorListRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, AccessorListSyntax accessorList)
        {
            SyntaxList<AccessorDeclarationSyntax> accessors = accessorList.Accessors;

            if (accessors.Any(f => f.Body != null))
            {
                if (accessorList.IsSingleLine(includeExteriorTrivia: false))
                {
                    ReportDiagnostic(context, accessorList);
                }
                else
                {
                    foreach (AccessorDeclarationSyntax accessor in accessors)
                    {
                        if (ShouldBeFormatted(accessor))
                            ReportDiagnostic(context, accessor);
                    }
                }
            }
            else
            {
                SyntaxNode parent = accessorList.Parent;

                switch (parent?.Kind())
                {
                    case SyntaxKind.PropertyDeclaration:
                        {
                            if (accessors.All(f => !f.AttributeLists.Any())
                                && !accessorList.IsSingleLine(includeExteriorTrivia: false))
                            {
                                var propertyDeclaration = (PropertyDeclarationSyntax)parent;
                                SyntaxToken identifier = propertyDeclaration.Identifier;

                                if (!identifier.IsMissing)
                                {
                                    SyntaxToken closeBrace = accessorList.CloseBraceToken;

                                    if (!closeBrace.IsMissing)
                                    {
                                        TextSpan span = TextSpan.FromBounds(identifier.Span.End, closeBrace.Span.Start);

                                        if (propertyDeclaration
                                            .DescendantTrivia(span)
                                            .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                                        {
                                            ReportDiagnostic(context, accessorList);
                                        }
                                    }
                                }
                            }

                            break;
                        }
                    case SyntaxKind.IndexerDeclaration:
                        {
                            if (accessors.All(f => !f.AttributeLists.Any())
                                && !accessorList.IsSingleLine(includeExteriorTrivia: false))
                            {
                                var indexerDeclaration = (IndexerDeclarationSyntax)parent;

                                BracketedParameterListSyntax parameterList = indexerDeclaration.ParameterList;

                                if (parameterList != null)
                                {
                                    SyntaxToken closeBracket = parameterList.CloseBracketToken;

                                    if (!closeBracket.IsMissing)
                                    {
                                        SyntaxToken closeBrace = accessorList.CloseBraceToken;

                                        if (!closeBrace.IsMissing)
                                        {
                                            TextSpan span = TextSpan.FromBounds(closeBracket.Span.End, closeBrace.Span.Start);

                                            if (indexerDeclaration
                                                .DescendantTrivia(span)
                                                .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                                            {
                                                ReportDiagnostic(context, accessorList);
                                            }
                                        }
                                    }
                                }
                            }

                            break;
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

            return false;
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            context.ReportDiagnostic(DiagnosticDescriptors.FormatAccessorList, node);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            AccessorListSyntax accessorList,
            CancellationToken cancellationToken)
        {
            if (accessorList.Accessors.All(f => f.Body == null))
            {
                SyntaxNode parent = accessorList.Parent;

                switch (parent.Kind())
                {
                    case SyntaxKind.PropertyDeclaration:
                        {
                            var propertyDeclaration = (PropertyDeclarationSyntax)parent;

                            TextSpan span = TextSpan.FromBounds(
                                propertyDeclaration.Identifier.Span.End,
                                accessorList.CloseBraceToken.Span.Start);

                            PropertyDeclarationSyntax newNode = Remover.RemoveWhitespaceOrEndOfLine(propertyDeclaration, span);

                            newNode = newNode.WithFormatterAnnotation();

                            return await document.ReplaceNodeAsync(propertyDeclaration, newNode, cancellationToken).ConfigureAwait(false);
                        }
                    case SyntaxKind.IndexerDeclaration:
                        {
                            var indexerDeclaration = (IndexerDeclarationSyntax)parent;

                            TextSpan span = TextSpan.FromBounds(
                                indexerDeclaration.ParameterList.CloseBracketToken.Span.End,
                                accessorList.CloseBraceToken.Span.Start);

                            IndexerDeclarationSyntax newNode = Remover.RemoveWhitespaceOrEndOfLine(indexerDeclaration, span);

                            newNode = newNode.WithFormatterAnnotation();

                            return await document.ReplaceNodeAsync(indexerDeclaration, newNode, cancellationToken).ConfigureAwait(false);
                        }
                    default:
                        {
                            Debug.Assert(false, parent.Kind().ToString());
                            return document;
                        }
                }
            }
            else
            {
                AccessorListSyntax newAccessorList = GetNewAccessorList(accessorList)
                    .WithFormatterAnnotation();

                return await document.ReplaceNodeAsync(accessorList, newAccessorList, cancellationToken).ConfigureAwait(false);
            }
        }

        private static AccessorListSyntax GetNewAccessorList(AccessorListSyntax accessorList)
        {
            if (accessorList.IsSingleLine(includeExteriorTrivia: false))
            {
                SyntaxTriviaList triviaList = accessorList.CloseBraceToken.LeadingTrivia
                    .Add(NewLineTrivia());

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
