// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SwitchSectionCodeFixProvider))]
    [Shared]
    public class SwitchSectionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.RemoveRedundantDefaultSwitchSection,
                    DiagnosticIdentifiers.DefaultLabelShouldBeLastLabelInSwitchSection,
                    DiagnosticIdentifiers.AddBracesToSwitchSectionWithMultipleStatements,
                    DiagnosticIdentifiers.AddBreakStatementToSwitchSection,
                    DiagnosticIdentifiers.MergeSwitchSectionsWithEquivalentContent);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            SwitchSectionSyntax switchSection = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<SwitchSectionSyntax>();

            Debug.Assert(switchSection != null, $"{nameof(switchSection)} is null");

            if (switchSection == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.RemoveRedundantDefaultSwitchSection:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove redundant switch section",
                                cancellationToken => RemoveRedundantDefaultSwitchSectionRefactoring.RefactorAsync(context.Document, switchSection, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.DefaultLabelShouldBeLastLabelInSwitchSection:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Move default label to the last position",
                                cancellationToken => DefaultLabelShouldBeLastLabelInSwitchSectionRefactoring.RefactorAsync(context.Document, switchSection, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.AddBracesToSwitchSectionWithMultipleStatements:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                AddBracesToSwitchSectionRefactoring.Title,
                                cancellationToken => AddBracesToSwitchSectionRefactoring.RefactorAsync(context.Document, switchSection, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.AddBreakStatementToSwitchSection:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Add break;",
                                cancellationToken => AddBreakStatementToSwitchSectionRefactoring.RefactorAsync(context.Document, switchSection, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.MergeSwitchSectionsWithEquivalentContent:
                        {
                            int additionalSections = diagnostic.AdditionalLocations.Count;

                            CodeAction codeAction = CodeAction.Create(
                                "Merge sections",
                                cancellationToken => MergeSwitchSectionsRefactoring.RefactorAsync(context.Document, switchSection, additionalSections, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
