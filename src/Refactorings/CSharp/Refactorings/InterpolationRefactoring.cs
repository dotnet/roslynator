// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InterpolationRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, InterpolationSyntax interpolation)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveInterpolation)
                && context.Span.IsContainedInSpan(interpolation.OpenBraceToken, interpolation.CloseBraceToken))
            {
                context.RegisterRefactoring("Remove interpolation",
                    cancellationToken => context.Document.RemoveNodeAsync(interpolation, SyntaxRemoveOptions.KeepUnbalancedDirectives, cancellationToken));
            }
        }
    }
}