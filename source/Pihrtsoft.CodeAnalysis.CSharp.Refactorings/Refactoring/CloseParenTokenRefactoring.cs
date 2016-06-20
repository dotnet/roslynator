// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class CloseParenTokenRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, SyntaxToken closeParen)
        {
            if (closeParen.IsKind(SyntaxKind.CloseParenToken)
                && closeParen.Parent?.IsKind(SyntaxKind.ParameterList) == true
                && context.Span.Start > 0
                && context.SupportsSemanticModel)
            {
                ParameterSyntax parameter = context.Root
                    .FindNode(new TextSpan(context.Span.Start - 1, 1))?
                    .FirstAncestorOrSelf<ParameterSyntax>();

                if (parameter != null)
                {
                    await RenameParameterAccordingToTypeNameRefactoring.RefactorAsync(context, parameter);

                    await AddParameterNullCheckRefactoring.RefactorAsync(context, parameter);
                }
            }
        }
    }
}
