// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
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
            TypeSyntax returnType = methodDeclaration.ReturnType;

            if (returnType?.IsVoid() != false)
                return;

            BlockSyntax body = methodDeclaration.Body;

            if (body == null)
                return;

            SyntaxList<StatementSyntax> statements = body.Statements;

            if (!statements.Any())
                return;

            if (statements.SingleOrDefault(shouldThrow: false)?.Kind() == SyntaxKind.ThrowStatement)
                return;

            if (methodDeclaration.ContainsYield())
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

            ComputeRefactoring(context, methodSymbol, semanticModel, body, returnType);
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, LocalFunctionStatementSyntax localFunction)
        {
            TypeSyntax returnType = localFunction.ReturnType;

            if (returnType?.IsVoid() != false)
                return;

            BlockSyntax body = localFunction.Body;

            if (body == null)
                return;

            SyntaxList<StatementSyntax> statements = body.Statements;

            if (!statements.Any())
                return;

            if (statements.SingleOrDefault(shouldThrow: false)?.Kind() == SyntaxKind.ThrowStatement)
                return;

            if (localFunction.ContainsYield())
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(localFunction, context.CancellationToken);

            ComputeRefactoring(context, methodSymbol, semanticModel, body, returnType);
        }

        private static void ComputeRefactoring(
            RefactoringContext context,
            IMethodSymbol methodSymbol,
            SemanticModel semanticModel,
            BlockSyntax body,
            TypeSyntax returnType)
        {
            if (methodSymbol?.IsOverride != false)
                return;

            if (methodSymbol.ImplementsInterfaceMember())
                return;

            if (methodSymbol.IsAsync
                && methodSymbol.ReturnType.HasMetadataName(MetadataNames.System_Threading_Tasks_Task))
            {
                return;
            }

            ControlFlowAnalysis analysis = semanticModel.AnalyzeControlFlow(body);

            if (!analysis.Succeeded)
                return;

            if (!analysis.ReturnStatements.All(f => (f as ReturnStatementSyntax)?.Expression == null))
                return;

            context.RegisterRefactoring(
                "Change return type to 'void'",
                ct => ChangeTypeRefactoring.ChangeTypeAsync(context.Document, returnType, CSharpFactory.VoidType(), ct),
                RefactoringIdentifiers.ChangeMethodReturnTypeToVoid);
        }
    }
}
