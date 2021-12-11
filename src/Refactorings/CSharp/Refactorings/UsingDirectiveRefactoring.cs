// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.InlineAliasExpression;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UsingDirectiveRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, UsingDirectiveSyntax usingDirective)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.InlineAliasExpression))
            {
                NameEqualsSyntax alias = usingDirective.Alias;

                if (alias != null)
                {
                    IdentifierNameSyntax name = alias.Name;

                    if (name != null
                        && context.Span.IsContainedInSpanOrBetweenSpans(name))
                    {
                        context.RegisterRefactoring(
                            "Inline alias expression",
                            ct => InlineAliasExpressionRefactoring.RefactorAsync(context.Document, usingDirective, ct),
                            RefactoringDescriptors.InlineAliasExpression);
                    }
                }
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.InlineUsingStaticDirective)
                && usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword)
                && usingDirective.IsParentKind(SyntaxKind.CompilationUnit, SyntaxKind.NamespaceDeclaration))
            {
                context.RegisterRefactoring(
                    "Inline using static",
                    ct => InlineUsingStaticDirectiveRefactoring.RefactorAsync(context.Document, usingDirective, ct),
                    RefactoringDescriptors.InlineUsingStaticDirective);
            }
        }
    }
}
