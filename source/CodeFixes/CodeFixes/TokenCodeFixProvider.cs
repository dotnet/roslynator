// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Comparers;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TokenCodeFixProvider))]
    [Shared]
    public class TokenCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.OperatorCannotBeAppliedToOperandOfType,
                    CompilerDiagnosticIdentifiers.PartialModifierCanOnlyAppearImmediatelyBeforeClassStructInterfaceOrVoid);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsAnyCodeFixEnabled(
                CodeFixIdentifiers.AddArgumentList,
                CodeFixIdentifiers.ReorderModifiers))
            {
                return;
            }

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindToken(root, context.Span.Start, out SyntaxToken token))
                return;

            SyntaxKind kind = token.Kind();

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.OperatorCannotBeAppliedToOperandOfType:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddArgumentList))
                                break;

                            if (kind != SyntaxKind.QuestionToken)
                                break;

                            if (!token.IsParentKind(SyntaxKind.ConditionalAccessExpression))
                                break;

                            var conditionalAccess = (ConditionalAccessExpressionSyntax)token.Parent;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(conditionalAccess.Expression, context.CancellationToken);

                            if (typeSymbol == null
                                || typeSymbol.IsErrorType()
                                || !typeSymbol.IsValueType
                                || typeSymbol.IsConstructedFrom(SpecialType.System_Nullable_T))
                            {
                                break;
                            }

                            CodeAction codeAction = CodeAction.Create(
                                "Remove '?' operator",
                                cancellationToken =>
                                {
                                    var textChange = new TextChange(token.Span, "");
                                    return context.Document.WithTextChangeAsync(textChange, cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.PartialModifierCanOnlyAppearImmediatelyBeforeClassStructInterfaceOrVoid:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.ReorderModifiers))
                                break;

                            ModifiersCodeFixes.MoveModifier(context, diagnostic, token.Parent, token);
                            break;
                        }
                }
            }
        }
    }
}
