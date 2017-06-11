// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MemberTypeMustMatchOverriddenMemberTypeRefactoring
    {
        internal static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            if (methodDeclaration.Modifiers.Contains(SyntaxKind.OverrideKeyword)
                && ((IMethodSymbol)context.ContainingSymbol)?.OverriddenMethod?.ReturnType?.IsErrorType() == false)
            {
                Analyze(context, methodDeclaration.Identifier);
            }
        }

        internal static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

            if (propertyDeclaration.Modifiers.Contains(SyntaxKind.OverrideKeyword)
                && ((IPropertySymbol)context.ContainingSymbol)?.OverriddenProperty?.Type?.IsErrorType() == false)
            {
                Analyze(context, propertyDeclaration.Identifier);
            }
        }

        internal static void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            if (indexerDeclaration.Modifiers.Contains(SyntaxKind.OverrideKeyword)
                && ((IPropertySymbol)context.ContainingSymbol)?.OverriddenProperty?.Type?.IsErrorType() == false)
            {
                Analyze(context, indexerDeclaration.ThisKeyword);
            }
        }

        internal static void AnalyzeEventDeclaration(SyntaxNodeAnalysisContext context)
        {
            var eventDeclaration = (EventDeclarationSyntax)context.Node;

            if (eventDeclaration.Modifiers.Contains(SyntaxKind.OverrideKeyword)
                && ((IEventSymbol)context.ContainingSymbol)?.OverriddenEvent?.Type?.IsErrorType() == false)
            {
                Analyze(context, eventDeclaration.Identifier);
            }
        }

        internal static void AnalyzeEventFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var eventFieldDeclaration = (EventFieldDeclarationSyntax)context.Node;

            if (eventFieldDeclaration.Modifiers.Contains(SyntaxKind.OverrideKeyword)
                && ((IEventSymbol)context.ContainingSymbol)?.OverriddenEvent?.Type?.IsErrorType() == false)
            {
                VariableDeclaratorSyntax declarator = eventFieldDeclaration.Declaration?.Variables.FirstOrDefault();

                if (declarator != null)
                    Analyze(context, declarator.Identifier);
            }
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, SyntaxToken identifier)
        {
            if (context.SemanticModel.ContainsCompilerDiagnostic(CSharpErrorCodes.MemberTypeMustMatchOverridenMemberType))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.MemberTypeMustMatchOverriddenMemberType,
                    identifier);
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            TypeSyntax newType,
            CancellationToken cancellationToken)
        {
            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)memberDeclaration;

                        MethodDeclarationSyntax newNode = methodDeclaration.WithReturnType(newType.WithTriviaFrom(methodDeclaration.ReturnType));

                        return document.ReplaceNodeAsync(methodDeclaration, newNode, cancellationToken);
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var propertyDeclaration = (PropertyDeclarationSyntax)memberDeclaration;

                        PropertyDeclarationSyntax newNode = propertyDeclaration.WithType(newType.WithTriviaFrom(propertyDeclaration.Type));

                        return document.ReplaceNodeAsync(propertyDeclaration, newNode, cancellationToken);
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var indexerDeclaration = (IndexerDeclarationSyntax)memberDeclaration;

                        IndexerDeclarationSyntax newNode = indexerDeclaration.WithType(newType.WithTriviaFrom(indexerDeclaration.Type));

                        return document.ReplaceNodeAsync(indexerDeclaration, newNode, cancellationToken);
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        var eventDeclaration = (EventDeclarationSyntax)memberDeclaration;

                        EventDeclarationSyntax newNode = eventDeclaration.WithType(newType.WithTriviaFrom(eventDeclaration.Type));

                        return document.ReplaceNodeAsync(eventDeclaration, newNode, cancellationToken);
                    }
                case SyntaxKind.EventFieldDeclaration:
                    {
                        var eventDeclaration = (EventFieldDeclarationSyntax)memberDeclaration;

                        VariableDeclarationSyntax declaration = eventDeclaration.Declaration;

                        EventFieldDeclarationSyntax newNode = eventDeclaration.WithDeclaration(declaration.WithType(newType.WithTriviaFrom(declaration.Type)));

                        return document.ReplaceNodeAsync(eventDeclaration, newNode, cancellationToken);
                    }
                default:
                    {
                        Debug.Fail(memberDeclaration.Kind().ToString());
                        return Task.FromResult(document);
                    }
            }
        }
    }
}
