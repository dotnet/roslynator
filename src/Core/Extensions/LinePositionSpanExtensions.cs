// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.Text;

namespace Roslynator
{
    internal static class LinePositionSpanExtensions
    {
        public static int GetLineCount(this LinePositionSpan linePositionSpan)
        {
            return linePositionSpan.End.Line - linePositionSpan.Start.Line + 1;
        }
    }
}
