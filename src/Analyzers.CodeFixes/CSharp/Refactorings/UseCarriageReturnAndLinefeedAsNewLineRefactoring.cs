// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseCarriageReturnAndLinefeedAsNewLineRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return document.WithTextChangeAsync(new TextChange(span, "\r\n"), cancellationToken);
        }
    }
}
