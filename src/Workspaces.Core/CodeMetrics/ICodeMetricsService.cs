// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CodeMetrics
{
    internal interface ICodeMetricsService : ILanguageService
    {
        ISyntaxFactsService SyntaxFacts { get; }

        CodeMetricsInfo CountPhysicalLines(SyntaxNode node, SourceText sourceText, CodeMetricsOptions options, CancellationToken cancellationToken);

        CodeMetricsInfo CountLogicalLines(SyntaxNode node, SourceText sourceText, CodeMetricsOptions options, CancellationToken cancellationToken);
    }
}
