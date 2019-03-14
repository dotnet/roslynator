// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class ExternalUrlProvider
    {
        public abstract string Name { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => Name;

        public abstract DocumentationUrlInfo CreateUrl(ISymbol symbol);

        public abstract DocumentationUrlInfo CreateUrl(ImmutableArray<string> folders);
    }
}
