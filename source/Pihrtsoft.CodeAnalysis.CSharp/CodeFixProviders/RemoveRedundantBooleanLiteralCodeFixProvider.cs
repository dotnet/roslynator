// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AttributeArgumentListCodeFixProvider))]
    [Shared]
    public class RemoveRedundantBooleanLiteralCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.RemoveRedundantBooleanLiteral,
                    DiagnosticIdentifiers.UseLogicalNotOperator);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            LiteralExpressionSyntax literalExpression = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<LiteralExpressionSyntax>();

            if (literalExpression == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case   DiagnosticIdentifiers.RemoveRedundantBooleanLiteral:
                        {
                            if (RemoveRedundantBooleanLiteralRefactoring.CanRefactor(literalExpression))
                            {
                                CodeAction codeAction = CodeAction.Create(
                                    "Remove redundant boolean literal",
                                    cancellationToken => RemoveRedundantBooleanLiteralRefactoring.RefactorAsync(context.Document, literalExpression, cancellationToken),
                                    diagnostic.Id + EquivalenceKeySuffix);

                                context.RegisterCodeFix(codeAction, diagnostic);
                            }

                            break;
                        }
                    case  DiagnosticIdentifiers.UseLogicalNotOperator:
                        {
                            if (UseLogicalNotOperatorRefactoring.CanRefactor(literalExpression))
                            {
                                CodeAction codeAction = CodeAction.Create(
                                    "Use '!' operator",
                                    cancellationToken => UseLogicalNotOperatorRefactoring.RefactorAsync(context.Document, literalExpression, cancellationToken),
                                    diagnostic.Id + EquivalenceKeySuffix);

                                context.RegisterCodeFix(codeAction, diagnostic);
                            }

                            break;
                        }
                }
            }
        }
    }
}