// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp
{
    internal static class SyntaxRefactorings
    {
        public static TypeSyntax ChangeType(TypeSyntax type, ITypeSymbol typeSymbol)
        {
            TypeSyntax newType = typeSymbol
                .ToTypeSyntax()
                .WithTriviaFrom(type);

            if (newType is TupleTypeSyntax tupleType)
            {
                SeparatedSyntaxList<TupleElementSyntax> newElements = tupleType
                    .Elements
                    .Select(tupleElement => tupleElement.WithType(tupleElement.Type.WithSimplifierAnnotation()))
                    .ToSeparatedSyntaxList();

                return tupleType.WithElements(newElements);
            }
            else
            {
                return newType.WithSimplifierAnnotation();
            }
        }

        public static VariableDeclarationSyntax ChangeTypeAndAddAwait(
            VariableDeclarationSyntax variableDeclaration,
            VariableDeclaratorSyntax variableDeclarator,
            ITypeSymbol typeSymbol)
        {
            TypeSyntax type = variableDeclaration.Type;

            ExpressionSyntax value = variableDeclarator.Initializer.Value;

            AwaitExpressionSyntax newValue = AwaitExpression(value.WithoutTrivia()).WithTriviaFrom(value);

            TypeSyntax newType = ChangeType(type, typeSymbol);

            return variableDeclaration
                .ReplaceNode(value, newValue)
                .WithType(newType);
        }
    }
}
