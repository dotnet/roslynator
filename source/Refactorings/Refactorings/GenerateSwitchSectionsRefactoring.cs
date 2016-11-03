// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class GenerateSwitchSectionsRefactoring
    {
        public static async Task<bool> CanRefactorAsync(
            RefactoringContext context,
            SwitchStatementSyntax switchStatement)
        {
            if (switchStatement.Expression != null
                && IsEmptyOrContainsOnlyDefaultSection(switchStatement))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                var namedTypeSymbol = semanticModel
                    .GetTypeInfo(switchStatement.Expression, context.CancellationToken)
                    .ConvertedType as INamedTypeSymbol;

                if (namedTypeSymbol?.TypeKind == TypeKind.Enum)
                {
                    foreach (ISymbol memberSymbol in namedTypeSymbol.GetMembers())
                    {
                        if (memberSymbol.IsField())
                            return true;
                    }
                }
            }

            return false;
        }

        private static bool IsEmptyOrContainsOnlyDefaultSection(SwitchStatementSyntax switchStatement)
        {
            if (switchStatement.Sections.Count == 0)
            {
                return true;
            }
            else if (switchStatement.Sections.Count == 1)
            {
                SwitchSectionSyntax section = switchStatement.Sections[0];

                return section.Labels.Count == 1
                    && section.Labels[0].IsKind(SyntaxKind.DefaultSwitchLabel);
            }
            else
            {
                return false;
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            SwitchStatementSyntax switchStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var enumTypeSymbol = semanticModel
                .GetTypeInfo(switchStatement.Expression, cancellationToken)
                .ConvertedType as INamedTypeSymbol;

            SwitchStatementSyntax newNode = switchStatement
                .WithSections(List(CreateSwitchSections(enumTypeSymbol)))
                .WithFormatterAnnotation();

            root = root.ReplaceNode(switchStatement, newNode);

            return document.WithSyntaxRoot(root);
        }

        private static List<SwitchSectionSyntax> CreateSwitchSections(INamedTypeSymbol enumTypeSymbol)
        {
            SyntaxList<StatementSyntax> statements = SingletonList<StatementSyntax>(BreakStatement());

            ImmutableArray<ISymbol> members = enumTypeSymbol.GetMembers();

            var sections = new List<SwitchSectionSyntax>(members.Length);

            TypeSyntax enumType = CSharpFactory.Type(enumTypeSymbol);

            if (members.Length <= 128)
                enumType = enumType.WithSimplifierAnnotation();

            foreach (ISymbol memberSymbol in members)
            {
                if (memberSymbol.IsField())
                {
                    sections.Add(
                        SwitchSection(
                            SingletonList<SwitchLabelSyntax>(
                                CaseSwitchLabel(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        enumType,
                                        IdentifierName(memberSymbol.Name)))),
                            statements));
                }
            }

            sections.Add(SwitchSection(
                SingletonList<SwitchLabelSyntax>(DefaultSwitchLabel()),
                statements));

            return sections;
        }
    }
}
