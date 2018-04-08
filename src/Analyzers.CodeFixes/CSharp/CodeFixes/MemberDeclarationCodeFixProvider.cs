// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MemberDeclarationCodeFixProvider))]
    [Shared]
    public class MemberDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.FormatDeclarationBraces,
                    DiagnosticIdentifiers.RemoveRedundantOverridingMember,
                    DiagnosticIdentifiers.AddDefaultAccessModifier,
                    DiagnosticIdentifiers.AddEmptyLineBetweenDeclarations,
                    DiagnosticIdentifiers.RemoveRedundantSealedModifier,
                    DiagnosticIdentifiers.AvoidSemicolonAtEndOfDeclaration,
                    DiagnosticIdentifiers.ReorderModifiers,
                    DiagnosticIdentifiers.MarkFieldAsReadOnly,
                    DiagnosticIdentifiers.UseConstantInsteadOfField,
                    DiagnosticIdentifiers.UseReadOnlyAutoProperty,
                    DiagnosticIdentifiers.ReplaceCommentWithDocumentationComment);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out MemberDeclarationSyntax memberDeclaration))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.FormatDeclarationBraces:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Format braces",
                                cancellationToken => FormatDeclarationBracesRefactoring.RefactorAsync(context.Document, memberDeclaration, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.RemoveRedundantOverridingMember:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                $"Remove {CSharpFacts.GetTitle(memberDeclaration)}",
                                cancellationToken => context.Document.RemoveMemberAsync(memberDeclaration, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.AddDefaultAccessModifier:
                        {
                            var accessibility = (Accessibility)Enum.Parse(
                                typeof(Accessibility),
                                diagnostic.Properties[nameof(Accessibility)]);

                            CodeAction codeAction = CodeAction.Create(
                                "Add default access modifier",
                                cancellationToken => AddDefaultAccessModifierRefactoring.RefactorAsync(context.Document, memberDeclaration, accessibility, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.AddEmptyLineBetweenDeclarations:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Add empty line",
                                cancellationToken => AddEmptyLineBetweenDeclarationsRefactoring.RefactorAsync(context.Document, memberDeclaration, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.RemoveRedundantSealedModifier:
                        {
                            ModifiersCodeFixRegistrator.RemoveModifier(context, diagnostic, memberDeclaration, SyntaxKind.SealedKeyword);
                            break;
                        }
                    case DiagnosticIdentifiers.AvoidSemicolonAtEndOfDeclaration:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove unnecessary semicolon",
                                cancellationToken => AvoidSemicolonAtEndOfDeclarationRefactoring.RefactorAsync(context.Document, memberDeclaration, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.ReorderModifiers:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Reorder modifiers",
                                cancellationToken => ReorderModifiersRefactoring.RefactorAsync(context.Document, memberDeclaration, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.MarkFieldAsReadOnly:
                        {
                            var fieldDeclaration = (FieldDeclarationSyntax)memberDeclaration;

                            SeparatedSyntaxList<VariableDeclaratorSyntax> declarators = fieldDeclaration.Declaration.Variables;

                            string title = (declarators.Count == 1)
                                ? $"Mark '{declarators[0].Identifier.ValueText}' as read-only"
                                : "Mark fields as read-only";

                            ModifiersCodeFixRegistrator.AddModifier(context, diagnostic, fieldDeclaration, SyntaxKind.ReadOnlyKeyword, title: title);
                            break;
                        }
                    case DiagnosticIdentifiers.UseConstantInsteadOfField:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Use constant instead of field",
                                cancellationToken => UseConstantInsteadOfFieldRefactoring.RefactorAsync(context.Document, (FieldDeclarationSyntax)memberDeclaration, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UseReadOnlyAutoProperty:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Use read-only auto-property",
                                cancellationToken => UseReadOnlyAutoPropertyRefactoring.RefactorAsync(context.Document, (PropertyDeclarationSyntax)memberDeclaration, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.ReplaceCommentWithDocumentationComment:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                ReplaceCommentWithDocumentationCommentRefactoring.Title,
                                cancellationToken => ReplaceCommentWithDocumentationCommentRefactoring.RefactorAsync(context.Document, memberDeclaration, context.Span, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
