// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
    public sealed class MemberHidesInheritedMemberCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.CS0108_MemberHidesInheritedMemberUseNewKeywordIfHidingWasIntended,
                    CompilerDiagnosticIdentifiers.CS0114_MemberHidesInheritedMemberToMakeCurrentMethodOverrideThatImplementationAddOverrideKeyword);
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out MemberDeclarationSyntax memberDeclaration))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.CS0108_MemberHidesInheritedMemberUseNewKeywordIfHidingWasIntended:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddNewModifier, context.Document, root.SyntaxTree))
                                ModifiersCodeFixRegistrator.AddModifier(context, diagnostic, memberDeclaration, SyntaxKind.NewKeyword, additionalKey: nameof(SyntaxKind.NewKeyword));

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveMemberDeclaration, context.Document, root.SyntaxTree))
                                CodeFixRegistrator.RemoveMemberDeclaration(context, diagnostic, memberDeclaration);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0114_MemberHidesInheritedMemberToMakeCurrentMethodOverrideThatImplementationAddOverrideKeyword:
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddOverrideModifier, context.Document, root.SyntaxTree)
                                && !SyntaxInfo.ModifierListInfo(memberDeclaration).IsStatic)
                            {
                                ModifiersCodeFixRegistrator.AddModifier(context, diagnostic, memberDeclaration, SyntaxKind.OverrideKeyword, additionalKey: nameof(SyntaxKind.OverrideKeyword));
                            }

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddNewModifier, context.Document, root.SyntaxTree))
                                ModifiersCodeFixRegistrator.AddModifier(context, diagnostic, memberDeclaration, SyntaxKind.NewKeyword, additionalKey: nameof(SyntaxKind.NewKeyword));

                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveMemberDeclaration, context.Document, root.SyntaxTree))
                                CodeFixRegistrator.RemoveMemberDeclaration(context, diagnostic, memberDeclaration);

                            break;
                        }
                }
            }
        }
    }
}
