// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.RemoveUsingAliasDirective;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UsingDirectiveRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, UsingDirectiveSyntax usingDirective)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveUsingAliasDirective))
            {
                NameEqualsSyntax alias = usingDirective.Alias;

                if (alias != null)
                {
                    IdentifierNameSyntax name = alias.Name;

                    if (name != null && context.Span.IsContainedInSpanOrBetweenSpans(name))
                    {
                        context.RegisterRefactoring(
                            $"Remove alias '{name.Identifier.ValueText}'",
                            cancellationToken =>
                            {
                                return RemoveUsingAliasDirectiveRefactoring.RefactorAsync(
                                    context.Document,
                                    usingDirective,
                                    cancellationToken);
                            });
                    }
                }
            }
        }
    }
}