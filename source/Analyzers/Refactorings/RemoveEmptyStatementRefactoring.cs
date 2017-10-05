// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveEmptyStatementRefactoring
    {
        public static void AnalyzeEmptyStatement(SyntaxNodeAnalysisContext context)
        {
            SyntaxNode emptyStatement = context.Node;

            SyntaxNode parent = emptyStatement.Parent;

            if (parent == null)
                return;

            SyntaxKind kind = parent.Kind();

            if (kind == SyntaxKind.LabeledStatement)
                return;

            if (kind.CanContainEmbeddedStatement())
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.RemoveEmptyStatement, emptyStatement);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            EmptyStatementSyntax emptyStatement,
            CancellationToken cancellationToken)
        {
            return document.RemoveNodeAsync(emptyStatement, SyntaxRemoveOptions.KeepExteriorTrivia, cancellationToken);
        }
    }
}
