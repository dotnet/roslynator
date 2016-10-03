// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    public static class TypeSyntaxRefactoring
    {
        private static readonly SymbolDisplayFormat _symbolDisplayFormatForTypeSyntax = new SymbolDisplayFormat(
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        public static SymbolDisplayFormat SymbolDisplayFormat { get; } = new SymbolDisplayFormat(
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        public static TypeSyntax CreateTypeSyntax(ITypeSymbol typeSymbol)
        {
            return CreateTypeSyntax(typeSymbol, _symbolDisplayFormatForTypeSyntax);
        }

        public static TypeSyntax CreateTypeSyntax(ITypeSymbol typeSymbol, SymbolDisplayFormat symbolDisplayFormat)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            string s = typeSymbol.ToDisplayString(symbolDisplayFormat);

            return SyntaxFactory.ParseTypeName(s);
        }

        public static async Task<Document> ChangeTypeAsync(
            Document document,
            TypeSyntax type,
            ITypeSymbol typeSymbol,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            TypeSyntax newType = CreateTypeSyntax(typeSymbol)
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
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            newType = newType
                .WithTriviaFrom(type)
                .WithSimplifierAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(type, newType);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> ChangeTypeToVarAsync(
            Document document,
            TypeSyntax type,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            IdentifierNameSyntax newType = CSharpFactory.Var().WithTriviaFrom(type);

            SyntaxNode newRoot = oldRoot.ReplaceNode(type, newType);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
