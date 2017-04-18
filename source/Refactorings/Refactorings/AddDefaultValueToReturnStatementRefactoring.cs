// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddDefaultValueToReturnStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ReturnStatementSyntax returnStatement)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            IMethodSymbol methodSymbol = GetContainingSymbol(returnStatement, semanticModel, context.CancellationToken);

            if (methodSymbol?.ReturnsVoid == false)
            {
                ITypeSymbol typeSymbol = GetTypeSymbol(methodSymbol, semanticModel);

                if (typeSymbol?.IsErrorType() == false)
                {
                    context.RegisterRefactoring(
                        "Return default value",
                        cancellationToken => RefactorAsync(context.Document, returnStatement, typeSymbol, cancellationToken));
                }
            }
        }

        private static IMethodSymbol GetContainingSymbol(
            SyntaxNode node,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ISymbol symbol = semanticModel.GetEnclosingSymbol(node.SpanStart, cancellationToken);

            if (symbol?.IsMethod() == true)
            {
                var methodSymbol = (IMethodSymbol)symbol;

                switch (methodSymbol.MethodKind)
                {
                    case MethodKind.LambdaMethod:
                    case MethodKind.Ordinary:
                    case MethodKind.PropertyGet:
                    case MethodKind.UserDefinedOperator:
                    case MethodKind.Conversion:
                        return methodSymbol;
                    case MethodKind.Constructor:
                    case MethodKind.DelegateInvoke:
                    case MethodKind.Destructor:
                    case MethodKind.EventAdd:
                    case MethodKind.EventRaise:
                    case MethodKind.EventRemove:
                    case MethodKind.ExplicitInterfaceImplementation:
                    case MethodKind.StaticConstructor:
                    case MethodKind.PropertySet:
                    case MethodKind.DeclareMethod:
                    case MethodKind.ReducedExtension:
                    case MethodKind.BuiltinOperator:
                        break;
                    default:
                        {
                            Debug.Assert(false, methodSymbol.MethodKind.ToString());
                            break;
                        }
                }
            }

            return null;
        }

        private static ITypeSymbol GetTypeSymbol(
            IMethodSymbol methodSymbol,
            SemanticModel semanticModel)
        {
            ITypeSymbol returnType = methodSymbol.ReturnType;

            if (methodSymbol.IsAsync)
            {
                if (returnType.IsConstructedFromTaskOfT(semanticModel))
                    return ((INamedTypeSymbol)returnType).TypeArguments[0];
            }
            else if (!returnType.IsIEnumerableOrConstructedFromIEnumerableOfT())
            {
                return returnType;
            }

            return null;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ReturnStatementSyntax returnStatement,
            ITypeSymbol typeSymbol,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax expression = typeSymbol.ToDefaultValueSyntax();

            ReturnStatementSyntax newReturnStatement = returnStatement.WithExpression(expression);

            return document.ReplaceNodeAsync(returnStatement, newReturnStatement, cancellationToken);
        }
    }
}
