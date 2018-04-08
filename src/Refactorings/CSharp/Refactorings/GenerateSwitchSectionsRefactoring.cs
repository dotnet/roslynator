// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class GenerateSwitchSectionsRefactoring
    {
        public static bool CanRefactor(
            SwitchStatementSyntax switchStatement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax expression = switchStatement.Expression;

            if (expression == null)
                return false;

            if (!IsEmptyOrContainsOnlyDefaultSection(switchStatement))
                return false;

            ISymbol symbol = semanticModel.GetSymbol(expression, cancellationToken);

            if (symbol?.IsErrorType() != false)
                return false;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken);

            return typeSymbol?.TypeKind == TypeKind.Enum
                && typeSymbol.ContainsMember<IFieldSymbol>();
        }

        private static bool IsEmptyOrContainsOnlyDefaultSection(SwitchStatementSyntax switchStatement)
        {
            SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;

            if (!sections.Any())
                return true;

            return sections
                .SingleOrDefault(shouldThrow: false)?
                .Labels
                .SingleOrDefault(shouldThrow: false)?
                .Kind() == SyntaxKind.DefaultSwitchLabel;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            SwitchStatementSyntax switchStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var enumTypeSymbol = semanticModel.GetTypeInfo(switchStatement.Expression, cancellationToken).ConvertedType as INamedTypeSymbol;

            TypeSyntax enumType = enumTypeSymbol.ToMinimalTypeSyntax(semanticModel, switchStatement.OpenBraceToken.FullSpan.End);

            SwitchStatementSyntax newNode = switchStatement
                .WithSections(List(CreateSwitchSections(enumTypeSymbol, enumType)))
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(switchStatement, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static List<SwitchSectionSyntax> CreateSwitchSections(INamedTypeSymbol enumTypeSymbol, TypeSyntax enumType)
        {
            SyntaxList<StatementSyntax> statements = SingletonList<StatementSyntax>(BreakStatement());

            ImmutableArray<ISymbol> members = enumTypeSymbol.GetMembers();

            var sections = new List<SwitchSectionSyntax>(members.Length);

            foreach (ISymbol memberSymbol in members)
            {
                if (memberSymbol.Kind == SymbolKind.Field)
                {
                    sections.Add(
                        SwitchSection(
                            SingletonList<SwitchLabelSyntax>(
                                CaseSwitchLabel(
                                    SimpleMemberAccessExpression(
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
