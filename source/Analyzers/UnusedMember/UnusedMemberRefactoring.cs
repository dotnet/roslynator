// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analyzers.UnusedMember
{
    internal static class UnusedMemberRefactoring
    {
        public static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            AnalyzeTypeDeclaration(context, (TypeDeclarationSyntax)context.Node);
        }

        public static void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
        {
            AnalyzeTypeDeclaration(context, (TypeDeclarationSyntax)context.Node);
        }

        private static void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDeclaration)
        {
            if (typeDeclaration.Modifiers.Contains(SyntaxKind.PartialKeyword))
                return;

            SyntaxList<MemberDeclarationSyntax> members = typeDeclaration.Members;

            UnusedMemberWalker walker = null;

            foreach (MemberDeclarationSyntax member in members)
            {
                if (member.ContainsDiagnostics)
                    continue;

                if (member.ContainsDirectives)
                    continue;

                switch (member.Kind())
                {
                    case SyntaxKind.DelegateDeclaration:
                        {
                            var declaration = (DelegateDeclarationSyntax)member;

                            if (IsPrivate(declaration, declaration.Modifiers))
                            {
                                if (walker == null)
                                    walker = UnusedMemberWalkerCache.Acquire(context.SemanticModel, context.CancellationToken);

                                walker.AddNode(declaration.Identifier.ValueText, declaration);
                            }

                            break;
                        }
                    case SyntaxKind.EventDeclaration:
                        {
                            var declaration = (EventDeclarationSyntax)member;

                            if (IsPrivate(declaration, declaration.Modifiers))
                            {
                                if (walker == null)
                                    walker = UnusedMemberWalkerCache.Acquire(context.SemanticModel, context.CancellationToken);

                                walker.AddNode(declaration.Identifier.ValueText, declaration);
                            }

                            break;
                        }
                    case SyntaxKind.EventFieldDeclaration:
                        {
                            var declaration = (EventFieldDeclarationSyntax)member;

                            if (IsPrivate(declaration, declaration.Modifiers))
                            {
                                if (walker == null)
                                    walker = UnusedMemberWalkerCache.Acquire(context.SemanticModel, context.CancellationToken);

                                walker.AddNodes(declaration.Declaration);
                            }

                            break;
                        }
                    case SyntaxKind.FieldDeclaration:
                        {
                            var declaration = (FieldDeclarationSyntax)member;
                            SyntaxTokenList modifiers = declaration.Modifiers;

                            if (IsPrivate(declaration, modifiers))
                            {
                                if (walker == null)
                                    walker = UnusedMemberWalkerCache.Acquire(context.SemanticModel, context.CancellationToken);

                                walker.AddNodes(declaration.Declaration, isConst: modifiers.Contains(SyntaxKind.ConstKeyword));
                            }

                            break;
                        }
                    case SyntaxKind.MethodDeclaration:
                        {
                            var declaration = (MethodDeclarationSyntax)member;

                            SyntaxTokenList modifiers = declaration.Modifiers;

                            if (IsPrivate(declaration, modifiers))
                            {
                                string methodName = declaration.Identifier.ValueText;

                                if (!IsMainMethod(declaration, modifiers, methodName))
                                {
                                    if (walker == null)
                                        walker = UnusedMemberWalkerCache.Acquire(context.SemanticModel, context.CancellationToken);

                                    walker.AddNode(methodName, declaration);
                                }
                            }

                            break;
                        }
                    case SyntaxKind.PropertyDeclaration:
                        {
                            var declaration = (PropertyDeclarationSyntax)member;

                            if (IsPrivate(declaration, declaration.Modifiers))
                            {
                                if (walker == null)
                                    walker = UnusedMemberWalkerCache.Acquire(context.SemanticModel, context.CancellationToken);

                                walker.AddNode(declaration.Identifier.ValueText, declaration);
                            }

                            break;
                        }
                }
            }

            if (walker == null)
                return;

            walker.Visit(typeDeclaration);

            foreach (NodeSymbolInfo info in UnusedMemberWalkerCache.GetNodesAndRelease(walker))
            {
                SyntaxNode node = info.Node;

                if (node is VariableDeclaratorSyntax variableDeclarator)
                {
                    var variableDeclaration = (VariableDeclarationSyntax)variableDeclarator.Parent;

                    if (variableDeclaration.Variables.Count == 1)
                    {
                        ReportDiagnostic(context, variableDeclaration.Parent, variableDeclaration.Parent.GetTitle());
                    }
                    else
                    {
                        ReportDiagnostic(context, variableDeclarator, variableDeclaration.Parent.GetTitle());
                    }
                }
                else
                {
                    ReportDiagnostic(context, node, node.GetTitle());
                }
            }
        }

        private static bool IsMainMethod(MethodDeclarationSyntax methodDeclaration, SyntaxTokenList modifiers, string methodName)
        {
            return string.Equals(methodName, "Main", StringComparison.Ordinal)
                && modifiers.Contains(SyntaxKind.StaticKeyword)
                && methodDeclaration.TypeParameterList == null
                && methodDeclaration.ParameterList?.Parameters.Count <= 1;
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxNode node, string declarationName)
        {
            context.ReportDiagnostic(DiagnosticDescriptors.RemoveUnusedMemberDeclaration, GetIdentifier(node), declarationName);
        }

        private static bool IsPrivate(MemberDeclarationSyntax memberDeclaration, SyntaxTokenList modifiers)
        {
            Accessibility accessibility = modifiers.GetAccessibility();

            if (accessibility == Accessibility.NotApplicable)
                accessibility = memberDeclaration.GetDefaultExplicitAccessibility();

            return accessibility == Accessibility.Private;
        }

        internal static SyntaxToken GetIdentifier(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)node).Identifier;
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)node).Identifier;
                case SyntaxKind.EventFieldDeclaration:
                    return ((EventFieldDeclarationSyntax)node).Declaration.Variables.First().Identifier;
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)node).Declaration.Variables.First().Identifier;
                case SyntaxKind.VariableDeclarator:
                    return ((VariableDeclaratorSyntax)node).Identifier;
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)node).Identifier;
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)node).Identifier;
                default:
                    throw new InvalidOperationException();
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            SyntaxNode node,
            CancellationToken cancellationToken)
        {
            if (node.Kind() == SyntaxKind.VariableDeclarator)
            {
                var variableDeclaration = (VariableDeclarationSyntax)node.Parent;

                if (variableDeclaration.Variables.Count == 1)
                {
                    SyntaxNode parent = variableDeclaration.Parent;

                    return document.RemoveNodeAsync(parent, RemoveHelper.GetRemoveOptions(parent), cancellationToken);
                }
            }

            return document.RemoveNodeAsync(node, RemoveHelper.GetRemoveOptions(node), cancellationToken);
        }
    }
}
