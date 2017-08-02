// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Comparers;
using Roslynator.CSharp.Helpers.ModifierHelpers;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MemberHidesInheritedMemberCodeFixProvider))]
    [Shared]
    public class MemberHidesInheritedMemberCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.MemberHidesInheritedMemberUseNewKeywordIfHidingWasIntended,
                    CompilerDiagnosticIdentifiers.MemberHidesInheritedMemberToMakeCurrentMethodOverrideThatImplementationAddOverrideKeyword);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsAnyCodeFixEnabled(
                CodeFixIdentifiers.AddOverrideModifier,
                CodeFixIdentifiers.AddNewModifier,
                CodeFixIdentifiers.RemoveMemberDeclaration))
            {
                return;
            }

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out MemberDeclarationSyntax memberDeclaration))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.MemberHidesInheritedMemberUseNewKeywordIfHidingWasIntended:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddNewModifier))
                                AddModifier(context, diagnostic, memberDeclaration, SyntaxKind.NewKeyword);

                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveMemberDeclaration))
                                RemoveMember(context, memberDeclaration, diagnostic);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.MemberHidesInheritedMemberToMakeCurrentMethodOverrideThatImplementationAddOverrideKeyword:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddOverrideModifier)
                                && !memberDeclaration.GetModifiers().Contains(SyntaxKind.StaticKeyword))
                            {
                                AddModifier(context, diagnostic, memberDeclaration, SyntaxKind.OverrideKeyword);
                            }

                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddNewModifier))
                                AddModifier(context, diagnostic, memberDeclaration, SyntaxKind.NewKeyword);

                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveMemberDeclaration))
                                RemoveMember(context, memberDeclaration, diagnostic);

                            break;
                        }
                }
            }
        }

        private void AddModifier(CodeFixContext context, Diagnostic diagnostic, MemberDeclarationSyntax memberDeclaration, SyntaxKind kind)
        {
            CodeAction codeAction = CodeAction.Create(
                $"Add '{ModifierHelper.GetModifierName(kind)}' modifier",
                cancellationToken => context.Document.InsertModifierAsync(memberDeclaration, kind, ModifierComparer.Instance, cancellationToken),
                GetEquivalenceKey(diagnostic, kind.ToString()));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private void RemoveMember(CodeFixContext context, MemberDeclarationSyntax memberDeclaration, Diagnostic diagnostic)
        {
            CodeAction codeAction = CodeAction.Create(
                $"Remove {memberDeclaration.GetTitle()}",
                cancellationToken => context.Document.RemoveMemberAsync(memberDeclaration, cancellationToken),
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
    }
}
