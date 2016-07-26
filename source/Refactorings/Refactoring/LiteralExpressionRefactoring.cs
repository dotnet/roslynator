// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class LiteralExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, LiteralExpressionSyntax literalExpression)
        {
            switch (literalExpression.Kind())
            {
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                    {
                        if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.NegateBooleanLiteral)
                            && literalExpression.Span.Contains(context.Span))
                        {
                            context.RegisterRefactoring(
                                "Negate boolean literal",
                                cancellationToken => NegateBooleanLiteralRefactoring.RefactorAsync(context.Document, literalExpression, cancellationToken));
                        }

                        break;
                    }
                case SyntaxKind.StringLiteralExpression:
                    {
                        StringLiteralExpressionRefactoring.ComputeRefactorings(context, literalExpression);
                        break;
                    }
            }
        }
    }
}