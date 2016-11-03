// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReturnExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ExpressionSyntax expression)
        {
            if (expression != null
                && context.SupportsSemanticModel)
            {
                MemberDeclarationSyntax declaration = GetContainingMethodOrPropertyOrIndexer(expression);

                if (declaration != null)
                {
                    TypeSyntax memberType = GetMemberType(declaration);

                    if (memberType != null)
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        ITypeSymbol memberTypeSymbol = semanticModel
                            .GetTypeInfo(memberType, context.CancellationToken)
                            .Type;

                        if (memberTypeSymbol != null)
                        {
                            ITypeSymbol expressionSymbol = semanticModel
                                .GetTypeInfo(expression, context.CancellationToken)
                                .Type;

                            if (expressionSymbol?.IsErrorType() == false)
                            {
                                if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddBooleanComparison)
                                    && memberTypeSymbol.IsBoolean()
                                    && expressionSymbol.IsNamedType())
                                {
                                    var namedTypeSymbol = (INamedTypeSymbol)expressionSymbol;

                                    if (namedTypeSymbol?.IsNullableOf(SpecialType.System_Boolean) == true)
                                    {
                                        AddBooleanComparisonRefactoring.RegisterRefactoring(context, expression);
                                    }
                                }

                                ISymbol memberSymbol = semanticModel.GetDeclaredSymbol(declaration, context.CancellationToken);

                                if (context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeMemberTypeAccordingToReturnExpression))
                                {
                                    ITypeSymbol newType = GetMemberNewType(memberSymbol, memberTypeSymbol, expression, expressionSymbol, semanticModel, context.CancellationToken);

                                    if (newType?.IsErrorType() == false
                                        && !memberTypeSymbol.Equals(newType))
                                    {
                                        if (newType.IsNamedType() && memberTypeSymbol.IsNamedType())
                                        {
                                            var newNamedType = (INamedTypeSymbol)newType;

                                            INamedTypeSymbol orderedEnumerableSymbol = semanticModel.Compilation.GetTypeByMetadataName("System.Linq.IOrderedEnumerable`1");

                                            if (newNamedType.ConstructedFrom == orderedEnumerableSymbol)
                                            {
                                                INamedTypeSymbol enumerableSymbol = semanticModel.Compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1");

                                                if (enumerableSymbol != null
                                                    && ((INamedTypeSymbol)memberTypeSymbol).ConstructedFrom != enumerableSymbol)
                                                {
                                                    RegisterChangeType(context, declaration, memberType, enumerableSymbol.Construct(newNamedType.TypeArguments.ToArray()));
                                                }
                                            }
                                        }

                                        RegisterChangeType(context, declaration, memberType, newType);
                                    }
                                }

                                if (context.IsAnyRefactoringEnabled(RefactoringIdentifiers.AddCastExpression, RefactoringIdentifiers.AddToMethodInvocation)
                                    && !memberTypeSymbol.IsErrorType())
                                {
                                    ITypeSymbol castTypeSymbol = GetCastTypeSymbol(memberSymbol, memberTypeSymbol, expressionSymbol, semanticModel);

                                    if (castTypeSymbol != null)
                                    {
                                        ModifyExpressionRefactoring.ComputeRefactoring(
                                           context,
                                           expression,
                                           castTypeSymbol,
                                           semanticModel);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void RegisterChangeType(RefactoringContext context, MemberDeclarationSyntax member, TypeSyntax type, ITypeSymbol newType)
        {
            context.RegisterRefactoring(
            $"Change {GetText(member)} type to '{newType.ToDisplayString(SyntaxUtility.DefaultSymbolDisplayFormat)}'",
            cancellationToken =>
            {
                return ChangeTypeRefactoring.ChangeTypeAsync(
                    context.Document,
                    type,
                    newType,
                    cancellationToken);
            });
        }

        private static ITypeSymbol GetMemberNewType(
            ISymbol memberSymbol,
            ITypeSymbol memberTypeSymbol,
            ExpressionSyntax expression,
            ITypeSymbol expressionSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (memberSymbol.IsAsyncMethod())
            {
                if (expression.IsKind(SyntaxKind.AwaitExpression))
                {
                    var awaitExpression = (AwaitExpressionSyntax)expression;

                    if (awaitExpression.Expression != null)
                    {
                        var awaitableSymbol = semanticModel
                            .GetTypeInfo(awaitExpression.Expression, cancellationToken)
                            .Type as INamedTypeSymbol;

                        if (awaitableSymbol != null)
                        {
                            INamedTypeSymbol taskOfTSymbol = semanticModel
                                .Compilation
                                .GetTypeByMetadataName("System.Threading.Tasks.Task`1");

                            if (awaitableSymbol.ConstructedFrom.Equals(taskOfTSymbol))
                                return awaitableSymbol;
                        }
                    }
                }
                else if (memberTypeSymbol.IsNamedType())
                {
                    INamedTypeSymbol taskOfTSymbol = semanticModel
                        .Compilation
                        .GetTypeByMetadataName("System.Threading.Tasks.Task`1");

                    if (((INamedTypeSymbol)memberTypeSymbol).ConstructedFrom.Equals(taskOfTSymbol))
                    {
                        if (expressionSymbol.IsNamedType()
                            && ((INamedTypeSymbol)expressionSymbol).ConstructedFrom.Equals(taskOfTSymbol))
                        {
                            return null;
                        }

                        INamedTypeSymbol taskSymbol = semanticModel
                            .Compilation
                            .GetTypeByMetadataName("System.Threading.Tasks.Task");

                        if (expressionSymbol.Equals(taskSymbol))
                            return null;

                        return taskOfTSymbol.Construct(expressionSymbol);
                    }
                }
            }
            else
            {
                return expressionSymbol;
            }

            return null;
        }

        private static ITypeSymbol GetCastTypeSymbol(
            ISymbol memberSymbol,
            ITypeSymbol memberTypeSymbol,
            ITypeSymbol expressionSymbol,
            SemanticModel semanticModel)
        {
            if (memberSymbol.IsAsyncMethod())
            {
                if (memberTypeSymbol.IsNamedType())
                {
                    var namedTypeSymbol = (INamedTypeSymbol)memberTypeSymbol;

                    INamedTypeSymbol taskOfTSymbol = semanticModel.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1");

                    if (taskOfTSymbol != null
                        && namedTypeSymbol.ConstructedFrom.Equals(taskOfTSymbol)
                        && !namedTypeSymbol.TypeArguments[0].Equals(expressionSymbol))
                    {
                        return namedTypeSymbol.TypeArguments[0];
                    }
                }
            }
            else if (!memberTypeSymbol.Equals(expressionSymbol))
            {
                return memberTypeSymbol;
            }

            return null;
        }

        internal static TypeSyntax GetMemberType(MemberDeclarationSyntax declaration)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)declaration).ReturnType;
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)declaration).Type;
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)declaration).Type;
                default:
                    return null;
            }
        }

        internal static string GetText(MemberDeclarationSyntax declaration)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return "return";
                case SyntaxKind.PropertyDeclaration:
                    return "property";
                case SyntaxKind.IndexerDeclaration:
                    return "indexer";
                default:
                    return null;
            }
        }

        internal static MemberDeclarationSyntax GetContainingMethodOrPropertyOrIndexer(ExpressionSyntax expression)
        {
            foreach (SyntaxNode ancestor in expression.Ancestors())
            {
                switch (ancestor.Kind())
                {
                    case SyntaxKind.MethodDeclaration:
                    case SyntaxKind.PropertyDeclaration:
                    case SyntaxKind.IndexerDeclaration:
                        return (MemberDeclarationSyntax)ancestor;
                    case SyntaxKind.SimpleLambdaExpression:
                    case SyntaxKind.ParenthesizedLambdaExpression:
                    case SyntaxKind.AnonymousMethodExpression:
                    case SyntaxKind.ConstructorDeclaration:
                    case SyntaxKind.DestructorDeclaration:
                    case SyntaxKind.EventDeclaration:
                    case SyntaxKind.ConversionOperatorDeclaration:
                    case SyntaxKind.OperatorDeclaration:
                    case SyntaxKind.IncompleteMember:
                        return null;
                }
            }

            return null;
        }
    }
}
