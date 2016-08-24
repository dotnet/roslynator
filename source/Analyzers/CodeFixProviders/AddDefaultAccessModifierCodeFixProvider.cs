// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
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

            SyntaxNode declaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<MemberDeclarationSyntax>();

            if (declaration != null
                && context.Document.SupportsSemanticModel)
            {
                CodeAction codeAction = CodeAction.Create(
                    "Add default access modifier",
                    cancellationToken => RefactorAsync(context.Document, declaration, cancellationToken),
                    DiagnosticIdentifiers.AddDefaultAccessModifier + EquivalenceKeySuffix);

                context.RegisterCodeFix(codeAction, context.Diagnostics);
            }
        }

        internal static async Task<Document> RefactorAsync(
            Document document,
            SyntaxNode declaration,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            SyntaxKind modifierKind = GetModifierKind(declaration, semanticModel, cancellationToken);
            SyntaxToken modifierToken = SyntaxFactory.Token(modifierKind).WithTrailingSpace();

            SyntaxTokenList modifiers = declaration.GetDeclarationModifiers();

            SyntaxNode newDeclaration = declaration;

            if (modifiers.Count > 0)
            {
                modifiers = modifiers
                    .Replace(modifiers[0], modifiers[0].WithoutLeadingTrivia())
                    .Insert(0, modifierToken.WithLeadingTrivia(modifiers[0].LeadingTrivia));
            }
            else
            {
                SyntaxNodeOrToken nodeOrToken = GetNodeOrToken(declaration);

                if ((nodeOrToken.IsNode && nodeOrToken.AsNode() == null)
                    || (nodeOrToken.IsToken && nodeOrToken.AsToken().IsKind(SyntaxKind.None)))
                {
                    Debug.Assert(false, "");
                    return document;
                }

                modifiers = SyntaxFactory.TokenList(modifierToken.WithLeadingTrivia(nodeOrToken.GetLeadingTrivia()));

                if (nodeOrToken.IsNode)
                {
                    newDeclaration = declaration.ReplaceNode(
                        nodeOrToken.AsNode(),
                        nodeOrToken.AsNode().WithoutLeadingTrivia());
                }
                else
                {
                    newDeclaration = declaration.ReplaceToken(
                        nodeOrToken.AsToken(),
                        nodeOrToken.AsToken().WithoutLeadingTrivia());
                }
            }

            newDeclaration = GetNewDeclaration(newDeclaration, modifiers);

            SyntaxNode newRoot = oldRoot.ReplaceNode(declaration, newDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }

        private static SyntaxKind GetModifierKind(SyntaxNode syntaxNode, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (syntaxNode.IsKind(SyntaxKind.FieldDeclaration)
                || syntaxNode.IsKind(SyntaxKind.EventFieldDeclaration))
            {
                return SyntaxKind.PrivateKeyword;
            }

            ISymbol symbol = semanticModel.GetDeclaredSymbol(syntaxNode, cancellationToken);

            if (symbol.ContainingType != null)
                return SyntaxKind.PrivateKeyword;

            return SyntaxKind.InternalKeyword;
        }

        private static SyntaxNode GetNewDeclaration(SyntaxNode declaration, SyntaxTokenList modifiers)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.EventFieldDeclaration:
                    return ((EventFieldDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)declaration).WithModifiers(modifiers);
            }

            Debug.Assert(false, declaration.Kind().ToString());

            return declaration;
        }

        private static SyntaxNodeOrToken GetNodeOrToken(SyntaxNode declaration)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)declaration).Keyword;
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)declaration).Identifier;
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)declaration).ImplicitOrExplicitKeyword;
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)declaration).DelegateKeyword;
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)declaration).EnumKeyword;
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)declaration).EventKeyword;
                case SyntaxKind.EventFieldDeclaration:
                    return ((EventFieldDeclarationSyntax)declaration).EventKeyword;
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)declaration).Declaration;
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)declaration).Type;
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)declaration).Keyword;
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)declaration).ReturnType;
                case SyntaxKind.OperatorDeclaration:
                    return ((OperatorDeclarationSyntax)declaration).ReturnType;
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)declaration).Type;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)declaration).Keyword;
                default:
                    {
                        Debug.Assert(false, declaration.Kind().ToString());
                        return SyntaxFactory.Token(SyntaxKind.None);
                    }
            }
        }
    }
}