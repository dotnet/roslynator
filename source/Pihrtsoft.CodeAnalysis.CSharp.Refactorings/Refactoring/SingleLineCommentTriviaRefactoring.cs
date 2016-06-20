// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class SingleLineCommentTriviaRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, SyntaxTrivia trivia)
        {
            context.RegisterRefactoring(
                "Uncomment",
                cancellationToken => UncommentRefactoring.RefactorAsync(context.Document, trivia, cancellationToken));
        }
    }
}