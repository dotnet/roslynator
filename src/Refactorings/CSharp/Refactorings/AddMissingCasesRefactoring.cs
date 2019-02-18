// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddMissingCasesRefactoring
    {
        private const string Title = "Add missing cases";

        public static void ComputeRefactoring(
            RefactoringContext context,
            SwitchStatementSyntax switchStatement,
            SemanticModel semanticModel)
        {
            ExpressionSyntax expression = switchStatement.Expression;

            if (expression?.IsMissing != false)
                return;

            SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;

            ISymbol symbol = semanticModel.GetSymbol(expression, context.CancellationToken);

            if (symbol?.IsErrorType() != false)
                return;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

            if (typeSymbol?.TypeKind != TypeKind.Enum)
                return;

            if (!typeSymbol.ContainsMember<IFieldSymbol>())
                return;

            if (sections.Any() && !ContainsOnlyDefaultSection(sections))
            {
                if (context.Span.IsEmptyAndContainedInSpan(switchStatement.SwitchKeyword))
                {
                    ImmutableArray<ISymbol> members = typeSymbol.GetMembers();

                    if (members.Length == 0)
                        return;

                    var fieldsToValue = new Dictionary<object, IFieldSymbol>(members.Length);

                    foreach (ISymbol member in members)
                    {
                        if (member.Kind == SymbolKind.Field)
                        {
                            var fieldSymbol = (IFieldSymbol)member;

                            if (fieldSymbol.HasConstantValue)
                            {
                                object constantValue = fieldSymbol.ConstantValue;

                                if (!fieldsToValue.ContainsKey(constantValue))
                                    fieldsToValue.Add(constantValue, fieldSymbol);
                            }
                        }
                    }

                    foreach (SwitchSectionSyntax section in sections)
                    {
                        foreach (SwitchLabelSyntax label in section.Labels)
                        {
                            if (label.IsKind(SyntaxKind.CaseSwitchLabel))
                            {
                                var caseLabel = (CaseSwitchLabelSyntax)label;

                                ExpressionSyntax value = caseLabel.Value.WalkDownParentheses();

                                if (value?.IsMissing == false)
                                {
                                    Optional<object> optional = semanticModel.GetConstantValue(value, context.CancellationToken);

                                    if (optional.HasValue
                                        && fieldsToValue.Remove(optional.Value)
                                        && fieldsToValue.Count == 0)
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    }

                    Document document = context.Document;

                    context.RegisterRefactoring(
                        Title,
                        ct => AddCasesAsync(document, switchStatement, fieldsToValue.Select(f => f.Value), ct),
                        RefactoringIdentifiers.AddMissingCases);
                }
            }
            else if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddMissingCases))
            {
                Document document = context.Document;

                context.RegisterRefactoring(
                    Title,
                    ct => AddCasesAsync(document, switchStatement, semanticModel, ct),
                    RefactoringIdentifiers.AddMissingCases);
            }
        }

        private static Task<Document> AddCasesAsync(
            Document document,
            SwitchStatementSyntax switchStatement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var enumTypeSymbol = semanticModel.GetTypeInfo(switchStatement.Expression, cancellationToken).ConvertedType as INamedTypeSymbol;

            TypeSyntax enumType = enumTypeSymbol.ToMinimalTypeSyntax(semanticModel, switchStatement.OpenBraceToken.FullSpan.End);

            cancellationToken.ThrowIfCancellationRequested();

            SyntaxList<StatementSyntax> statements = SingletonList<StatementSyntax>(BreakStatement());

            ImmutableArray<ISymbol> members = enumTypeSymbol.GetMembers();

            var newSections = new List<SwitchSectionSyntax>(members.Length);

            foreach (ISymbol memberSymbol in members)
            {
                if (memberSymbol.Kind == SymbolKind.Field)
                    newSections.Add(CreateSwitchSection(memberSymbol, enumType, statements));
            }

            newSections.Add(SwitchSection(
                SingletonList<SwitchLabelSyntax>(DefaultSwitchLabel()),
                statements));

            SwitchStatementSyntax newSwitchStatement = switchStatement
                .WithSections(newSections.ToSyntaxList())
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(switchStatement, newSwitchStatement, cancellationToken);
        }

        private static Task<Document> AddCasesAsync(
            Document document,
            SwitchStatementSyntax switchStatement,
            IEnumerable<IFieldSymbol> fieldSymbols,
            CancellationToken cancellationToken)
        {
            SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;

            TypeSyntax enumType = fieldSymbols.First().ContainingType.ToTypeSyntax().WithSimplifierAnnotation();

            SyntaxList<StatementSyntax> statements = SingletonList<StatementSyntax>(BreakStatement());

            cancellationToken.ThrowIfCancellationRequested();

            SyntaxList<SwitchSectionSyntax> newSections = fieldSymbols
                .Select(fieldSymbol => CreateSwitchSection(fieldSymbol, enumType, statements))
                .ToSyntaxList();

            int insertIndex = sections.Count;

            if (sections.Last().ContainsDefaultLabel())
                insertIndex--;

            SwitchStatementSyntax newSwitchStatement = switchStatement
                .WithSections(sections.InsertRange(insertIndex, newSections))
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(switchStatement, newSwitchStatement, cancellationToken);
        }

        private static SwitchSectionSyntax CreateSwitchSection(ISymbol symbol, TypeSyntax enumType, SyntaxList<StatementSyntax> statements)
        {
            return SwitchSection(
                SingletonList<SwitchLabelSyntax>(
                    CaseSwitchLabel(
                        SimpleMemberAccessExpression(
                            enumType,
                            IdentifierName(symbol.Name)))),
                statements);
        }

        private static bool ContainsOnlyDefaultSection(SyntaxList<SwitchSectionSyntax> sections)
        {
            return sections
                .SingleOrDefault(shouldThrow: false)?
                .Labels
                .SingleOrDefault(shouldThrow: false)?
                .Kind() == SyntaxKind.DefaultSwitchLabel;
        }
    }
}
