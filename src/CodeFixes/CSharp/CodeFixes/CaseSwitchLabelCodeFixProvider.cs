// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CaseSwitchLabelCodeFixProvider))]
    [Shared]
    public sealed class CaseSwitchLabelCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.CS0152_SwitchStatementContainsMultipleCasesWithSameLabelValue); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            Diagnostic diagnostic = context.Diagnostics[0];

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveSwitchLabel, context.Document, root.SyntaxTree))
                return;

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out CaseSwitchLabelSyntax caseSwitchLabel))
                return;

            if (caseSwitchLabel.Parent.Parent is not SwitchStatementSyntax switchStatement)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Remove duplicate cases",
                ct => RefactorAsync(context.Document, switchStatement, ct),
                EquivalenceKey.Create(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            SwitchStatementSyntax switchStatement,
            CancellationToken cancellationToken)
        {
            var nodesToRemove = new List<SyntaxNode>();
            var labelsToRemove = new List<SyntaxNode>();

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            foreach (SwitchSectionSyntax section in switchStatement.Sections)
            {
                labelsToRemove.Clear();

                SyntaxList<SwitchLabelSyntax> labels = section.Labels;

                foreach (SwitchLabelSyntax label in labels)
                {
                    if (semanticModel.GetDiagnostic(
                        CompilerDiagnosticIdentifiers.CS0152_SwitchStatementContainsMultipleCasesWithSameLabelValue,
                        label.Span,
                        cancellationToken) != null)
                    {
                        labelsToRemove.Add(label);
                    }
                }

                if (labelsToRemove.Count == labels.Count)
                {
                    nodesToRemove.Add(section);
                }
                else
                {
                    nodesToRemove.AddRange(labelsToRemove);
                }
            }

            SwitchStatementSyntax newSwitchStatement = switchStatement
                .RemoveNodes(nodesToRemove, SyntaxRemoveOptions.KeepNoTrivia)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(switchStatement, newSwitchStatement, cancellationToken).ConfigureAwait(false);
        }
    }
}
