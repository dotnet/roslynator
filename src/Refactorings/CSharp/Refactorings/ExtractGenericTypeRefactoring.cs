// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExtractGenericTypeRefactoring
    {
        public static bool CanRefactor(RefactoringContext context, GenericNameSyntax genericName)
        {
            TypeSyntax typeArgument = genericName.TypeArgumentList?.Arguments.SingleOrDefault(shouldThrow: false);

            return typeArgument != null
                && context.Span.IsBetweenSpans(typeArgument);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            GenericNameSyntax genericName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            TypeSyntax typeSyntax = genericName
                .TypeArgumentList
                .Arguments[0]
                .WithTriviaFrom(genericName);

            return document.ReplaceNodeAsync(genericName, typeSyntax, cancellationToken);
        }
    }
}
