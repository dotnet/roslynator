// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceCountWithLengthOrLengthWithCountRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, MemberAccessExpressionSyntax memberAccess)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceCountWithLengthOrLengthWithCount)
                && memberAccess.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                memberAccess = GetTopmostMemberAccessExpression(memberAccess);

                if (memberAccess.Name?.Span.Contains(context.Span) == true)
                {
                    string name = memberAccess.Name.Identifier.ValueText;

                    if (string.Equals(name, "Count", StringComparison.Ordinal))
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        if (CanRefactor(memberAccess, "Length", semanticModel, context.CancellationToken))
                            RegisterRefactoring(context, memberAccess, "Count", "Length");
                    }
                    else if (string.Equals(name, "Length", StringComparison.Ordinal))
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        if (CanRefactor(memberAccess, "Count", semanticModel, context.CancellationToken))
                            RegisterRefactoring(context, memberAccess, "Length", "Count");
                    }
                }
            }
        }

        private static void RegisterRefactoring(
            RefactoringContext context,
            MemberAccessExpressionSyntax memberAccess,
            string name,
            string newName)
        {
            context.RegisterRefactoring(
                $"Replace '{name}' with '{newName}'",
                cancellationToken => RefactorAsync(context.Document, memberAccess, newName, cancellationToken));
        }

        private static bool CanRefactor(
            MemberAccessExpressionSyntax memberAccess,
            string newName,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ISymbol memberAccessSymbol = semanticModel
                .GetSymbol(memberAccess, cancellationToken);

            if (memberAccessSymbol == null)
            {
                ITypeSymbol expressionSymbol = semanticModel
                    .GetTypeSymbol(memberAccess.Expression, cancellationToken);

                if (expressionSymbol != null)
                {
                    if (expressionSymbol.IsArrayType())
                        expressionSymbol = ((IArrayTypeSymbol)expressionSymbol).ElementType;

                    foreach (ISymbol symbol in expressionSymbol.GetMembers(newName))
                    {
                        if (symbol.IsPublic()
                            && !symbol.IsStatic
                            && symbol.IsProperty()
                            && ((IPropertySymbol)symbol).IsReadOnly)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static Task<Document> RefactorAsync(
            Document document,
            MemberAccessExpressionSyntax memberAccess,
            string newName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            MemberAccessExpressionSyntax newNode = memberAccess
                .WithName(SyntaxFactory.IdentifierName(newName))
                .WithTriviaFrom(memberAccess.Name);

            return document.ReplaceNodeAsync(memberAccess, newNode, cancellationToken);
        }

        private static MemberAccessExpressionSyntax GetTopmostMemberAccessExpression(MemberAccessExpressionSyntax memberAccess)
        {
            while (memberAccess.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
                memberAccess = (MemberAccessExpressionSyntax)memberAccess.Parent;

            return memberAccess;
        }
    }
}
