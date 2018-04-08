// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CloseParenTokenRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, SyntaxToken closeParen)
        {
            if (closeParen.Kind() != SyntaxKind.CloseParenToken)
                return;

            if (context.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.AddParameterNameToParameter,
                    RefactoringIdentifiers.RenameParameterAccordingToTypeName,
                    RefactoringIdentifiers.CheckParameterForNull,
                    RefactoringIdentifiers.IntroduceAndInitializeField,
                    RefactoringIdentifiers.IntroduceAndInitializeProperty)
                && closeParen.IsParentKind(SyntaxKind.ParameterList)
                && context.Span.Start > 0)
            {
                ParameterSyntax parameter = context.Root
                    .FindNode(new TextSpan(context.Span.Start - 1, 1))?
                    .FirstAncestorOrSelf<ParameterSyntax>();

                if (parameter != null)
                    await ParameterRefactoring.ComputeRefactoringsAsync(context, parameter).ConfigureAwait(false);
            }

            if (closeParen.IsParentKind(SyntaxKind.ArgumentList))
            {
                ArgumentSyntax argument = ((ArgumentListSyntax)closeParen.Parent)
                    .Arguments
                    .FirstOrDefault(f => f.FullSpan.End == closeParen.FullSpan.Start);

                if (argument != null)
                    await ArgumentRefactoring.ComputeRefactoringsAsync(context, argument).ConfigureAwait(false);
            }
        }
    }
}
