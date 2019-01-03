// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis.RemoveAsyncAwait;

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

                        RemoveAsyncAwaitAnalysis analysis = RemoveAsyncAwaitAnalysis.Create(methodDeclaration, semanticModel, context.CancellationToken);

                        if (analysis.Success)
                            RegisterRefactoring(context, token, analysis);

                        return;
                    }
                case SyntaxKind.LocalFunctionStatement:
                    {
                        var localFunction = (LocalFunctionStatementSyntax)parent;

                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        RemoveAsyncAwaitAnalysis analysis = RemoveAsyncAwaitAnalysis.Create(localFunction, semanticModel, context.CancellationToken);

                        if (analysis.Success)
                            RegisterRefactoring(context, token, analysis);

                        return;
                    }
                case SyntaxKind.ParenthesizedLambdaExpression:
                    {
                        var parenthesizedLambda = (ParenthesizedLambdaExpressionSyntax)parent;

                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        RemoveAsyncAwaitAnalysis analysis = RemoveAsyncAwaitAnalysis.Create(parenthesizedLambda, semanticModel, context.CancellationToken);

                        if (analysis.Success)
                            RegisterRefactoring(context, token, analysis);

                        return;
                    }
                case SyntaxKind.SimpleLambdaExpression:
                    {
                        var simpleLambda = (SimpleLambdaExpressionSyntax)parent;

                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        RemoveAsyncAwaitAnalysis analysis = RemoveAsyncAwaitAnalysis.Create(simpleLambda, semanticModel, context.CancellationToken);

                        if (analysis.Success)
                            RegisterRefactoring(context, token, analysis);

                        return;
                    }
                case SyntaxKind.AnonymousMethodExpression:
                    {
                        var anonymousMethod = (AnonymousMethodExpressionSyntax)parent;

                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        RemoveAsyncAwaitAnalysis analysis = RemoveAsyncAwaitAnalysis.Create(anonymousMethod, semanticModel, context.CancellationToken);

                        if (analysis.Success)
                            RegisterRefactoring(context, token, analysis);

                        return;
                    }
            }
        }

        private static void RegisterRefactoring(RefactoringContext context, SyntaxToken token, in RemoveAsyncAwaitAnalysis analysis)
        {
            CodeAction codeAction = CodeActionFactory.RemoveAsyncAwait(context.Document, token, equivalenceKey: RefactoringIdentifiers.RemoveAsyncAwait);

            context.RegisterRefactoring(codeAction);

            if (analysis.Walker != null)
                RemoveAsyncAwaitWalker.Free(analysis.Walker);
        }
    }
}
