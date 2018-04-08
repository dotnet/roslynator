// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveEmptyRegionRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            RegionInfo region,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return document.RemoveRegionAsync(region, cancellationToken);
        }
    }
}
