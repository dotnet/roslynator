// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal readonly struct DocumentReferenceInfo
    {
        public DocumentReferenceInfo(Document document, SyntaxNode root, ImmutableArray<SyntaxNode> references)
        {
            Document = document;
            Root = root;
            References = references;
        }

        public static DocumentReferenceInfo Default { get; } = new DocumentReferenceInfo(null, null, ImmutableArray<SyntaxNode>.Empty);

        public Document Document { get; }

        public SyntaxNode Root { get; }

        public ImmutableArray<SyntaxNode> References { get; }

        public SyntaxTree SyntaxTree
        {
            get { return Root?.SyntaxTree; }
        }
    }
}
