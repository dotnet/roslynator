// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Roslynator.CSharp.SyntaxRewriters;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExpandPropertyAndAddBackingFieldRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            bool prefixIdentifierWithUnderscore = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            string fieldName = TextUtility.ToCamelCase(
                propertyDeclaration.Identifier.ValueText,
                prefixWithUnderscore: prefixIdentifierWithUnderscore);

            FieldDeclarationSyntax fieldDeclaration = CreateBackingField(propertyDeclaration, fieldName)
                .WithFormatterAnnotation();

            PropertyDeclarationSyntax newPropertyDeclaration = ExpandPropertyAndAddBackingField(propertyDeclaration, fieldName)
                .WithTriviaFrom(propertyDeclaration)
                .WithFormatterAnnotation();

            var parentMember = (MemberDeclarationSyntax)propertyDeclaration.Parent;
            SyntaxList<MemberDeclarationSyntax> members = parentMember.GetMembers();

            int propertyIndex = members.IndexOf(propertyDeclaration);

            if (propertyDeclaration.IsReadOnlyAutoProperty())
            {
                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                IEnumerable<ReferencedSymbol> referencedSymbols = await SymbolFinder.FindReferencesAsync(
                    semanticModel.GetDeclaredSymbol(propertyDeclaration, cancellationToken),
                    document.Project.Solution,
                    cancellationToken).ConfigureAwait(false);

                ImmutableArray<IdentifierNameSyntax> identifierNames = SyntaxUtility
                    .FindNodes<IdentifierNameSyntax>(oldRoot, referencedSymbols)
                    .ToImmutableArray();

                var rewriter = new IdentifierNameSyntaxRewriter(identifierNames, Identifier(fieldName));

                var newParentMember = (MemberDeclarationSyntax)rewriter.Visit(parentMember);

                members = newParentMember.GetMembers();
            }

            int indexOfLastField = members.LastIndexOf(f => f.IsKind(SyntaxKind.FieldDeclaration));

            SyntaxList<MemberDeclarationSyntax> newMembers = members
                .Replace(members[propertyIndex], newPropertyDeclaration)
                .Insert(indexOfLastField + 1, fieldDeclaration);

            SyntaxNode newRoot = oldRoot.ReplaceNode(parentMember, parentMember.SetMembers(newMembers));

            return document.WithSyntaxRoot(newRoot);
        }

        private static PropertyDeclarationSyntax ExpandPropertyAndAddBackingField(PropertyDeclarationSyntax propertyDeclaration, string name)
        {
            AccessorDeclarationSyntax getter = propertyDeclaration.Getter();

            if (getter != null)
            {
                AccessorDeclarationSyntax newGetter = getter
                    .WithBody(Block(ReturnStatement(IdentifierName(name))))
                    .WithoutSemicolonToken();

                propertyDeclaration = propertyDeclaration
                    .ReplaceNode(getter, newGetter)
                    .WithoutInitializer()
                    .WithoutSemicolonToken();
            }

            AccessorDeclarationSyntax setter = propertyDeclaration.Setter();

            if (setter != null)
            {
                AccessorDeclarationSyntax newSetter = setter
                    .WithBody(Block(
                        ExpressionStatement(
                            SimpleAssignmentExpression(
                                IdentifierName(name),
                                IdentifierName("value")))))
                    .WithoutSemicolonToken();

                propertyDeclaration = propertyDeclaration.ReplaceNode(setter, newSetter);
            }

            AccessorListSyntax accessorList = SyntaxRemover.RemoveWhitespaceOrEndOfLine(propertyDeclaration.AccessorList)
                .WithCloseBraceToken(propertyDeclaration.AccessorList.CloseBraceToken.WithLeadingTrivia(CSharpFactory.NewLineTrivia()));

            return propertyDeclaration
                .WithAccessorList(accessorList);
        }

        private static FieldDeclarationSyntax CreateBackingField(PropertyDeclarationSyntax propertyDeclaration, string name)
        {
            SyntaxTokenList modifiers = TokenList(Token(SyntaxKind.PrivateKeyword));

            if (propertyDeclaration.Modifiers.Contains(SyntaxKind.StaticKeyword))
                modifiers = modifiers.Add(Token(SyntaxKind.StaticKeyword));

            return FieldDeclaration(modifiers, propertyDeclaration.Type, name, propertyDeclaration.Initializer);
        }
    }
}
