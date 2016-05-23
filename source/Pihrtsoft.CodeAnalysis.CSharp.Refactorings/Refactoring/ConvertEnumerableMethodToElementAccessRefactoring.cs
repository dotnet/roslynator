// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.SyntaxRewriters;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ConvertEnumerableMethodToElementAccessRefactoring
    {
        public static void Refactor(CodeRefactoringContext context, InvocationExpressionSyntax invocation, SemanticModel semanticModel)
        {
            if (invocation.Expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true
                && invocation.ArgumentList != null)
            {
                var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

                string methodName = memberAccess.Name?.Identifier.ValueText;

                switch (methodName)
                {
                    case "First":
                    case "Last":
                        {
                            ProcessFirstOrLast(invocation, methodName, context, semanticModel);
                            break;
                        }
                    case "ElementAt":
                        {
                            ProcessElementAt(invocation, context, semanticModel);
                            break;
                        }
                }
            }
        }

        private static void ProcessFirstOrLast(InvocationExpressionSyntax invocation, string methodName, CodeRefactoringContext context, SemanticModel semanticModel)
        {
            if (invocation.ArgumentList.Arguments.Count == 0
                && (IsEnumerableExtensionMethod(invocation, methodName, semanticModel, context.CancellationToken)
                    || IsImmutableArrayExtensionMethod(invocation, methodName, semanticModel, context.CancellationToken)))
            {
                var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

                ITypeSymbol typeSymbol = semanticModel
                    .GetTypeInfo(memberAccess.Expression, context.CancellationToken)
                    .Type;

                if (typeSymbol != null
                    && (typeSymbol.IsKind(SymbolKind.ArrayType) || typeSymbol.HasPublicIndexer()))
                {
                    string propertyName = GetCountOrLengthPropertyName(memberAccess.Expression, semanticModel, context.CancellationToken);

                    if (propertyName != null)
                    {
                        context.RegisterRefactoring(
                            $"Access element using '[]' instead of '{methodName}' method",
                            cancellationToken =>
                            {
                                return RefactorAsync(
                                    context.Document,
                                    invocation,
                                    propertyName,
                                    context.CancellationToken);
                            });
                    }
                }
            }
        }

        private static bool ProcessElementAt(
            InvocationExpressionSyntax invocation,
            CodeRefactoringContext context,
            SemanticModel semanticModel)
        {
            if (invocation.ArgumentList?.Arguments.Count == 1
                && (IsEnumerableElementAtMethod(invocation, semanticModel)
                    || IsImmutableArrayElementAtMethod(invocation, semanticModel)))
            {
                var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

                ITypeSymbol typeSymbol = semanticModel
                    .GetTypeInfo(memberAccess.Expression, context.CancellationToken)
                    .Type;

                if (typeSymbol != null
                    && (typeSymbol.IsKind(SymbolKind.ArrayType) || typeSymbol.HasPublicIndexer()))
                {
                    context.RegisterRefactoring(
                            "Access element using '[]' instead of 'ElementAt' method",
                        cancellationToken => RefactorAsync(context.Document, invocation, null, cancellationToken));
                }
            }

            return false;
        }

        private static string GetCountOrLengthPropertyName(ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            ITypeSymbol typeSymbol = semanticModel
                .GetTypeInfo(expression, cancellationToken)
                .Type;

            if (typeSymbol?.IsKind(SymbolKind.ErrorType) == false
                && !typeSymbol.IsGenericIEnumerable())
            {
                if (typeSymbol.BaseType?.SpecialType == SpecialType.System_Array)
                    return "Length";

                if (typeSymbol.IsGenericImmutableArray(semanticModel))
                    return "Length";

                for (int i = 0; i < typeSymbol.AllInterfaces.Length; i++)
                {
                    if (typeSymbol.AllInterfaces[i].ConstructedFrom.SpecialType == SpecialType.System_Collections_Generic_ICollection_T)
                    {
                        foreach (ISymbol members in typeSymbol.GetMembers("Count"))
                        {
                            if (members.IsKind(SymbolKind.Property)
                                && members.DeclaredAccessibility == Accessibility.Public)
                            {
                                return "Count";
                            }
                        }
                    }
                }
            }

            return null;
        }

        private static bool IsEnumerableExtensionMethod(
            InvocationExpressionSyntax invocation,
            string methodName,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var methodSymbol = semanticModel
                .GetSymbolInfo(invocation)
                .Symbol as IMethodSymbol;

            if (methodSymbol?.ReducedFrom != null)
            {
                methodSymbol = methodSymbol.ReducedFrom;

                return methodSymbol.MetadataName == methodName
                    && methodSymbol.Parameters.Length == 1
                    && methodSymbol.ContainingType?.Equals(semanticModel.Compilation.GetTypeByMetadataName("System.Linq.Enumerable")) == true
                    && methodSymbol.Parameters[0].Type.IsGenericIEnumerable();
            }

            return false;
        }

        private static bool IsImmutableArrayExtensionMethod(
            InvocationExpressionSyntax invocation,
            string methodName,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var methodSymbol = semanticModel
                .GetSymbolInfo(invocation)
                .Symbol as IMethodSymbol;

            if (methodSymbol?.ReducedFrom != null)
            {
                methodSymbol = methodSymbol.ReducedFrom;

                return methodSymbol.MetadataName == methodName
                    && methodSymbol.Parameters.Length == 1
                    && methodSymbol.ContainingType?.Equals(semanticModel.Compilation.GetTypeByMetadataName("System.Linq.ImmutableArrayExtensions")) == true
                    && methodSymbol.Parameters[0].Type.IsGenericImmutableArray(semanticModel);
            }

            return false;
        }

        private static bool IsEnumerableElementAtMethod(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel)
        {
            var methodSymbol = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;

            if (methodSymbol?.ReducedFrom != null)
            {
                methodSymbol = methodSymbol.ReducedFrom;

                return methodSymbol.MetadataName == "ElementAt"
                    && methodSymbol.Parameters.Length == 2
                    && methodSymbol.ContainingType?.Equals(semanticModel.Compilation.GetTypeByMetadataName("System.Linq.Enumerable")) == true
                    && methodSymbol.Parameters[0].Type.IsGenericIEnumerable()
                    && methodSymbol.Parameters[1].Type.IsInt32();
            }

            return false;
        }

        private static bool IsImmutableArrayElementAtMethod(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel)
        {
            var methodSymbol = semanticModel
                .GetSymbolInfo(invocation)
                .Symbol as IMethodSymbol;

            if (methodSymbol?.ReducedFrom != null)
            {
                methodSymbol = methodSymbol.ReducedFrom;

                return methodSymbol.MetadataName == "ElementAt"
                    && methodSymbol.Parameters.Length == 2
                    && methodSymbol.ContainingType?.Equals(semanticModel.Compilation.GetTypeByMetadataName("System.Linq.ImmutableArrayExtensions")) == true
                    && methodSymbol.Parameters[0].Type.IsGenericImmutableArray(semanticModel)
                    && methodSymbol.Parameters[1].Type.IsInt32();
            }

            return false;
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            string propertyName = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            ElementAccessExpressionSyntax elementAccess = ElementAccessExpression(
                memberAccess.Expression.WithoutTrailingTrivia(),
                BracketedArgumentList(
                    SingletonSeparatedList(
                        Argument(CreateArgumentExpression(invocation, memberAccess, propertyName)))));

            SyntaxNode newRoot = oldRoot.ReplaceNode(
                invocation,
                elementAccess.WithTriviaFrom(invocation));

            return document.WithSyntaxRoot(newRoot);
        }

        private static ExpressionSyntax CreateArgumentExpression(
            InvocationExpressionSyntax invocation,
            MemberAccessExpressionSyntax memberAccess,
            string propertyName)
        {
            switch (memberAccess.Name.Identifier.ValueText)
            {
                case "First":
                    {
                        return LiteralExpression(
                            SyntaxKind.NumericLiteralExpression,
                            Literal(0));
                    }
                case "Last":
                    {
                        return BinaryExpression(
                            SyntaxKind.SubtractExpression,
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                ProcessExpression(memberAccess.Expression),
                                IdentifierName(propertyName)),
                            LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(1)));
                    }
                case "ElementAt":
                    {
                        return ProcessExpression(invocation.ArgumentList.Arguments[0].Expression);
                    }
            }

            return default(ExpressionSyntax);
        }

        private static ExpressionSyntax ProcessExpression(ExpressionSyntax expression)
        {
            if (expression
                .DescendantTrivia(expression.Span)
                .All(f => f.IsWhitespaceOrEndOfLine()))
            {
                expression = RemoveWhitespaceOrEndOfLineSyntaxRewriter.VisitNode(expression)
                    .WithAdditionalAnnotations(Formatter.Annotation);
            }

            return expression;
        }
    }
}
