// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
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
            CancellationToken cancellationToken = default)
        {
            string fieldName = StringUtility.ToCamelCase(
                propertyDeclaration.Identifier.ValueText,
                prefixWithUnderscore: prefixIdentifierWithUnderscore);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            fieldName = NameGenerator.Default.EnsureUniqueName(
                fieldName,
                semanticModel,
                propertyDeclaration.SpanStart);

            FieldDeclarationSyntax fieldDeclaration = FieldDeclaration(
                (propertyDeclaration.Modifiers.Contains(SyntaxKind.StaticKeyword)) ? Modifiers.Private_Static() : Modifiers.Private(),
                propertyDeclaration.Type,
                fieldName,
                propertyDeclaration.Initializer).WithFormatterAnnotation();

            IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, cancellationToken);

            PropertyDeclarationSyntax newPropertyDeclaration = ExpandAccessors(document, propertyDeclaration, propertySymbol, fieldName, semanticModel)
                .WithModifiers(propertyDeclaration.Modifiers.Replace(SyntaxKind.AbstractKeyword, SyntaxKind.VirtualKeyword))
                .WithTriviaFrom(propertyDeclaration)
                .WithFormatterAnnotation();

            MemberDeclarationListInfo membersInfo = SyntaxInfo.MemberDeclarationListInfo(propertyDeclaration.Parent);

            SyntaxList<MemberDeclarationSyntax> members = membersInfo.Members;

            int propertyIndex = membersInfo.IndexOf(propertyDeclaration);

            AccessorListSyntax accessorList = propertyDeclaration.AccessorList;

            if (accessorList?.Getter()?.IsAutoImplemented() == true
                && accessorList.Setter() == null)
            {
                ImmutableArray<SyntaxNode> nodes = await SyntaxFinder.FindReferencesAsync(propertySymbol, document, cancellationToken: cancellationToken).ConfigureAwait(false);

                IdentifierNameSyntax newNode = IdentifierName(fieldName);

                SyntaxNode newParent = membersInfo.Parent.ReplaceNodes(nodes, (node, _) =>
                {
                    if (node.IsParentKind(SyntaxKind.SimpleMemberAccessExpression)
                        && ((MemberAccessExpressionSyntax)node.Parent).Expression.IsKind(SyntaxKind.BaseExpression))
                    {
                        return node;
                    }

                    return newNode.WithTriviaFrom(node);
                });

                MemberDeclarationListInfo newMembersInfo = SyntaxInfo.MemberDeclarationListInfo(newParent);

                members = newMembersInfo.Members;
            }

            SyntaxList<MemberDeclarationSyntax> newMembers = members.ReplaceAt(propertyIndex, newPropertyDeclaration);

            newMembers = MemberDeclarationInserter.Default.Insert(newMembers, fieldDeclaration);

            return await document.ReplaceMembersAsync(membersInfo, newMembers, cancellationToken).ConfigureAwait(false);
        }

        private static PropertyDeclarationSyntax ExpandAccessors(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            IPropertySymbol propertySymbol,
            string fieldName,
            SemanticModel semanticModel)
        {
            AccessorDeclarationSyntax getter = propertyDeclaration.Getter();

            if (getter != null)
            {
                AccessorDeclarationSyntax newGetter = getter.Update(
                    getter.AttributeLists,
                    getter.Modifiers,
                    getter.Keyword,
                    Block(ReturnStatement(IdentifierName(fieldName))),
                    default(SyntaxToken));

                propertyDeclaration = propertyDeclaration
                    .ReplaceNode(getter, newGetter.RemoveWhitespace())
                    .WithInitializer(null)
                    .WithSemicolonToken(default);
            }

            AccessorDeclarationSyntax setter = propertyDeclaration.Setter();

            AccessorDeclarationSyntax newSetter = null;

            if (setter != null)
            {
                newSetter = ExpandSetter();

                propertyDeclaration = propertyDeclaration.ReplaceNode(setter, newSetter);
            }

            if (newSetter?.Body.Statements.First().Kind() != SyntaxKind.IfStatement)
            {
                AccessorListSyntax newAccessorList = propertyDeclaration.AccessorList
                    .RemoveWhitespace()
                    .WithCloseBraceToken(propertyDeclaration.AccessorList.CloseBraceToken.WithLeadingTrivia(NewLine()));

                propertyDeclaration = propertyDeclaration.WithAccessorList(newAccessorList);
            }

            return propertyDeclaration;

            AccessorDeclarationSyntax ExpandSetter()
            {
                BlockSyntax body = null;

                INamedTypeSymbol containingType = propertySymbol.ContainingType;

                if (containingType?.Implements(MetadataNames.System_ComponentModel_INotifyPropertyChanged, allInterfaces: true) == true)
                {
                    IMethodSymbol methodSymbol = SymbolUtility.FindMethodThatRaisePropertyChanged(containingType, setter.SpanStart, semanticModel);

                    if (methodSymbol != null)
                    {
                        string propertyName = propertyDeclaration.Identifier.ValueText;

                        ExpressionSyntax argumentExpression;
                        if (document.SupportsLanguageFeature(CSharpLanguageFeature.NameOf))
                        {
                            argumentExpression = NameOfExpression(propertyName);
                        }
                        else
                        {
                            argumentExpression = StringLiteralExpression(propertyName);
                        }

                        body = Block(
                            IfStatement(
                                NotEqualsExpression(
                                    IdentifierName(fieldName),
                                    IdentifierName("value")),
                                Block(
                                    SimpleAssignmentStatement(
                                        IdentifierName(fieldName),
                                        IdentifierName("value")),
                                    ExpressionStatement(
                                        InvocationExpression(
                                            IdentifierName(methodSymbol.Name),
                                            ArgumentList(Argument(argumentExpression)))))));
                    }
                }

                if (body == null)
                {
                    body = Block(
                       SimpleAssignmentStatement(
                           IdentifierName(fieldName),
                           IdentifierName("value")));
                }

                return setter.Update(
                    setter.AttributeLists,
                    setter.Modifiers,
                    setter.Keyword,
                    body,
                    default(SyntaxToken));
            }
        }
    }
}
