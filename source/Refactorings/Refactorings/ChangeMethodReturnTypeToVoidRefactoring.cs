// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ChangeMethodReturnTypeToVoidRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, MethodDeclarationSyntax methodDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeMethodReturnTypeToVoid))
            {
                TypeSyntax returnType = methodDeclaration.ReturnType;

                if (returnType?.IsVoid() == false)
                {
                    BlockSyntax body = methodDeclaration.Body;

                    if (body?.Statements.Count > 0
                        && !methodDeclaration.IsIterator())
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

                        if (methodSymbol?.IsOverride == false
                            && !methodSymbol.ImplementsInterfaceMember()
                            && !IsAsyncMethodThatReturnsTask(methodSymbol, semanticModel))
                        {
                            ControlFlowAnalysis analysis = semanticModel.AnalyzeControlFlow(body);

                            if (analysis.Succeeded
                                && analysis.ReturnStatements.All(node => IsReturnStatementWithoutExpression(node)))
                            {
                                context.RegisterRefactoring(
                                    "Change return type to 'void'",
                                    cancellationToken =>
                                    {
                                        return ChangeTypeRefactoring.ChangeTypeAsync(
                                            context.Document,
                                            returnType,
                                            CSharpFactory.VoidType(),
                                            cancellationToken);
                                    });
                            }
                        }
                    }
                }
            }
        }

        private static bool IsAsyncMethodThatReturnsTask(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            if (methodSymbol.IsAsync)
            {
                ITypeSymbol returnType = methodSymbol.ReturnType;

                return returnType?.IsErrorType() == false
                    && returnType.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task));
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
