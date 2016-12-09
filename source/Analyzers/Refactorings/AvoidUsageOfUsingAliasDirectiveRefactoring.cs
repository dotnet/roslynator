// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Refactorings.InlineAliasExpression;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AvoidUsageOfUsingAliasDirectiveRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, UsingDirectiveSyntax usingDirective)
        {
            if (usingDirective.Alias != null)
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.AvoidUsageOfUsingAliasDirective,
                    usingDirective.GetLocation());
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            UsingDirectiveSyntax usingDirective,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await InlineAliasExpressionSyntaxRewriter.VisitAsync(document, usingDirective, cancellationToken).ConfigureAwait(false);
        }
    }
}
