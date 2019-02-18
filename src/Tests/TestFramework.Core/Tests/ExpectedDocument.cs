// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.Tests
{
    internal readonly struct ExpectedDocument
    {
        public ExpectedDocument(DocumentId id, string text)
        {
            Id = id;
            Text = text;
        }

        public DocumentId Id { get; }

        public string Text { get; }
    }
}
