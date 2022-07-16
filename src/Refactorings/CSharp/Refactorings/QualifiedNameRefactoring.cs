// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class QualifiedNameRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, QualifiedNameSyntax qualifiedName)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.AddUsingDirective)
                && context.Span.IsEmpty
                && qualifiedName.DotToken.SpanStart == context.Span.Start)
            {
                IdentifierNameSyntax identifierName = qualifiedName.Left as IdentifierNameSyntax
                    ?? qualifiedName.Right as IdentifierNameSyntax;

                if (identifierName != null)
                    await AddUsingDirectiveRefactoring.ComputeRefactoringsAsync(context, identifierName).ConfigureAwait(false);
            }
        }
    }
}
