// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveAsyncAwaitRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, SyntaxToken token)
        {
            switch (token.Parent)
            {
                case MethodDeclarationSyntax methodDeclaration:
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        using (RemoveAsyncAwaitAnalysis analysis = RemoveAsyncAwaitAnalysis.Create(methodDeclaration, semanticModel, context.CancellationToken))
                        {
                            if (analysis.Success)
                                RegisterRefactoring();
                        }

                        return;
                    }
                case LocalFunctionStatementSyntax localFunction:
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        using (RemoveAsyncAwaitAnalysis analysis = RemoveAsyncAwaitAnalysis.Create(localFunction, semanticModel, context.CancellationToken))
                        {
                            if (analysis.Success)
                                RegisterRefactoring();
                        }

                        return;
                    }
                case ParenthesizedLambdaExpressionSyntax parenthesizedLambda:
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        using (RemoveAsyncAwaitAnalysis analysis = RemoveAsyncAwaitAnalysis.Create(parenthesizedLambda, semanticModel, context.CancellationToken))
                        {
                            if (analysis.Success)
                                RegisterRefactoring();
                        }

                        return;
                    }
                case SimpleLambdaExpressionSyntax simpleLambda:
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        using (RemoveAsyncAwaitAnalysis analysis = RemoveAsyncAwaitAnalysis.Create(simpleLambda, semanticModel, context.CancellationToken))
                        {
                            if (analysis.Success)
                                RegisterRefactoring();
                        }

                        return;
                    }
                case AnonymousMethodExpressionSyntax anonymousMethod:
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        using (RemoveAsyncAwaitAnalysis analysis = RemoveAsyncAwaitAnalysis.Create(anonymousMethod, semanticModel, context.CancellationToken))
                        {
                            if (analysis.Success)
                                RegisterRefactoring();
                        }

                        return;
                    }
            }

            void RegisterRefactoring()
            {
                CodeAction codeAction = CodeActionFactory.RemoveAsyncAwait(context.Document, token, equivalenceKey: RefactoringIdentifiers.RemoveAsyncAwait);

                context.RegisterRefactoring(codeAction);
            }
        }
    }
}
