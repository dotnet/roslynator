// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class CommaTokenRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, SyntaxToken commaToken)
        {
            if (!commaToken.IsKind(SyntaxKind.CommaToken))
                return;

            if (context.Settings.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.AddParameterNameToParameter,
                    RefactoringIdentifiers.RenameParameterAccordingToTypeName,
                    RefactoringIdentifiers.CheckParameterForNull)
                && commaToken.Parent?.IsKind(SyntaxKind.ParameterList) == true
                && context.Span.Start > 0)
            {
                ParameterSyntax parameter = context.Root
                    .FindNode(new TextSpan(context.Span.Start - 1, 1))?
                    .FirstAncestorOrSelf<ParameterSyntax>();

                if (parameter != null)
                    await ParameterRefactoring.ComputeRefactoringsAsync(context, parameter);
            }

            if (commaToken.Parent?.IsKind(SyntaxKind.ArgumentList) == true)
            {
                ArgumentSyntax argument = ((ArgumentListSyntax)commaToken.Parent)
                    .Arguments
                    .FirstOrDefault(f => f.FullSpan.End == commaToken.FullSpan.Start);

                if (argument != null)
                    await ArgumentRefactoring.ComputeRefactoringsAsync(context, argument);
            }
        }
    }
}