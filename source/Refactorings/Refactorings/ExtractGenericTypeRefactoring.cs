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
            TypeArgumentListSyntax typeArgumentList = genericName.TypeArgumentList;

            if (typeArgumentList != null)
            {
                SeparatedSyntaxList<TypeSyntax> arguments = typeArgumentList.Arguments;

                if (arguments.Count == 1
                    && context.Span.IsBetweenSpans(arguments[0]))
                {
                    return true;
                }
            }

            return false;
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
