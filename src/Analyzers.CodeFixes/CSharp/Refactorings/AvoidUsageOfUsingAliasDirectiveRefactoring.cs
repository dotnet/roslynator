// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.InlineAliasExpression;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AvoidUsageOfUsingAliasDirectiveRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            UsingDirectiveSyntax usingDirective,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return InlineAliasExpressionRefactoring.RefactorAsync(document, usingDirective, cancellationToken);
        }
    }
}
