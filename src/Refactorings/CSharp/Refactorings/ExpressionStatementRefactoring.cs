// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExpressionStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ExpressionStatementSyntax expressionStatement)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddIdentifierToVariableDeclaration))
                await AddIdentifierToLocalDeclarationRefactoring.ComputeRefactoringAsync(context, expressionStatement).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.IntroduceLocalVariable))
            {
                ExpressionSyntax expression = expressionStatement.Expression;

                if (expression?.IsMissing == false
                    && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(expression)
                    && !expressionStatement.IsEmbedded()
                    && !(expression is AssignmentExpressionSyntax)
                    && !CSharpFacts.IsIncrementOrDecrementExpression(expression.Kind()))
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    if (semanticModel.GetSymbol(expression, context.CancellationToken)?.IsErrorType() == false)
                    {
                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

                        if (typeSymbol?.IsErrorType() == false
                            && !typeSymbol.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task))
                            && !typeSymbol.IsVoid())
                        {
                            bool addAwait = false;

                            if (typeSymbol.OriginalDefinition.EqualsOrInheritsFromTaskOfT(semanticModel))
                            {
                                ISymbol enclosingSymbol = semanticModel.GetEnclosingSymbol(expressionStatement.SpanStart, context.CancellationToken);

                                addAwait = enclosingSymbol.IsAsyncMethod();
                            }

                            context.RegisterRefactoring(
                                IntroduceLocalVariableRefactoring.GetTitle(expression),
                                cancellationToken => IntroduceLocalVariableRefactoring.RefactorAsync(context.Document, expressionStatement, typeSymbol, addAwait, semanticModel, cancellationToken));
                        }
                    }
                }
            }
        }
    }
}