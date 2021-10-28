// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal readonly struct CommonFixContext
    {
        public CommonFixContext(
            Document document,
            string equivalenceKey,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            Document = document;
            EquivalenceKey = equivalenceKey;
            SemanticModel = semanticModel;
            CancellationToken = cancellationToken;
        }

        public Document Document { get; }

        public string EquivalenceKey { get; }

        public SemanticModel SemanticModel { get; }

        public CancellationToken CancellationToken { get; }
    }
}
