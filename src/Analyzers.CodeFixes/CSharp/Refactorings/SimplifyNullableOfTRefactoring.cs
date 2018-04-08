// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyNullableOfTRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            TypeSyntax type,
            TypeSyntax nullableType,
            CancellationToken cancellationToken)
        {
            TypeSyntax newType = NullableType(nullableType.WithoutTrivia(), QuestionToken())
                .WithTriviaFrom(type)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(type, newType, cancellationToken);
        }
    }
}
