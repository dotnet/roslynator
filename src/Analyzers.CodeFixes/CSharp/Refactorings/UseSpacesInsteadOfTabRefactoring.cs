// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseSpacesInsteadOfTabRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            var textChange = new TextChange(span, new string(' ', span.Length * 4));

            SourceText newSourceText = sourceText.WithChanges(textChange);

            return document.WithText(newSourceText);
        }
    }
}
