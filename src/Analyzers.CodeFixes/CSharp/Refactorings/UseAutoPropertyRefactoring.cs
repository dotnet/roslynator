// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    //XPERF:
    internal static class UseAutoPropertyRefactoring
    {
        private static readonly SyntaxAnnotation _removeAnnotation = new SyntaxAnnotation();

        private static readonly SymbolDisplayFormat _symbolDisplayFormat = new SymbolDisplayFormat(
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);

        public static async Task<Solution> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            CancellationToken cancellationToken)
        {
            SyntaxToken propertyIdentifier = propertyDeclaration.Identifier.WithoutTrivia();

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, cancellationToken);

            ISymbol fieldSymbol = GetFieldSymbol(propertyDeclaration, semanticModel, cancellationToken);

            var variableDeclarator = (VariableDeclaratorSyntax)await fieldSymbol.GetSyntaxAsync(cancellationToken).ConfigureAwait(false);

            var variableDeclaration = (VariableDeclarationSyntax)variableDeclarator.Parent;

            var fieldDeclaration = (FieldDeclarationSyntax)variableDeclaration.Parent;

            bool isSingleDeclarator = variableDeclaration.Variables.Count == 1;

            Solution solution = document.Solution();

            foreach (DocumentReferenceInfo info in await SyntaxFinder.FindReferencesByDocumentAsync(fieldSymbol, solution, allowCandidate: false, cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                ImmutableArray<SyntaxNode> nodes = info.References;

                if (propertyDeclaration.SyntaxTree == info.SyntaxTree)
                {
                    nodes = nodes.Add(propertyDeclaration);

                    if (isSingleDeclarator)
                    {
                        nodes = nodes.Add(fieldDeclaration);
                    }
                    else
                    {
                        nodes = nodes.Add(variableDeclarator);
                    }
                }

                SyntaxNode newRoot = info.Root.ReplaceNodes(nodes, (node, _) =>
                {
                    switch (node.Kind())
                    {
                        case SyntaxKind.IdentifierName:
                            {
                                return CreateNewExpression(node, propertyIdentifier, propertySymbol)
                                    .WithTriviaFrom(node)
                                    .WithFormatterAnnotation();
                            }
                        case SyntaxKind.PropertyDeclaration:
                            {
                                return CreateAutoProperty(propertyDeclaration, variableDeclarator.Initializer);
                            }
                        case SyntaxKind.VariableDeclarator:
                        case SyntaxKind.FieldDeclaration:
                            {
                                return node.WithAdditionalAnnotations(_removeAnnotation);
                            }
                        default:
                            {
                                Debug.Fail(node.ToString());
                                return node;
                            }
                    }
                });

                SyntaxNode nodeToRemove = newRoot.GetAnnotatedNodes(_removeAnnotation).FirstOrDefault();

                if (nodeToRemove != null)
                    newRoot = newRoot.RemoveNode(nodeToRemove, SyntaxRemoveOptions.KeepUnbalancedDirectives);

                solution = solution.WithDocumentSyntaxRoot(info.Document.Id, newRoot);
            }

            return solution;
        }

        private static ISymbol GetFieldSymbol(PropertyDeclarationSyntax property, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            ArrowExpressionClauseSyntax expressionBody = property.ExpressionBody;

            if (expressionBody != null)
            {
                return semanticModel.GetSymbol(expressionBody.Expression, cancellationToken);
            }
            else
            {
                AccessorDeclarationSyntax getter = property.Getter();

                BlockSyntax body = getter.Body;

                if (body != null)
                {
                    var returnStatement = (ReturnStatementSyntax)body.Statements[0];

                    return semanticModel.GetSymbol(returnStatement.Expression, cancellationToken);
                }
                else
                {
                    return semanticModel.GetSymbol(getter.ExpressionBody.Expression, cancellationToken);
                }
            }
        }

        public static ExpressionSyntax CreateNewExpression(SyntaxNode node, SyntaxToken identifier, IPropertySymbol propertySymbol)
        {
            if (node.IsParentKind(SyntaxKind.SimpleMemberAccessExpression)
                && ((MemberAccessExpressionSyntax)node.Parent).Name == node)
            {
                return IdentifierName(identifier);
            }
            else if (propertySymbol.IsStatic)
            {
                return ParseName($"{propertySymbol.ContainingType.ToTypeSyntax()}.{propertySymbol.ToDisplayString(_symbolDisplayFormat)}")
                    .WithSimplifierAnnotation();
            }
            else
            {
                return IdentifierName(identifier).QualifyWithThis();
            }
        }

        public static PropertyDeclarationSyntax CreateAutoProperty(PropertyDeclarationSyntax propertyDeclaration, EqualsValueClauseSyntax initializer)
        {
            AccessorListSyntax accessorList = CreateAccessorList(propertyDeclaration);

            if (accessorList
                .DescendantTrivia()
                .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                accessorList = accessorList.RemoveWhitespace();
            }

            PropertyDeclarationSyntax newProperty = propertyDeclaration
                .WithIdentifier(propertyDeclaration.Identifier.WithTrailingTrivia(Space))
                .WithExpressionBody(null)
                .WithAccessorList(accessorList);

            if (initializer != null)
            {
                newProperty = newProperty
                    .WithInitializer(initializer)
                    .WithSemicolonToken(SemicolonToken());
            }
            else
            {
                newProperty = newProperty.WithSemicolonToken(default(SyntaxToken));
            }

            return newProperty
                .WithTriviaFrom(propertyDeclaration)
                .WithFormatterAnnotation();
        }

        private static AccessorListSyntax CreateAccessorList(PropertyDeclarationSyntax property)
        {
            if (property.ExpressionBody != null)
            {
                return AccessorList(AutoGetAccessorDeclaration())
                    .WithTriviaFrom(property.ExpressionBody);
            }
            else
            {
                AccessorListSyntax accessorList = property.AccessorList;

                IEnumerable<AccessorDeclarationSyntax> newAccessors = accessorList
                    .Accessors
                    .Select(accessor =>
                    {
                        return accessor
                            .WithBody(null)
                            .WithExpressionBody(null)
                            .WithSemicolonToken(SemicolonToken())
                            .WithTriviaFrom(accessor);
                    });

                return accessorList.WithAccessors(List(newAccessors));
            }
        }
    }
}
