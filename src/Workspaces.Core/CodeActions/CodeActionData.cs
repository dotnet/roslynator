// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;

namespace Roslynator.CodeActions
{
    internal readonly struct CodeActionData
    {
        public CodeActionData(string title, string equivalenceKey = null)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            EquivalenceKey = equivalenceKey;
        }

        public string Title { get; }

        public string EquivalenceKey { get; }

        public bool IsDefault => Title == null;

        public CodeAction ToCodeAction(Func<CancellationToken, Task<Document>> createChangedDocument)
        {
            return CodeAction.Create(Title, createChangedDocument, EquivalenceKey);
        }

        public CodeAction ToCodeAction(Func<CancellationToken, Task<Solution>> createChangedDocument)
        {
            return CodeAction.Create(Title, createChangedDocument, EquivalenceKey);
        }
    }
}
