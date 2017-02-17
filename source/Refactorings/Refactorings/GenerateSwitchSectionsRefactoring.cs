// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class GenerateSwitchSectionsRefactoring
    {
        public static async Task<bool> CanRefactorAsync(
            RefactoringContext context,
            SwitchStatementSyntax switchStatement)
        {
            ExpressionSyntax expression = switchStatement.Expression;

            if (expression != null
                && IsEmptyOrContainsOnlyDefaultSection(switchStatement))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

                if (typeSymbol?.IsEnum() == true
                    && typeSymbol.GetFields().Any())
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsEmptyOrContainsOnlyDefaultSection(SwitchStatementSyntax switchStatement)
        {
            SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;

            if (!sections.Any())
            {
                return true;
            }
            else if (sections.Count == 1)
            {
                SwitchSectionSyntax section = sections[0];
                SyntaxList<SwitchLabelSyntax> labels = section.Labels;

                return labels.Count == 1
                    && labels[0].IsKind(SyntaxKind.DefaultSwitchLabel);
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
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var enumTypeSymbol = semanticModel.GetTypeInfo(switchStatement.Expression, cancellationToken).ConvertedType as INamedTypeSymbol;

            TypeSyntax enumType = enumTypeSymbol.ToMinimalTypeSyntax(semanticModel, switchStatement.OpenBraceToken.FullSpan.End);

            SwitchStatementSyntax newNode = switchStatement
                .WithSections(List(CreateSwitchSections(enumTypeSymbol, enumType)))
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(switchStatement, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static IEnumerable<SwitchSectionSyntax> CreateSwitchSections(INamedTypeSymbol enumTypeSymbol, TypeSyntax enumType)
        {
            SyntaxList<StatementSyntax> statements = SingletonList<StatementSyntax>(BreakStatement());

            ImmutableArray<ISymbol> members = enumTypeSymbol.GetMembers();

            var sections = new List<SwitchSectionSyntax>(members.Length);

            foreach (ISymbol memberSymbol in members)
            {
                if (memberSymbol.IsField())
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
