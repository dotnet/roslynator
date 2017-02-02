// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveImplementationFromAbstractMemberRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration)
                || methodDeclaration.Modifiers.Contains(SyntaxKind.AbstractKeyword))
            {
                CSharpSyntaxNode body = methodDeclaration.BodyOrExpressionBody();

                if (body != null)
                    ReportDiagnostic(context, methodDeclaration, body);
            }
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration)
                || propertyDeclaration.Modifiers.Contains(SyntaxKind.AbstractKeyword))
            {
                Analyze(context, propertyDeclaration, propertyDeclaration.AccessorList);
            }
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, IndexerDeclarationSyntax indexerDeclaration)
        {
            if (indexerDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration)
                || indexerDeclaration.Modifiers.Contains(SyntaxKind.AbstractKeyword))
            {
                Analyze(context, indexerDeclaration, indexerDeclaration.AccessorList);
            }
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, EventDeclarationSyntax eventDeclaration)
        {
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

        private static object GetName(SyntaxNode declaration)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        if (declaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return "interface method";
                        }
                        else
                        {
                            return "abstract method";
                        }
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        if (declaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return "interface property";
                        }
                        else
                        {
                            return "abstract property";
                        }
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        if (declaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return "interface indexer";
                        }
                        else
                        {
                            return "abstract indexer";
                        }
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        if (declaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return "interface event";
                        }
                        else
                        {
                            return "abstract event";
                        }
                    }
            }

            Debug.Assert(false, declaration.Kind().ToString());

            return "member";
        }

        public static async Task<Document> RefactorAsync(Document document, SyntaxNode node, CancellationToken cancellationToken)
        {
            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)node;

                        CSharpSyntaxNode body = methodDeclaration.BodyOrExpressionBody();

                        MethodDeclarationSyntax newMethodDeclaration = methodDeclaration.RemoveNode(body, SyntaxRemoveOptions.KeepUnbalancedDirectives);

                        newMethodDeclaration = newMethodDeclaration.WithSemicolonToken(SemicolonToken());

                        return await document.ReplaceNodeAsync(methodDeclaration, newMethodDeclaration, cancellationToken).ConfigureAwait(false);
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

                        CSharpSyntaxNode body = accessor.BodyOrExpressionBody();

                        AccessorDeclarationSyntax newAccessor = accessor.RemoveNode(body, SyntaxRemoveOptions.KeepUnbalancedDirectives);

                        newAccessor = newAccessor.WithSemicolonToken(SemicolonToken());

                        return await document.ReplaceNodeAsync(accessor, newAccessor, cancellationToken).ConfigureAwait(false);
                    }
            }

            Debug.Assert(false, "");

            return document;
        }
    }
}
