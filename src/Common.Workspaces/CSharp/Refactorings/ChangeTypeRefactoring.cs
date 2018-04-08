// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ChangeTypeRefactoring
    {
        public static Task<Document> ChangeTypeAsync(
           Document document,
           TypeSyntax type,
           ITypeSymbol typeSymbol,
           CancellationToken cancellationToken = default(CancellationToken))
        {
            TypeSyntax newType = typeSymbol.ToTypeSyntax()
                .WithTriviaFrom(type)
                .WithSimplifierAnnotation();

            return document.ReplaceNodeAsync(type, newType, cancellationToken);
        }

        public static Task<Document> ChangeTypeAsync(
            Document document,
            TypeSyntax type,
            TypeSyntax newType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return document.ReplaceNodeAsync(type, newType.WithTriviaFrom(type), cancellationToken);
        }

        public static Task<Document> ChangeTypeToVarAsync(
            Document document,
            TypeSyntax type,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IdentifierNameSyntax newType = VarType().WithTriviaFrom(type);

            return document.ReplaceNodeAsync(type, newType, cancellationToken);
        }
    }
}
