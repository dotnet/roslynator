// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.DiagnosticAnalyzers;
using Roslynator.CSharp.Refactorings;
using Roslynator.CSharp.Refactorings.If;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IfStatementCodeFixProvider))]
    [Shared]
    public class IfStatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.MergeIfStatementWithNestedIfStatement,
                    DiagnosticIdentifiers.ReplaceIfStatementWithAssignment,
                    DiagnosticIdentifiers.UseCoalesceExpressionInsteadOfIf);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            IfStatementSyntax ifStatement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<IfStatementSyntax>();

            Debug.Assert(ifStatement != null, $"{nameof(ifStatement)} is null");

            if (ifStatement == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.MergeIfStatementWithNestedIfStatement:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Merge if with nested if",
                                cancellationToken =>
                                {
                                    return MergeIfStatementWithNestedIfStatementRefactoring.RefactorAsync(
                                        context.Document,
                                        ifStatement,
                                        cancellationToken);
                                },
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.ReplaceIfStatementWithAssignment:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Replace if with assignment",
                                cancellationToken =>
                                {
                                    return ReplaceIfStatementWithAssignmentRefactoring.RefactorAsync(
                                        context.Document,
                                        ifStatement,
                                        cancellationToken);
                                },
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UseCoalesceExpressionInsteadOfIf:
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            IfRefactoring refactoring = IfRefactoring.Analyze(
                                ifStatement,
                                UseCoalesceExpressionInsteadOfIfDiagnosticAnalyzer.AnalysisOptions,
                                semanticModel,
                                context.CancellationToken).First();

                            CodeAction codeAction = CodeAction.Create(
                                refactoring.Title,
                                cancellationToken => refactoring.RefactorAsync(context.Document, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
