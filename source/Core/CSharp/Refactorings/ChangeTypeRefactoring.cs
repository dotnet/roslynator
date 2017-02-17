// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ChangeTypeRefactoring
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

            TypeSyntax newType = typeSymbol.ToTypeSyntax()
                .WithTriviaFrom(type)
                .WithSimplifierAnnotation();

            return await document.ReplaceNodeAsync(type, newType, cancellationToken).ConfigureAwait(false);
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

            return await document.ReplaceNodeAsync(type, newType.WithTriviaFrom(type), cancellationToken).ConfigureAwait(false);
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

            IdentifierNameSyntax newType = VarType().WithTriviaFrom(type);

            return await document.ReplaceNodeAsync(type, newType, cancellationToken).ConfigureAwait(false);
        }
    }
}
