// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis.RemoveAsyncAwait;
using Roslynator.CSharp.CodeFixes;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveAsyncAwaitRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, SyntaxToken token)
        {
            SyntaxNode parent = token.Parent;

            switch (parent.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)parent;

                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        RemoveAsyncAwaitResult result = RemoveAsyncAwaitAnalysis.Analyze(methodDeclaration, semanticModel, context.CancellationToken);

                        ComputeRefactoring(context, token, result);

                        return;
                    }
                case SyntaxKind.LocalFunctionStatement:
                    {
                        var localFunction = (LocalFunctionStatementSyntax)parent;

                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        RemoveAsyncAwaitResult result = RemoveAsyncAwaitAnalysis.Analyze(localFunction, semanticModel, context.CancellationToken);

                        ComputeRefactoring(context, token, result);

                        return;
                    }
                case SyntaxKind.ParenthesizedLambdaExpression:
                    {
                        var parenthesizedLambda = (ParenthesizedLambdaExpressionSyntax)parent;

                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        RemoveAsyncAwaitResult result = RemoveAsyncAwaitAnalysis.Analyze(parenthesizedLambda, semanticModel, context.CancellationToken);

                        ComputeRefactoring(context, token, result);

                        return;
                    }
                case SyntaxKind.SimpleLambdaExpression:
                    {
                        var simpleLambda = (SimpleLambdaExpressionSyntax)parent;

                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        RemoveAsyncAwaitResult result = RemoveAsyncAwaitAnalysis.Analyze(simpleLambda, semanticModel, context.CancellationToken);

                        ComputeRefactoring(context, token, result);

                        return;
                    }
                case SyntaxKind.AnonymousMethodExpression:
                    {
                        var anonymousMethod = (AnonymousMethodExpressionSyntax)parent;

                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        RemoveAsyncAwaitResult result = RemoveAsyncAwaitAnalysis.Analyze(anonymousMethod, semanticModel, context.CancellationToken);

                        ComputeRefactoring(context, token, result);

                        return;
                    }
            }
        }

        private static void ComputeRefactoring(RefactoringContext context, SyntaxToken token, in RemoveAsyncAwaitResult result)
        {
            if (result.Success)
            {
                context.RegisterRefactoring(
                    "Remove async/await",
                    ct => RemoveAsyncAwaitCodeFix.RefactorAsync(context.Document, token, ct),
                    RefactoringIdentifiers.RemoveAsyncAwait);

                if (result.Walker != null)
                    RemoveAsyncAwaitWalker.Free(result.Walker);
            }
        }
    }
}
