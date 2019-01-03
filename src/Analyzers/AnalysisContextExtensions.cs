// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator
{
    internal static class AnalysisContextExtensions
    {
        internal static void ThrowIfCancellationRequested(this SyntaxNodeAnalysisContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
        }

        internal static void ThrowIfCancellationRequested(this SyntaxTreeAnalysisContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
        }

        internal static void ThrowIfCancellationRequested(this SymbolAnalysisContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
        }
    }
}
