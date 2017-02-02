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

namespace Roslynator.CSharp.Refactorings
{
    internal static class MemberTypeMustMatchOverriddenMemberTypeRefactoring
    {
        internal static void Analyze(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration.Modifiers.Contains(SyntaxKind.OverrideKeyword))
            {
                IMethodSymbol methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

                if (methodSymbol?.OverriddenMethod != null)
                    Analyze(context, methodDeclaration.Identifier);
            }
        }

        internal static void Analyze(SyntaxNodeAnalysisContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration.Modifiers.Contains(SyntaxKind.OverrideKeyword))
            {
                IPropertySymbol propertySymbol = context.SemanticModel.GetDeclaredSymbol(propertyDeclaration, context.CancellationToken);

                if (propertySymbol?.OverriddenProperty != null)
                    Analyze(context, propertyDeclaration.Identifier);
            }
        }

        internal static void Analyze(SyntaxNodeAnalysisContext context, IndexerDeclarationSyntax indexerDeclaration)
        {
            if (indexerDeclaration.Modifiers.Contains(SyntaxKind.OverrideKeyword))
            {
                IPropertySymbol propertySymbol = context.SemanticModel.GetDeclaredSymbol(indexerDeclaration, context.CancellationToken);

                if (propertySymbol?.OverriddenProperty != null)
                    Analyze(context, indexerDeclaration.ThisKeyword);
            }
        }

        internal static void Analyze(SyntaxNodeAnalysisContext context, EventDeclarationSyntax eventDeclaration)
        {
            if (eventDeclaration.Modifiers.Contains(SyntaxKind.OverrideKeyword))
            {
                IEventSymbol eventSymbol = context.SemanticModel.GetDeclaredSymbol(eventDeclaration, context.CancellationToken);

                if (eventSymbol?.OverriddenEvent != null)
                    Analyze(context, eventDeclaration.Identifier);
            }
        }

        internal static void Analyze(SyntaxNodeAnalysisContext context, EventFieldDeclarationSyntax eventFieldDeclaration)
        {
            if (eventFieldDeclaration.Modifiers.Contains(SyntaxKind.OverrideKeyword))
            {
                VariableDeclaratorSyntax declarator = eventFieldDeclaration.Declaration?.Variables.FirstOrDefault();

                if (declarator != null)
                {
                    var eventSymbol = context.SemanticModel.GetDeclaredSymbol(declarator, context.CancellationToken) as IEventSymbol;

                    if (eventSymbol?.OverriddenEvent != null)
                        Analyze(context, declarator.Identifier);
                }
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

        public static async Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            TypeSyntax newType,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)memberDeclaration;

                        MethodDeclarationSyntax newNode = methodDeclaration.WithReturnType(newType.WithTriviaFrom(methodDeclaration.ReturnType));

                        return await document.ReplaceNodeAsync(methodDeclaration, newNode, cancellationToken).ConfigureAwait(false);
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var propertyDeclaration = (PropertyDeclarationSyntax)memberDeclaration;

                        PropertyDeclarationSyntax newNode = propertyDeclaration.WithType(newType.WithTriviaFrom(propertyDeclaration.Type));

                        return await document.ReplaceNodeAsync(propertyDeclaration, newNode, cancellationToken).ConfigureAwait(false);
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var indexerDeclaration = (IndexerDeclarationSyntax)memberDeclaration;

                        IndexerDeclarationSyntax newNode = indexerDeclaration.WithType(newType.WithTriviaFrom(indexerDeclaration.Type));

                        return await document.ReplaceNodeAsync(indexerDeclaration, newNode, cancellationToken).ConfigureAwait(false);
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        var eventDeclaration = (EventDeclarationSyntax)memberDeclaration;

                        EventDeclarationSyntax newNode = eventDeclaration.WithType(newType.WithTriviaFrom(eventDeclaration.Type));

                        return await document.ReplaceNodeAsync(eventDeclaration, newNode, cancellationToken).ConfigureAwait(false);
                    }
                case SyntaxKind.EventFieldDeclaration:
                    {
                        var eventDeclaration = (EventFieldDeclarationSyntax)memberDeclaration;

                        VariableDeclarationSyntax declaration = eventDeclaration.Declaration;
                        VariableDeclaratorSyntax declarator = declaration.Variables.First();

                        EventFieldDeclarationSyntax newNode = eventDeclaration.WithDeclaration(declaration.WithType(newType.WithTriviaFrom(declaration.Type)));

                        return await document.ReplaceNodeAsync(eventDeclaration, newNode, cancellationToken).ConfigureAwait(false);
                    }
                default:
                    {
                        Debug.Assert(false, memberDeclaration.Kind().ToString());
                        return document;
                    }
            }
        }
    }
}
