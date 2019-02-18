// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    internal static class DocumentRefactoringFactory
    {
        public static Func<CancellationToken, Task<Document>> ChangeTypeAndAddAwait(
            Document document,
            VariableDeclarationSyntax variableDeclaration,
            VariableDeclaratorSyntax variableDeclarator,
            ITypeSymbol newTypeSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            if (!newTypeSymbol.OriginalDefinition.EqualsOrInheritsFromTaskOfT())
                return default;

            if (!(semanticModel.GetEnclosingSymbol(variableDeclaration.SpanStart, cancellationToken) is IMethodSymbol methodSymbol))
                return default;

            if (!methodSymbol.MethodKind.Is(MethodKind.Ordinary, MethodKind.LocalFunction))
                return default;

            SyntaxNode containingMethod = GetContainingMethod();

            if (containingMethod == null)
                return default;

            SyntaxNode bodyOrExpressionBody = GetBodyOrExpressionBody();

            if (bodyOrExpressionBody == null)
                return default;

            foreach (SyntaxNode descendant in bodyOrExpressionBody.DescendantNodes())
            {
                if (descendant.IsKind(SyntaxKind.ReturnStatement))
                {
                    var returnStatement = (ReturnStatementSyntax)descendant;

                    if (returnStatement
                        .Expression?
                        .WalkDownParentheses()
                        .IsKind(SyntaxKind.AwaitExpression) == false)
                    {
                        return default;
                    }
                }
            }

            ITypeSymbol typeArgument = ((INamedTypeSymbol)newTypeSymbol).TypeArguments[0];

            return ct => DocumentRefactorings.ChangeTypeAndAddAwaitAsync(document, variableDeclaration, variableDeclarator, containingMethod, typeArgument, ct);

            SyntaxNode GetContainingMethod()
            {
                foreach (SyntaxReference syntaxReference in methodSymbol.DeclaringSyntaxReferences)
                {
                    SyntaxNode syntax = syntaxReference.GetSyntax(cancellationToken);

                    if (syntax.Contains(variableDeclaration))
                        return syntax;
                }

                return null;
            }

            SyntaxNode GetBodyOrExpressionBody()
            {
                switch (containingMethod.Kind())
                {
                    case SyntaxKind.MethodDeclaration:
                        return ((MethodDeclarationSyntax)containingMethod).BodyOrExpressionBody();
                    case SyntaxKind.LocalFunctionStatement:
                        return ((LocalFunctionStatementSyntax)containingMethod).BodyOrExpressionBody();
                    default:
                        throw new InvalidOperationException();
                }
            }
        }
    }
}
