// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.DocumentationComment;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ParameterCodeFixProvider))]
    [Shared]
    public class ParameterCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.AddParameterToDocumentationComment,
                    DiagnosticIdentifiers.OverridingMemberCannotChangeParamsModifier);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out ParameterSyntax parameter))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.AddParameterToDocumentationComment:
                        {
                            var refactoring = new AddParameterToDocumentationCommentRefactoring();

                            CodeAction codeAction = CodeAction.Create(
                                "Add parameter to documentation comment",
                                cancellationToken => refactoring.RefactorAsync(context.Document, parameter, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.OverridingMemberCannotChangeParamsModifier:
                        {
                            if (parameter.IsParams())
                            {
                                ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, parameter, SyntaxKind.ParamsKeyword);
                            }
                            else
                            {
                                ModifiersCodeFixRegistrator.AddModifier(context, diagnostic, parameter, SyntaxKind.ParamsKeyword);
                            }

                            break;
                        }
                }
            }
        }
    }
}
