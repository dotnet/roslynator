// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EventFieldDeclarationCodeFixProvider))]
    [Shared]
    public class EventFieldDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.SplitDeclarationIntoMultipleDeclarations);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            EventFieldDeclarationSyntax eventFieldDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<EventFieldDeclarationSyntax>();

            if (eventFieldDeclaration == null)
                return;

            context.RegisterCodeFix(
                CodeAction.Create(
                    "Split declaration into multiple declarations",
                    cancellationToken =>
                    {
                        return SplitVariablesIntoMultipleDeclarationsAsync(
                            context.Document,
                            eventFieldDeclaration,
                            cancellationToken);
                    },
                    DiagnosticIdentifiers.SplitDeclarationIntoMultipleDeclarations + EquivalenceKeySuffix),
                context.Diagnostics);
        }

        private static async Task<Document> SplitVariablesIntoMultipleDeclarationsAsync(
            Document document,
            EventFieldDeclarationSyntax declaration,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            var containingMember = (MemberDeclarationSyntax)declaration.Parent;

            SyntaxList<MemberDeclarationSyntax> members = containingMember.GetMembers();

            SyntaxList<MemberDeclarationSyntax> newMembers = members.ReplaceRange(
                declaration,
                CreateFieldDeclarations(declaration));

            MemberDeclarationSyntax newNode = containingMember.SetMembers(newMembers);

            SyntaxNode newRoot = oldRoot.ReplaceNode(containingMember, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static IEnumerable<EventFieldDeclarationSyntax> CreateFieldDeclarations(EventFieldDeclarationSyntax fieldDeclaration)
        {
            SeparatedSyntaxList<VariableDeclaratorSyntax> variables = fieldDeclaration.Declaration.Variables;

            EventFieldDeclarationSyntax fieldDeclaration2 = fieldDeclaration.WithoutTrivia();

            for (int i = 0; i < variables.Count; i++)
            {
                EventFieldDeclarationSyntax newDeclaration = EventFieldDeclaration(
                    fieldDeclaration2.AttributeLists,
                    fieldDeclaration2.Modifiers,
                    VariableDeclaration(
                        fieldDeclaration2.Declaration.Type,
                        SingletonSeparatedList(variables[i])));

                if (i == 0)
                    newDeclaration = newDeclaration.WithLeadingTrivia(fieldDeclaration.GetLeadingTrivia());

                if (i == variables.Count - 1)
                    newDeclaration = newDeclaration.WithTrailingTrivia(fieldDeclaration.GetTrailingTrivia());

                yield return newDeclaration.WithAdditionalAnnotations(Formatter.Annotation);
            }
        }
    }
}
