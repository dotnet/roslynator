// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MarkLocalVariableAsConstRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            LocalDeclarationStatementSyntax localDeclaration,
            CancellationToken cancellationToken)
        {
            TypeSyntax type = localDeclaration.Declaration.Type;

            LocalDeclarationStatementSyntax newNode = localDeclaration;

            if (type.IsVar)
            {
                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, cancellationToken);

                TypeSyntax newType = typeSymbol.ToMinimalTypeSyntax(semanticModel, localDeclaration.SpanStart);

                newNode = newNode.ReplaceNode(type, newType.WithTriviaFrom(type));
            }

            Debug.Assert(!newNode.Modifiers.Any(), newNode.Modifiers.ToString());

            if (newNode.Modifiers.Any())
            {
                newNode = newNode.InsertModifier(SyntaxKind.ConstKeyword);
            }
            else
            {
                newNode = newNode
                    .WithoutLeadingTrivia()
                    .WithModifiers(TokenList(ConstKeyword().WithLeadingTrivia(newNode.GetLeadingTrivia())));
            }

            return await document.ReplaceNodeAsync(localDeclaration, newNode).ConfigureAwait(false);
        }
    }
}
