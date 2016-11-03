// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    public static class ChangeTypeRefactoring
    {
        public static async Task<Document> ChangeTypeAsync(
           Document document,
           TypeSyntax type,
           ITypeSymbol typeSymbol,
           CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            TypeSyntax newType = Type(typeSymbol)
                .WithTriviaFrom(type)
                .WithSimplifierAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(type, newType);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> ChangeTypeAsync(
            Document document,
            TypeSyntax type,
            TypeSyntax newType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (newType == null)
                throw new ArgumentNullException(nameof(newType));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            newType = newType
                .WithTriviaFrom(type)
                .WithSimplifierAnnotation();

            SyntaxNode newRoot = root.ReplaceNode(type, newType);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> ChangeTypeToVarAsync(
            Document document,
            TypeSyntax type,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            IdentifierNameSyntax newType = Var().WithTriviaFrom(type);

            SyntaxNode newRoot = root.ReplaceNode(type, newType);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
