// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    public abstract class UrlSegmentProvider
    {
        public abstract ImmutableArray<string> GetSegments(ISymbol symbol);
    }
}
