// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddDefaultAccessModifierCodeFixProvider))]
    [Shared]
    public class AddDefaultAccessModifierCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.AddDefaultAccessModifier); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            MemberDeclarationSyntax declaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<MemberDeclarationSyntax>();

            if (declaration != null
                && context.Document.SupportsSemanticModel)
            {
                var accessModifier = (AccessModifier)Enum.Parse(
                    typeof(AccessModifier),
                    context.Diagnostics[0].Properties[nameof(AccessModifier)]);

                CodeAction codeAction = CodeAction.Create(
                    "Add default access modifier",
                    cancellationToken => RefactorAsync(context.Document, declaration, accessModifier, cancellationToken),
                    DiagnosticIdentifiers.AddDefaultAccessModifier + EquivalenceKeySuffix);

                context.RegisterCodeFix(codeAction, context.Diagnostics);
            }
        }

        internal static async Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax declaration,
            AccessModifier accessModifier,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxToken[] accessModifiers = CreateModifiers(accessModifier);

            List<SyntaxToken> modifiers = declaration.GetModifiers().ToList();

            MemberDeclarationSyntax newDeclaration = declaration;

            if (modifiers.Count > 0)
            {
                accessModifiers[0] = accessModifiers[0].WithLeadingTrivia(modifiers[0].LeadingTrivia);

                modifiers[0] = modifiers[0].WithoutLeadingTrivia();

                modifiers.InsertRange(0, accessModifiers);
            }
            else
            {
                SyntaxToken token = declaration.GetFirstToken();

                accessModifiers[0] = accessModifiers[0].WithLeadingTrivia(token.LeadingTrivia);

                modifiers = accessModifiers.ToList();

                newDeclaration = declaration.ReplaceToken(
                    token,
                    token.WithoutLeadingTrivia());
            }

            newDeclaration = newDeclaration.SetModifiers(TokenList(modifiers));

            SyntaxNode newRoot = root.ReplaceNode(declaration, newDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }

        private static SyntaxToken[] CreateModifiers(AccessModifier accessModifier)
        {
            switch (accessModifier)
            {
                case AccessModifier.Public:
                    return new SyntaxToken[] { PublicToken() };
                case AccessModifier.Internal:
                    return new SyntaxToken[] { InternalToken() };
                case AccessModifier.Protected:
                    return new SyntaxToken[] { ProtectedToken() };
                case AccessModifier.ProtectedInternal:
                    return new SyntaxToken[] { ProtectedToken(), InternalToken() };
                case AccessModifier.Private:
                    return new SyntaxToken[] { PrivateToken() };
                default:
                    return new SyntaxToken[0];
            }
        }
    }
}