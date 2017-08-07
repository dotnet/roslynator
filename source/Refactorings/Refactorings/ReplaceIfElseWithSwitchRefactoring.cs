// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
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
    internal static class ReplaceIfElseWithSwitchRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, IfStatementSyntax ifStatement)
        {
            if (ifStatement.IsTopmostIf())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                if (GetIfStatements(ifStatement)
                    .All(f => IsValidIf(f, semanticModel, context.CancellationToken)))
                {
                    string title = (ifStatement.IsSimpleIf())
                        ? "Replace if with switch"
                        : "Replace if-else with switch";

                    context.RegisterRefactoring(
                        title,
                        cancellationToken => RefactorAsync(context.Document, ifStatement, cancellationToken));
                }
            }
        }

        private static IEnumerable<IfStatementSyntax> GetIfStatements(IfStatementSyntax ifStatement)
        {
            foreach (IfStatementOrElseClause ifOrElse in ifStatement.GetChain())
            {
                if (ifOrElse.IsIf)
                    yield return ifOrElse.AsIf();
            }
        }

        private static bool IsValidIf(IfStatementSyntax ifStatement, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            ExpressionSyntax condition = ifStatement.Condition;

            return condition.IsKind(SyntaxKind.EqualsExpression, SyntaxKind.LogicalOrExpression)
                && IsValidCondition((BinaryExpressionSyntax)condition, null, semanticModel, cancellationToken);
        }

        private static bool IsValidCondition(
            BinaryExpressionSyntax binaryExpression,
            ExpressionSyntax switchExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            bool success = true;

            while (success)
            {
                success = false;

                SyntaxKind kind = binaryExpression.Kind();

                if (kind == SyntaxKind.LogicalOrExpression)
                {
                    ExpressionSyntax right = binaryExpression.Right;

                    if (right?.IsKind(SyntaxKind.EqualsExpression) == true)
                    {
                        var equalsExpression = (BinaryExpressionSyntax)right;

                        if (IsValidEqualsExpression(equalsExpression, switchExpression, semanticModel, cancellationToken))
                        {
                            if (switchExpression == null)
                            {
                                switchExpression = equalsExpression.Left;

                                if (!IsValidSwitchExpression(switchExpression, semanticModel, cancellationToken))
                                    return false;
                            }

                            ExpressionSyntax left = binaryExpression.Left;

                            if (left?.IsKind(SyntaxKind.LogicalOrExpression, SyntaxKind.EqualsExpression) == true)
                            {
                                binaryExpression = (BinaryExpressionSyntax)left;
                                success = true;
                            }
                        }
                    }
                }
                else if (kind == SyntaxKind.EqualsExpression)
                {
                    return IsValidEqualsExpression(binaryExpression, switchExpression, semanticModel, cancellationToken);
                }
            }

            return false;
        }

        private static bool IsValidEqualsExpression(
            BinaryExpressionSyntax equalsExpression,
            ExpressionSyntax switchExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax left = equalsExpression.Left;

            if (IsValidSwitchExpression(left, semanticModel, cancellationToken))
            {
                ExpressionSyntax right = equalsExpression.Right;

                if (IsValidSwitchExpression(right, semanticModel, cancellationToken)
                    && semanticModel.GetConstantValue(right).HasValue)
                {
                    return switchExpression == null
                        || left?.IsEquivalentTo(switchExpression, topLevel: false) == true;
                }
            }

            return false;
        }

        private static bool IsValidSwitchExpression(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (expression == null)
                return false;

            ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(expression, cancellationToken).ConvertedType;

            if (typeSymbol.IsEnum())
                return true;

            switch (typeSymbol.SpecialType)
            {
                case SpecialType.System_Boolean:
                case SpecialType.System_Char:
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_String:
                    return true;
            }

            if (typeSymbol.IsNamedType())
            {
                var namedTypeSymbol = (INamedTypeSymbol)typeSymbol;

                if (namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T)
                {
                    switch (namedTypeSymbol.ConstructedFrom.TypeArguments[0].SpecialType)
                    {
                        case SpecialType.System_Boolean:
                        case SpecialType.System_Char:
                        case SpecialType.System_SByte:
                        case SpecialType.System_Byte:
                        case SpecialType.System_Int16:
                        case SpecialType.System_UInt16:
                        case SpecialType.System_Int32:
                        case SpecialType.System_UInt32:
                        case SpecialType.System_Int64:
                        case SpecialType.System_UInt64:
                        case SpecialType.System_Single:
                        case SpecialType.System_Double:
                            return true;
                    }
                }
            }

            return false;
        }

        private static Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken)
        {
            SwitchStatementSyntax switchStatement = SwitchStatement(
                GetSwitchExpression(ifStatement),
                List(CreateSwitchSections(ifStatement)));

            switchStatement = switchStatement
                .WithTriviaFrom(ifStatement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(ifStatement, switchStatement, cancellationToken);
        }

        private static ExpressionSyntax GetSwitchExpression(IfStatementSyntax ifStatement)
        {
            var condition = (BinaryExpressionSyntax)ifStatement.Condition;

            if (condition.IsKind(SyntaxKind.LogicalOrExpression))
            {
                var right = (BinaryExpressionSyntax)condition.Right;

                return right.Left;
            }

            return condition.Left;
        }

        private static IEnumerable<SwitchSectionSyntax> CreateSwitchSections(IfStatementSyntax ifStatement)
        {
            foreach (IfStatementOrElseClause ifOrElse in ifStatement.GetChain())
            {
                if (ifOrElse.IsIf)
                {
                    ifStatement = ifOrElse.AsIf();

                    var condition = ifStatement.Condition as BinaryExpressionSyntax;

                    List<SwitchLabelSyntax> labels = CreateSwitchLabels(condition, new List<SwitchLabelSyntax>());
                    labels.Reverse();

                    SwitchSectionSyntax section = SwitchSection(
                        List(labels),
                        AddBreakStatementIfNecessary(ifStatement.Statement));

                    yield return section;
                }
                else
                {
                    yield return DefaultSwitchSection(AddBreakStatementIfNecessary(ifOrElse.Statement));
                }
            }
        }

        private static SyntaxList<StatementSyntax> AddBreakStatementIfNecessary(StatementSyntax statement)
        {
            if (statement.IsKind(SyntaxKind.Block))
            {
                var block = (BlockSyntax)statement;

                SyntaxList<StatementSyntax> statements = block.Statements;

                if (statements.Any()
                    && IsJumpStatement(statements.Last()))
                {
                    return SingletonList<StatementSyntax>(block);
                }
                else
                {
                    return SingletonList<StatementSyntax>(block.AddStatements(BreakStatement()));
                }
            }
            else
            {
                if (IsJumpStatement(statement))
                {
                    return SingletonList(statement);
                }
                else
                {
                    return SingletonList<StatementSyntax>(Block(statement, BreakStatement()));
                }
            }
        }

        private static bool IsJumpStatement(StatementSyntax statement)
        {
            return statement.IsKind(
                SyntaxKind.BreakStatement,
                SyntaxKind.GotoCaseStatement,
                SyntaxKind.ReturnStatement,
                SyntaxKind.ThrowStatement);
        }

        private static List<SwitchLabelSyntax> CreateSwitchLabels(BinaryExpressionSyntax binaryExpression, List<SwitchLabelSyntax> labels)
        {
            if (binaryExpression.IsKind(SyntaxKind.EqualsExpression))
            {
                labels.Add(CaseSwitchLabel(binaryExpression.Right));
            }
            else
            {
                var equalsExpression = (BinaryExpressionSyntax)binaryExpression.Right;

                labels.Add(CaseSwitchLabel(equalsExpression.Right));

                if (binaryExpression.IsKind(SyntaxKind.LogicalOrExpression))
                    return CreateSwitchLabels((BinaryExpressionSyntax)binaryExpression.Left, labels);
            }

            return labels;
        }
    }
}