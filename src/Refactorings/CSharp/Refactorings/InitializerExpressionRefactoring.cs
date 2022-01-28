// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InitializerExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, InitializerExpressionSyntax initializer)
        {
            if (initializer.IsKind(SyntaxKind.ComplexElementInitializerExpression)
                && initializer.IsParentKind(SyntaxKind.CollectionInitializerExpression))
            {
                initializer = (InitializerExpressionSyntax)initializer.Parent;
            }

            if (context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(initializer)
                || context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(initializer.Expressions))
            {
                SeparatedSyntaxList<ExpressionSyntax> expressions = initializer.Expressions;

                if (context.IsRefactoringEnabled(RefactoringDescriptors.WrapInitializerExpressions)
                    && expressions.Any()
                    && !initializer.IsKind(SyntaxKind.ComplexElementInitializerExpression)
                    && initializer.IsParentKind(
                        SyntaxKind.ArrayCreationExpression,
                        SyntaxKind.ImplicitArrayCreationExpression,
                        SyntaxKind.ObjectCreationExpression,
                        SyntaxKind.CollectionInitializerExpression,
                        SyntaxKind.WithExpression))
                {
                    if (initializer.IsSingleLine(includeExteriorTrivia: false))
                    {
                        context.RegisterRefactoring(
                            "Wrap initializer expression",
                            ct => SyntaxFormatter.ToMultiLineAsync(context.Document, initializer, ct),
                            RefactoringDescriptors.WrapInitializerExpressions);
                    }
                    else if (expressions.All(expression => expression.IsSingleLine())
                        && initializer.DescendantTrivia(initializer.Span).All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                    {
                        context.RegisterRefactoring(
                            "Unwrap initializer expressions",
                            ct => SyntaxFormatter.ToSingleLineAsync(
                                context.Document,
                                initializer.Parent,
                                TextSpan.FromBounds(initializer.OpenBraceToken.GetPreviousToken().Span.End, initializer.CloseBraceToken.Span.End),
                                ct),
                            RefactoringDescriptors.WrapInitializerExpressions);
                    }
                }

                if (context.IsRefactoringEnabled(RefactoringDescriptors.AddAllPropertiesToInitializer)
                    && initializer.IsKind(SyntaxKind.ObjectInitializerExpression, SyntaxKind.WithInitializerExpression)
                    && AddAllPropertiesToInitializerRefactoring.IsApplicableSpan(initializer, context.Span))
                {
                    SemanticModel semanticModdel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    AddAllPropertiesToInitializerRefactoring.ComputeRefactorings(context, initializer, semanticModdel);
                }

                await ExpandInitializerRefactoring.ComputeRefactoringsAsync(context, initializer).ConfigureAwait(false);

                if (context.IsRefactoringEnabled(RefactoringDescriptors.UseIndexInitializer)
                    && context.SupportsCSharp6)
                {
                    await UseIndexInitializerRefactoring.ComputeRefactoringAsync(context, initializer).ConfigureAwait(false);
                }
            }
        }
    }
}
