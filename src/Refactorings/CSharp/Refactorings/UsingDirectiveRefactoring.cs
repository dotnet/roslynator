// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.InlineAliasExpression))
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
                            ct => InlineAliasExpressionRefactoring.RefactorAsync(context.Document, usingDirective, ct));
                    }
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.InlineUsingStatic)
                && usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword)
                && usingDirective.IsParentKind(SyntaxKind.CompilationUnit, SyntaxKind.NamespaceDeclaration))
            {
                context.RegisterRefactoring(
                    "Inline using static",
                    ct => InlineUsingStaticRefactoring.RefactorAsync(context.Document, usingDirective, ct));
            }
        }
    }
}