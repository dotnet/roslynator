// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ChangeMethodReturnTypeToVoidRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, MethodDeclarationSyntax methodDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeMethodReturnTypeToVoid)
                && methodDeclaration.ReturnType?.IsVoid() == false
                && methodDeclaration.Body?.Statements.Count > 0
                && !methodDeclaration.IsIterator()
                && context.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                if (!IsAsyncMethodThatReturnsTask(methodDeclaration, semanticModel, context.CancellationToken))
                {
                    ControlFlowAnalysis analysis = semanticModel.AnalyzeControlFlow(methodDeclaration.Body);

                    if (analysis.Succeeded
                        && analysis.ReturnStatements.All(node => IsReturnStatementWithoutExpression(node)))
                    {
                        context.RegisterRefactoring(
                            "Change return type to 'void'",
                            cancellationToken =>
                            {
                                return ChangeTypeRefactoring.ChangeTypeAsync(
                                    context.Document,
                                    methodDeclaration.ReturnType,
                                    CSharpFactory.VoidType(),
                                    cancellationToken);
                            });
                    }
                }
            }
        }

        private static bool IsAsyncMethodThatReturnsTask(
            MethodDeclarationSyntax methodDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            IMethodSymbol methodSymbol = semanticModel
                .GetDeclaredSymbol(methodDeclaration, cancellationToken);

            if (methodSymbol?.IsAsync == true
                && methodSymbol.ReturnType?.IsErrorType() == false)
            {
                INamedTypeSymbol taskSymbol = semanticModel
                    .Compilation
                    .GetTypeByMetadataName("System.Threading.Tasks.Task");

                return methodSymbol.ReturnType.Equals(taskSymbol);
            }

            return false;
        }

        private static bool IsReturnStatementWithoutExpression(SyntaxNode node)
        {
            return node.IsKind(SyntaxKind.ReturnStatement)
                && ((ReturnStatementSyntax)node).Expression == null;
        }
    }
}
