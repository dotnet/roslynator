// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Roslynator.CodeMetrics;

namespace Roslynator.CSharp.CodeMetrics
{
    internal abstract class CSharpCodeMetricsCounter : CodeMetricsCounter
    {
        internal override SyntaxFactsService SyntaxFacts => CSharpSyntaxFactsService.Instance;
    }
}
