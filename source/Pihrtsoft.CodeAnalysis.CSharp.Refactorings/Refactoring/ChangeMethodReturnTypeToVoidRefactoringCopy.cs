// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ChangeMethodReturnTypeToVoidRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, MethodDeclarationSyntax methodDeclaration)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ChangeMethodReturnTypeToVoid)
                && methodDeclaration.ReturnType?.IsVoid() == false
                && methodDeclaration.Body?.IsMissing == false
                && context.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync();

                ControlFlowAnalysis analysis = semanticModel.AnalyzeControlFlow(methodDeclaration.Body);

                if (analysis.Succeeded
                    && analysis.ReturnStatements.All(node => IsReturnStatementWithoutExpression(node)))
                {
                    context.RegisterRefactoring(
                        "Change method's return type to 'void'",
                        cancellationToken =>
                        {
                            return ChangeMemberTypeRefactoring.RefactorAsync(
                                context.Document,
                                methodDeclaration.ReturnType,
                                CSharpFactory.VoidType(),
                                cancellationToken);
                        });
                }
            }
        }

        private static bool IsReturnStatementWithoutExpression(SyntaxNode node)
        {
            return node.IsKind(SyntaxKind.ReturnStatement)
                && ((ReturnStatementSyntax)node).Expression == null;
        }
    }
}
