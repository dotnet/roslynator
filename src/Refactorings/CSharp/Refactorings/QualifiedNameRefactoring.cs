// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class QualifiedNameRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, QualifiedNameSyntax qualifiedName)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddUsingDirective)
                && context.Span.IsEmpty
                && qualifiedName.DotToken.SpanStart == context.Span.Start
                && qualifiedName.Left?.Kind() == SyntaxKind.IdentifierName)
            {
                await AddUsingDirectiveRefactoring.ComputeRefactoringsAsync(context, (IdentifierNameSyntax)qualifiedName.Left).ConfigureAwait(false);
            }
        }
    }
}
