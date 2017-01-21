// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddBreakStatementToSwitchSectionRefactoring
    {
        public static bool CanRefactor(SyntaxNodeAnalysisContext context, SwitchSectionSyntax switchSection)
        {
            SwitchLabelSyntax label = switchSection.Labels.LastOrDefault();

            if (label != null)
            {
                ImmutableArray<Diagnostic> diagnostics = context.SemanticModel.GetDiagnostics(label.Span, context.CancellationToken);

                foreach (Diagnostic diagnostic in diagnostics)
                {
                    switch (diagnostic.Id)
                    {
                        case "CS0163":
                        case "CS8070":
                            return true;
                    }
                }
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            SwitchSectionSyntax switchSection,
            CancellationToken cancellationToken)
        {
            SwitchSectionSyntax newNode = switchSection;

            SyntaxList<StatementSyntax> statements = switchSection.Statements;

            if (statements.Count == 1
                && statements.First().IsKind(SyntaxKind.Block))
            {
                var block = (BlockSyntax)statements.First();
                newNode = newNode.ReplaceNode(block, block.AddStatements(BreakStatement()));
            }
            else
            {
                newNode = switchSection.AddStatements(BreakStatement());
            }

            return await document.ReplaceNodeAsync(switchSection, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}