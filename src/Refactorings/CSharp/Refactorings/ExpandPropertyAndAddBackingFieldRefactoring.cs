// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
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
            CancellationToken cancellationToken = default)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            string fieldName = StringUtility.ToCamelCase(
                propertyDeclaration.Identifier.ValueText,
                prefixWithUnderscore: prefixIdentifierWithUnderscore);

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

            PropertyDeclarationSyntax newPropertyDeclaration = propertyDeclaration;

            AccessorDeclarationSyntax getter = propertyDeclaration.Getter();

            if (getter != null)
            {
                AccessorDeclarationSyntax newGetter = getter.Update(
                    getter.AttributeLists,
                    getter.Modifiers,
                    getter.Keyword,
                    Block(ReturnStatement(IdentifierName(fieldName))),
                    default(SyntaxToken));

                newPropertyDeclaration = newPropertyDeclaration
                    .ReplaceAccessor(getter, newGetter.RemoveWhitespace())
                    .WithInitializer(null)
                    .WithSemicolonToken(default);
            }

            AccessorDeclarationSyntax setter = newPropertyDeclaration.Setter();

            AccessorDeclarationSyntax newSetter = null;

            if (setter != null)
            {
                newSetter = ExpandSetter();

                newPropertyDeclaration = newPropertyDeclaration.ReplaceAccessor(setter, newSetter);
            }

            if (newSetter?.Body.Statements[0].Kind() != SyntaxKind.IfStatement)
            {
                AccessorListSyntax newAccessorList = newPropertyDeclaration.AccessorList
                    .RemoveWhitespace()
                    .WithCloseBraceToken(newPropertyDeclaration.AccessorList.CloseBraceToken.WithLeadingTrivia(NewLine()));

                newPropertyDeclaration = newPropertyDeclaration.WithAccessorList(newAccessorList);
            }

            newPropertyDeclaration = newPropertyDeclaration
                .WithModifiers(newPropertyDeclaration.Modifiers.Replace(SyntaxKind.AbstractKeyword, SyntaxKind.VirtualKeyword))
                .WithTriviaFrom(propertyDeclaration)
                .WithFormatterAnnotation();

            MemberDeclarationListInfo membersInfo = SyntaxInfo.MemberDeclarationListInfo(propertyDeclaration.Parent);

            SyntaxList<MemberDeclarationSyntax> members = membersInfo.Members;

            int propertyIndex = membersInfo.IndexOf(propertyDeclaration);

            if (propertyDeclaration.AccessorList?.Getter()?.IsAutoImplemented() == true
                && propertyDeclaration.AccessorList.Setter() == null)
            {
                var rewriter = new Rewriter(propertySymbol, fieldName, semanticModel, cancellationToken);

                SyntaxNode newParent = rewriter.Visit(membersInfo.Parent);

                members = SyntaxInfo.MemberDeclarationListInfo(newParent).Members;
            }

            SyntaxList<MemberDeclarationSyntax> newMembers = members.ReplaceAt(propertyIndex, newPropertyDeclaration);

            newMembers = MemberDeclarationInserter.Default.Insert(newMembers, fieldDeclaration);

            return await document.ReplaceMembersAsync(membersInfo, newMembers, cancellationToken).ConfigureAwait(false);

            AccessorDeclarationSyntax ExpandSetter()
            {
                BlockSyntax body = null;

                INamedTypeSymbol containingType = propertySymbol.ContainingType;

                ExpressionSyntax fieldExpression;
                if (propertySymbol.IsStatic)
                {
                    fieldExpression = IdentifierName(fieldName);
                }
                else
                {
                    fieldExpression = IdentifierName(fieldName).QualifyWithThis();
                }

                IdentifierNameSyntax valueName = IdentifierName("value");

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
                                NotEqualsExpression(fieldExpression, valueName),
                                Block(
                                    SimpleAssignmentStatement(fieldExpression, valueName),
                                    ExpressionStatement(
                                        InvocationExpression(
                                            IdentifierName(methodSymbol.Name),
                                            ArgumentList(Argument(argumentExpression)))))));
                    }
                }

                if (body == null)
                {
                    body = Block(SimpleAssignmentStatement(fieldExpression, valueName));
                }

                return setter.Update(
                    setter.AttributeLists,
                    setter.Modifiers,
                    setter.Keyword,
                    body,
                    default(SyntaxToken));
            }
        }

        private class Rewriter : RenameRewriter
        {
            public Rewriter(
                ISymbol symbol,
                string newName,
                SemanticModel semanticModel,
                CancellationToken cancellationToken) : base(symbol, newName, semanticModel, cancellationToken)
            {
            }

            protected override SyntaxNode Rename(IdentifierNameSyntax node)
            {
                if (node.IsParentKind(SyntaxKind.SimpleMemberAccessExpression)
                    && ((MemberAccessExpressionSyntax)node.Parent).Expression.IsKind(SyntaxKind.BaseExpression))
                {
                    return node;
                }

                return base.Rename(node);
            }
        }
    }
}
