// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MemberTypeMustMatchOverriddenMemberTypeRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            ITypeSymbol typeSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            TypeSyntax newType = typeSymbol.ToMinimalTypeSyntax(semanticModel, memberDeclaration.SpanStart);

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

                        EventFieldDeclarationSyntax newNode = eventDeclaration.WithDeclaration(declaration.WithType(newType.WithTriviaFrom(declaration.Type)));

                        return await document.ReplaceNodeAsync(eventDeclaration, newNode, cancellationToken).ConfigureAwait(false);
                    }
                default:
                    {
                        Debug.Fail(memberDeclaration.Kind().ToString());
                        return document;
                    }
            }
        }
    }
}
