// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;

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
                                ModifiersCodeFixRegistrator.AddModifier(context, diagnostic, memberDeclaration, SyntaxKind.NewKeyword, additionalKey: nameof(SyntaxKind.NewKeyword));

                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveMemberDeclaration))
                                CodeFixRegistrator.RemoveMember(context, diagnostic, memberDeclaration);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.MemberHidesInheritedMemberToMakeCurrentMethodOverrideThatImplementationAddOverrideKeyword:
                        {
                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddOverrideModifier)
                                && !SyntaxInfo.ModifierListInfo(memberDeclaration).IsStatic)
                            {
                                ModifiersCodeFixRegistrator.AddModifier(context, diagnostic, memberDeclaration, SyntaxKind.OverrideKeyword, additionalKey: nameof(SyntaxKind.OverrideKeyword));
                            }

                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddNewModifier))
                                ModifiersCodeFixRegistrator.AddModifier(context, diagnostic, memberDeclaration, SyntaxKind.NewKeyword, additionalKey: nameof(SyntaxKind.NewKeyword));

                            if (Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveMemberDeclaration))
                                CodeFixRegistrator.RemoveMember(context, diagnostic, memberDeclaration);

                            break;
                        }
                }
            }
        }
    }
}
