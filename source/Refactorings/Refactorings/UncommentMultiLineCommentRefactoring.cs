// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UncommentMultiLineCommentRefactoring
    {
        public static void ComputeRefactoring(
            RefactoringContext context,
            SyntaxTrivia multiLineComment)
        {
            string s = multiLineComment.ToString();

            Debug.Assert(s.StartsWith("/*", StringComparison.Ordinal));
            Debug.Assert(s.EndsWith("*/", StringComparison.Ordinal));

            if (!s.StartsWith("/*", StringComparison.Ordinal))
                return;

            if (!s.EndsWith("*/", StringComparison.Ordinal))
                return;

            context.RegisterRefactoring(
                "Uncomment",
                cancellationToken =>
                {
                    var textChange = new TextChange(multiLineComment.Span, s.Substring(2, s.Length - 4));
                    return context.Document.WithTextChangeAsync(textChange, cancellationToken);
                });
        }
    }
}
