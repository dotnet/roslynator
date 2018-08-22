// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExtractGenericTypeRefactoring
    {
        public static bool CanRefactor(RefactoringContext context, GenericNameSyntax name)
        {
            TypeSyntax typeArgument = name.TypeArgumentList?.Arguments.SingleOrDefault(shouldThrow: false);

            return typeArgument != null
                && context.Span.IsBetweenSpans(typeArgument)
                && IsTypeOrReturnType(name);
        }

        private static bool IsTypeOrReturnType(GenericNameSyntax name)
        {
            SyntaxNode parent = name.Parent;

            switch (parent.Kind())
            {
                case SyntaxKind.TupleElement:
                    return ((TupleElementSyntax)parent).Type == name;
                case SyntaxKind.RefType:
                    return ((RefTypeSyntax)parent).Type == name;
                case SyntaxKind.RefValueExpression:
                    return ((RefValueExpressionSyntax)parent).Type == name;
                case SyntaxKind.DefaultExpression:
                    return ((DefaultExpressionSyntax)parent).Type == name;
                case SyntaxKind.TypeOfExpression:
                    return ((TypeOfExpressionSyntax)parent).Type == name;
                case SyntaxKind.SizeOfExpression:
                    return ((SizeOfExpressionSyntax)parent).Type == name;
                case SyntaxKind.DeclarationExpression:
                    return ((DeclarationExpressionSyntax)parent).Type == name;
                case SyntaxKind.CastExpression:
                    return ((CastExpressionSyntax)parent).Type == name;
                case SyntaxKind.ObjectCreationExpression:
                    return ((ObjectCreationExpressionSyntax)parent).Type == name;
                case SyntaxKind.StackAllocArrayCreationExpression:
                    return ((StackAllocArrayCreationExpressionSyntax)parent).Type == name;
                case SyntaxKind.FromClause:
                    return ((FromClauseSyntax)parent).Type == name;
                case SyntaxKind.JoinClause:
                    return ((JoinClauseSyntax)parent).Type == name;
                case SyntaxKind.DeclarationPattern:
                    return ((DeclarationPatternSyntax)parent).Type == name;
                case SyntaxKind.VariableDeclaration:
                    return ((VariableDeclarationSyntax)parent).Type == name;
                case SyntaxKind.ForEachStatement:
                    return ((ForEachStatementSyntax)parent).Type == name;
                case SyntaxKind.CatchDeclaration:
                    return ((CatchDeclarationSyntax)parent).Type == name;
                case SyntaxKind.SimpleBaseType:
                    return ((SimpleBaseTypeSyntax)parent).Type == name;
                case SyntaxKind.TypeConstraint:
                    return ((TypeConstraintSyntax)parent).Type == name;
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)parent).Type == name;
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)parent).Type == name;
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)parent).Type == name;
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)parent).Type == name;
                case SyntaxKind.Parameter:
                    return ((ParameterSyntax)parent).Type == name;
                case SyntaxKind.TypeCref:
                    return ((TypeCrefSyntax)parent).Type == name;
                case SyntaxKind.ConversionOperatorMemberCref:
                    return ((ConversionOperatorMemberCrefSyntax)parent).Type == name;
                case SyntaxKind.CrefParameter:
                    return ((CrefParameterSyntax)parent).Type == name;
                case SyntaxKind.LocalFunctionStatement:
                    return ((LocalFunctionStatementSyntax)parent).ReturnType == name;
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)parent).ReturnType == name;
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)parent).ReturnType == name;
                case SyntaxKind.OperatorDeclaration:
                    return ((OperatorDeclarationSyntax)parent).ReturnType == name;
            }

            return false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            GenericNameSyntax genericName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            TypeSyntax typeSyntax = genericName
                .TypeArgumentList
                .Arguments[0]
                .WithTriviaFrom(genericName);

            return document.ReplaceNodeAsync(genericName, typeSyntax, cancellationToken);
        }
    }
}
