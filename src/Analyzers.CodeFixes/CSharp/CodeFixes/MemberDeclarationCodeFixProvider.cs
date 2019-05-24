// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Refactorings;
using Roslynator.CSharp.Syntax;

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
                    DiagnosticIdentifiers.RemoveRedundantSealedModifier,
                    DiagnosticIdentifiers.AvoidSemicolonAtEndOfDeclaration,
                    DiagnosticIdentifiers.OrderModifiers,
                    DiagnosticIdentifiers.MakeFieldReadOnly,
                    DiagnosticIdentifiers.UseConstantInsteadOfField,
                    DiagnosticIdentifiers.UseReadOnlyAutoProperty,
                    DiagnosticIdentifiers.ConvertCommentToDocumentationComment,
                    DiagnosticIdentifiers.MakeMethodExtensionMethod);
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
                            CodeAction codeAction = CodeActionFactory.RemoveMemberDeclaration(context.Document, memberDeclaration, equivalenceKey: GetEquivalenceKey(diagnostic));

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
                    case DiagnosticIdentifiers.OrderModifiers:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Order modifiers",
                                ct => OrderModifiersAsync(context.Document, memberDeclaration, ct),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.MakeFieldReadOnly:
                        {
                            var fieldDeclaration = (FieldDeclarationSyntax)memberDeclaration;

                            SeparatedSyntaxList<VariableDeclaratorSyntax> declarators = fieldDeclaration.Declaration.Variables;

                            string title = (declarators.Count == 1)
                                ? $"Make '{declarators[0].Identifier.ValueText}' read-only"
                                : "Make fields read-only";

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
                    case DiagnosticIdentifiers.ConvertCommentToDocumentationComment:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                ConvertCommentToDocumentationCommentRefactoring.Title,
                                cancellationToken => ConvertCommentToDocumentationCommentRefactoring.RefactorAsync(context.Document, memberDeclaration, context.Span, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.MakeMethodExtensionMethod:
                        {
                            var methodDeclaration = (MethodDeclarationSyntax)memberDeclaration;

                            CodeAction codeAction = CodeAction.Create(
                                "Make method an extension method",
                                cancellationToken =>
                                {
                                    ParameterSyntax parameter = methodDeclaration.ParameterList.Parameters[0];

                                    ParameterSyntax newParameter = ModifierList.Insert(parameter, SyntaxKind.ThisKeyword);

                                    return context.Document.ReplaceNodeAsync(parameter, newParameter, cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }

        private static Task<Document> OrderModifiersAsync(
            Document document,
            MemberDeclarationSyntax declaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ModifierListInfo info = SyntaxInfo.ModifierListInfo(declaration);

            SyntaxTokenList modifiers = info.Modifiers;

            SyntaxToken[] newModifiers = modifiers.OrderBy(f => f, ModifierComparer.Default).ToArray();

            for (int i = 0; i < modifiers.Count; i++)
                newModifiers[i] = newModifiers[i].WithTriviaFrom(modifiers[i]);

            return document.ReplaceModifiersAsync(info, newModifiers, cancellationToken);
        }
    }
}
