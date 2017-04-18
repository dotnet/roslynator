// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveImplementationFromAbstractMemberRefactoring
    {
        public static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            if (methodDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration)
                || methodDeclaration.Modifiers.Contains(SyntaxKind.AbstractKeyword))
            {
                CSharpSyntaxNode body = methodDeclaration.BodyOrExpressionBody();

                if (body != null)
                    ReportDiagnostic(context, methodDeclaration, body);
            }
        }

        public static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

            if (propertyDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration)
                || propertyDeclaration.Modifiers.Contains(SyntaxKind.AbstractKeyword))
            {
                ArrowExpressionClauseSyntax expressionBody = propertyDeclaration.ExpressionBody;

                if (expressionBody != null)
                {
                    ReportDiagnostic(context, propertyDeclaration, expressionBody);
                }
                else
                {
                    Analyze(context, propertyDeclaration, propertyDeclaration.AccessorList);
                }
            }
        }

        public static void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            if (indexerDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration)
                || indexerDeclaration.Modifiers.Contains(SyntaxKind.AbstractKeyword))
            {
                ArrowExpressionClauseSyntax expressionBody = indexerDeclaration.ExpressionBody;

                if (expressionBody != null)
                {
                    ReportDiagnostic(context, indexerDeclaration, expressionBody);
                }
                else
                {
                    Analyze(context, indexerDeclaration, indexerDeclaration.AccessorList);
                }
            }
        }

        public static void AnalyzeEventDeclaration(SyntaxNodeAnalysisContext context)
        {
            var eventDeclaration = (EventDeclarationSyntax)context.Node;

            eventDeclaration = (EventDeclarationSyntax)context.Node;

            if (eventDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration)
                || eventDeclaration.Modifiers.Contains(SyntaxKind.AbstractKeyword))
            {
                AccessorListSyntax accessorList = eventDeclaration.AccessorList;

                if (accessorList != null)
                    ReportDiagnostic(context, eventDeclaration, accessorList);
            }
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, MemberDeclarationSyntax memberDeclaration, AccessorListSyntax accessorList)
        {
            if (accessorList != null)
            {
                foreach (AccessorDeclarationSyntax accessor in accessorList.Accessors)
                {
                    CSharpSyntaxNode body = accessor.BodyOrExpressionBody();

                    if (body != null)
                        ReportDiagnostic(context, memberDeclaration, body);
                }
            }
        }

        public static void ReportDiagnostic(SyntaxNodeAnalysisContext context, MemberDeclarationSyntax declaration, SyntaxNode node)
        {
            context.ReportDiagnostic(DiagnosticDescriptors.RemoveImplementationFromAbstractMember, node, GetName(declaration));
        }

        private static string GetName(SyntaxNode declaration)
        {
            return ((declaration.IsParentKind(SyntaxKind.InterfaceDeclaration)) ? "interface" : "abstract")
                + " "
                + declaration.GetTitle();
        }

        public static async Task<Document> RefactorAsync(Document document, SyntaxNode node, CancellationToken cancellationToken)
        {
            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)node;

                        MethodDeclarationSyntax newNode = methodDeclaration
                            .WithBody(null)
                            .WithExpressionBody(null)
                            .WithSemicolonToken(SemicolonToken());

                        return await document.ReplaceNodeAsync(methodDeclaration, newNode, cancellationToken).ConfigureAwait(false);
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var propertyDeclaration = (PropertyDeclarationSyntax)node;
                        ArrowExpressionClauseSyntax expressionBody = propertyDeclaration.ExpressionBody;

                        PropertyDeclarationSyntax newNode = propertyDeclaration
                            .WithExpressionBody(null)
                            .WithoutSemicolonToken()
                            .WithAccessorList(AccessorList(AutoGetAccessorDeclaration()).WithTriviaFrom(expressionBody));

                        return await document.ReplaceNodeAsync(propertyDeclaration, newNode, cancellationToken).ConfigureAwait(false);
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var indexerDeclaration = (IndexerDeclarationSyntax)node;
                        ArrowExpressionClauseSyntax expressionBody = indexerDeclaration.ExpressionBody;

                        IndexerDeclarationSyntax newNode = indexerDeclaration
                            .WithExpressionBody(null)
                            .WithoutSemicolonToken()
                            .WithAccessorList(AccessorList(AutoGetAccessorDeclaration()).WithTriviaFrom(expressionBody));

                        return await document.ReplaceNodeAsync(indexerDeclaration, newNode, cancellationToken).ConfigureAwait(false);
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        var eventDeclaration = (EventDeclarationSyntax)node;

                        EventFieldDeclarationSyntax eventFieldDeclaration = EventFieldDeclaration(
                            eventDeclaration.AttributeLists,
                            eventDeclaration.Modifiers,
                            eventDeclaration.EventKeyword,
                            VariableDeclaration(eventDeclaration.Type, VariableDeclarator(eventDeclaration.Identifier)),
                            SemicolonToken());

                        return await document.ReplaceNodeAsync(eventDeclaration, eventFieldDeclaration, cancellationToken).ConfigureAwait(false);
                    }
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                    {
                        var accessor = (AccessorDeclarationSyntax)node;

                        AccessorDeclarationSyntax newAccessor = accessor
                            .WithBody(null)
                            .WithExpressionBody(null)
                            .WithSemicolonToken(SemicolonToken());

                        return await document.ReplaceNodeAsync(accessor, newAccessor, cancellationToken).ConfigureAwait(false);
                    }
            }

            Debug.Assert(false, "");

            return document;
        }
    }
}
