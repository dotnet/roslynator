// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemovePropertyInitializerRefactoring
    {
        public static bool CanRefactor(RefactoringContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            return propertyDeclaration.Initializer?.Span.Contains(context.Span) == true;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            PropertyDeclarationSyntax newNode = propertyDeclaration
                .WithInitializer(null)
                .WithSemicolonToken(default(SyntaxToken))
                .WithTriviaFrom(propertyDeclaration)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(propertyDeclaration, newNode, cancellationToken);
        }
    }
}
