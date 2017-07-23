// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Comparers;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseConstantInsteadOfFieldRefactoring
    {
        public static bool CanRefactor(FieldDeclarationSyntax fieldDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            SyntaxTokenList modifiers = fieldDeclaration.Modifiers;

            return modifiers.Contains(SyntaxKind.StaticKeyword)
                && modifiers.Contains(SyntaxKind.ReadOnlyKeyword)
                && !modifiers.Contains(SyntaxKind.NewKeyword)
                && SupportsConstant(fieldDeclaration.Declaration, semanticModel, cancellationToken);
        }

        private static bool SupportsConstant(
            VariableDeclarationSyntax variableDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            foreach (VariableDeclaratorSyntax declarator in variableDeclaration.Variables)
            {
                if (!SupportsConstant(variableDeclaration.Type, declarator, semanticModel, cancellationToken))
                    return false;
            }

            return true;
        }

        private static bool SupportsConstant(
            TypeSyntax type,
            VariableDeclaratorSyntax declarator,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax value = declarator.Initializer?.Value;

            return value != null
                && semanticModel.GetTypeSymbol(type, cancellationToken)?.SupportsConstantValue() == true
                && semanticModel.GetConstantValue(value, cancellationToken).HasValue;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            FieldDeclarationSyntax fieldDeclaration,
            CancellationToken cancellationToken)
        {
            FieldDeclarationSyntax newNode = fieldDeclaration
                .InsertModifier(SyntaxKind.ConstKeyword, ModifierComparer.Instance)
                .RemoveModifier(SyntaxKind.StaticKeyword)
                .RemoveModifier(SyntaxKind.ReadOnlyKeyword);

            return document.ReplaceNodeAsync(fieldDeclaration, newNode, cancellationToken);
        }
    }
}
