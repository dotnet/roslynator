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
        public static bool CanRefactor(
            FieldDeclarationSyntax fieldDeclaration,
            SemanticModel semanticModel,
            bool onlyPrivate = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            bool isStatic = false;
            bool isReadOnly = false;

            foreach (SyntaxToken modifier in fieldDeclaration.Modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.ConstKeyword:
                    case SyntaxKind.NewKeyword:
                        {
                            return false;
                        }
                    case SyntaxKind.PublicKeyword:
                    case SyntaxKind.InternalKeyword:
                    case SyntaxKind.ProtectedKeyword:
                        {
                            if (onlyPrivate)
                                return false;

                            break;
                        }
                    case SyntaxKind.StaticKeyword:
                        {
                            isStatic = true;
                            break;
                        }
                    case SyntaxKind.ReadOnlyKeyword:
                        {
                            isReadOnly = true;
                            break;
                        }
                }
            }

            if (!isStatic)
                return false;

            if (!isReadOnly)
                return false;

            SeparatedSyntaxList<VariableDeclaratorSyntax> declarators = fieldDeclaration.Declaration.Variables;

            VariableDeclaratorSyntax firstDeclarator = declarators.First();

            var fieldSymbol = (IFieldSymbol)semanticModel.GetDeclaredSymbol(firstDeclarator, cancellationToken);

            if (fieldSymbol == null)
                return false;

            if (!fieldSymbol.Type.SupportsConstantValue())
                return false;

            foreach (VariableDeclaratorSyntax declarator in declarators)
            {
                ExpressionSyntax value = declarator.Initializer?.Value;

                if (value == null)
                    return false;

                if (!semanticModel.GetConstantValue(value, cancellationToken).HasValue)
                    return false;
            }

            return true;
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
