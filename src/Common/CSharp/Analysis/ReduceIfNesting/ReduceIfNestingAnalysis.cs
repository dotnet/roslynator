﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.ReduceIfNesting;

internal static class ReduceIfNestingAnalysis
{
    private static ReduceIfNestingAnalysisResult Success(SyntaxKind jumpKind, SyntaxNode topNode)
    {
        return new(jumpKind, topNode);
    }

    private static ReduceIfNestingAnalysisResult Fail(SyntaxNode topNode)
    {
        return new(SyntaxKind.None, topNode);
    }

    public static ReduceIfNestingAnalysisResult Analyze(
        IfStatementSyntax ifStatement,
        SemanticModel semanticModel,
        ReduceIfNestingOptions options,
        CancellationToken cancellationToken = default)
    {
        if (!IsFixable(ifStatement))
            return Fail(ifStatement);

        return AnalyzeCore(ifStatement, semanticModel, SyntaxKind.None, options, cancellationToken);
    }

    private static ReduceIfNestingAnalysisResult AnalyzeCore(
        IfStatementSyntax ifStatement,
        SemanticModel semanticModel,
        SyntaxKind jumpKind,
        ReduceIfNestingOptions options,
        CancellationToken cancellationToken = default)
    {
        StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(ifStatement);

        if (!statementsInfo.Success)
            return Fail(ifStatement);

        SyntaxNode node = statementsInfo.Parent;
        SyntaxNode parent = node.Parent;
        SyntaxKind parentKind = parent.Kind();

        SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

        if (statementsInfo.IsParentSwitchSection
            || parentKind == SyntaxKind.SwitchSection)
        {
            SyntaxNode switchSection = (statementsInfo.IsParentSwitchSection) ? node : parent;

            if (!options.AllowSwitchSection())
                return Fail(switchSection);

            if (ifStatement != statements.LastButOneOrDefault())
                return Fail(switchSection);

            if (!IsFixableJumpStatement(statements.Last(), ref jumpKind))
                return Fail(switchSection);

            if (!options.AllowNestedFix()
                && IsNestedFix(switchSection.Parent, semanticModel, options, cancellationToken))
            {
                return Fail(switchSection);
            }

            if (IfStatementLocalVariableAnalysis.DoDeclaredVariablesOverlapWithOuterScope(ifStatement, semanticModel))
                return Fail(switchSection);

            return Success(jumpKind, switchSection);
        }

        if (parentKind.Is(
            SyntaxKind.ForStatement,
            SyntaxKind.ForEachStatement,
            SyntaxKind.DoStatement,
            SyntaxKind.WhileStatement))
        {
            if (!options.AllowLoop())
                return Fail(parent);

            StatementSyntax lastStatement = statements.Last();

            if (ifStatement == lastStatement)
            {
                jumpKind = SyntaxKind.ContinueStatement;
            }
            else
            {
                if (ifStatement != statements.LastButOneOrDefault())
                    return Fail(parent);

                if (!IsFixableJumpStatement(lastStatement, ref jumpKind))
                    return Fail(parent);
            }

            if (!options.AllowNestedFix()
                && IsNestedFix(parent.Parent, semanticModel, options, cancellationToken))
            {
                return Fail(parent);
            }

            if (IfStatementLocalVariableAnalysis.DoDeclaredVariablesOverlapWithOuterScope(ifStatement, semanticModel))
                return Fail(parent);

            return Success(jumpKind, parent);
        }

        if (!IsFixable(ifStatement, statements, options, ref jumpKind))
            return Fail(node);

        switch (parentKind)
        {
            case SyntaxKind.ConstructorDeclaration:
            case SyntaxKind.DestructorDeclaration:
            case SyntaxKind.SetAccessorDeclaration:
            case SyntaxKind.AddAccessorDeclaration:
            case SyntaxKind.RemoveAccessorDeclaration:
            {
                if (jumpKind == SyntaxKind.None)
                {
                    jumpKind = SyntaxKind.ReturnStatement;
                }
                else if (jumpKind != SyntaxKind.ReturnStatement)
                {
                    return Fail(parent);
                }

                if (IfStatementLocalVariableAnalysis.DoDeclaredVariablesOverlapWithOuterScope(ifStatement, semanticModel))
                    return Fail(parent);

                return Success(jumpKind, parent);
            }
            case SyntaxKind.OperatorDeclaration:
            case SyntaxKind.ConversionOperatorDeclaration:
            case SyntaxKind.GetAccessorDeclaration:
            {
                if (jumpKind == SyntaxKind.None)
                    return Fail(parent);

                if (IfStatementLocalVariableAnalysis.DoDeclaredVariablesOverlapWithOuterScope(ifStatement, semanticModel))
                    return Fail(parent);

                return Success(jumpKind, parent);
            }
            case SyntaxKind.MethodDeclaration:
            {
                var methodDeclaration = (MethodDeclarationSyntax)parent;

                if (IfStatementLocalVariableAnalysis.DoDeclaredVariablesOverlapWithOuterScope(ifStatement, semanticModel))
                    return Fail(parent);

                if (jumpKind != SyntaxKind.None)
                    return Success(jumpKind, parent);

                if (methodDeclaration.ReturnsVoid())
                    return Success(SyntaxKind.ReturnStatement, parent);

                if (methodDeclaration.Modifiers.Contains(SyntaxKind.AsyncKeyword)
                    && semanticModel
                        .GetDeclaredSymbol(methodDeclaration, cancellationToken)?
                        .ReturnType
                        .HasMetadataName(MetadataNames.System_Threading_Tasks_Task) == true)
                {
                    return Success(SyntaxKind.ReturnStatement, parent);
                }

                if (semanticModel
                    .GetDeclaredSymbol(methodDeclaration, cancellationToken)?
                    .ReturnType
                    .OriginalDefinition
                    .IsIEnumerableOrIEnumerableOfT() == true
                    && methodDeclaration.ContainsYield())
                {
                    return Success(SyntaxKind.YieldBreakStatement, parent);
                }

                break;
            }
            case SyntaxKind.LocalFunctionStatement:
            {
                var localFunction = (LocalFunctionStatementSyntax)parent;

                if (IfStatementLocalVariableAnalysis.DoDeclaredVariablesOverlapWithOuterScope(ifStatement, semanticModel))
                    return Fail(parent);

                if (jumpKind != SyntaxKind.None)
                    return Success(jumpKind, parent);

                if (localFunction.ReturnsVoid())
                    return Success(SyntaxKind.ReturnStatement, parent);

                if (localFunction.Modifiers.Contains(SyntaxKind.AsyncKeyword)
                    && semanticModel.GetDeclaredSymbol(localFunction, cancellationToken)?
                        .ReturnType
                        .HasMetadataName(MetadataNames.System_Threading_Tasks_Task) == true)
                {
                    return Success(SyntaxKind.ReturnStatement, parent);
                }

                if (semanticModel.GetDeclaredSymbol(localFunction, cancellationToken)?
                    .ReturnType
                    .OriginalDefinition
                    .IsIEnumerableOrIEnumerableOfT() == true
                    && localFunction.ContainsYield())
                {
                    return Success(SyntaxKind.YieldBreakStatement, parent);
                }

                break;
            }
            case SyntaxKind.AnonymousMethodExpression:
            case SyntaxKind.SimpleLambdaExpression:
            case SyntaxKind.ParenthesizedLambdaExpression:
            {
                var anonymousFunction = (AnonymousFunctionExpressionSyntax)parent;

                if (IfStatementLocalVariableAnalysis.DoDeclaredVariablesOverlapWithOuterScope(ifStatement, semanticModel))
                    return Fail(parent);

                if (jumpKind != SyntaxKind.None)
                    return Success(jumpKind, parent);

                if (semanticModel.GetSymbol(anonymousFunction, cancellationToken) is not IMethodSymbol methodSymbol)
                    return Fail(parent);

                if (methodSymbol.ReturnsVoid)
                    return Success(SyntaxKind.ReturnStatement, parent);

                if (anonymousFunction.AsyncKeyword.IsKind(SyntaxKind.AsyncKeyword)
                    && methodSymbol.ReturnType.HasMetadataName(MetadataNames.System_Threading_Tasks_Task))
                {
                    return Success(SyntaxKind.ReturnStatement, parent);
                }

                break;
            }
            case SyntaxKind.IfStatement:
            {
                ifStatement = (IfStatementSyntax)parent;

                if (ifStatement.IsParentKind(SyntaxKind.ElseClause))
                {
                    if (ifStatement.Else is not null)
                        return Fail(parent);

                    if (!options.AllowIfInsideIfElse())
                        return Fail(parent);

                    return AnalyzeCore(ifStatement.GetTopmostIf(), semanticModel, jumpKind, options, cancellationToken);
                }
                else
                {
                    if (!IsFixable(ifStatement))
                        return Fail(parent);

                    if (!options.AllowNestedFix())
                        return Fail(parent);

                    return AnalyzeCore(ifStatement, semanticModel, jumpKind, options, cancellationToken);
                }
            }
            case SyntaxKind.ElseClause:
            {
                if (!options.AllowIfInsideIfElse())
                    return Fail(parent);

                var elseClause = (ElseClauseSyntax)parent;

                return AnalyzeCore(elseClause.GetTopmostIf(), semanticModel, jumpKind, options, cancellationToken);
            }
        }

        return Fail(parent);
    }

    private static bool IsNestedFix(SyntaxNode node, SemanticModel semanticModel, ReduceIfNestingOptions options, CancellationToken cancellationToken)
    {
        options |= ReduceIfNestingOptions.AllowNestedFix;

        while (node is not null)
        {
            if (node is IfStatementSyntax ifStatement)
            {
                ReduceIfNestingAnalysisResult analysis = Analyze(ifStatement, semanticModel, options, cancellationToken);

                if (analysis.Success)
                    return true;

                node = analysis.TopNode;
            }

            if (node is MemberDeclarationSyntax)
                return false;

            if (node is AccessorDeclarationSyntax)
                return false;

            node = node.Parent;
        }

        Debug.Fail("");

        return false;
    }

    private static bool IsFixable(
        IfStatementSyntax ifStatement,
        SyntaxList<StatementSyntax> statements,
        ReduceIfNestingOptions options,
        ref SyntaxKind jumpKind)
    {
        int i = statements.Count - 1;

        while (i >= 0
            && statements[i].Kind() == SyntaxKind.LocalFunctionStatement)
        {
            i--;
        }

        if (statements[i] == ifStatement)
        {
            return (options & ReduceIfNestingOptions.AllowLastIf) != 0;
        }
        else if (IsFixableJumpStatement(statements[i], ref jumpKind))
        {
            i--;

            while (i >= 0
                && statements[i].Kind() == SyntaxKind.LocalFunctionStatement)
            {
                i--;
            }

            return statements[i] == ifStatement;
        }

        return false;
    }

    private static bool IsFixableJumpStatement(StatementSyntax statement, ref SyntaxKind kind)
    {
        SyntaxKind kind2 = GetJumpKind(statement);

        if (kind2 == SyntaxKind.None)
        {
            kind = SyntaxKind.None;
            return false;
        }
        else if (kind == SyntaxKind.None)
        {
            kind = kind2;
            return true;
        }

        return kind == kind2;
    }

    internal static SyntaxKind GetJumpKind(StatementSyntax statement)
    {
        switch (statement)
        {
            case BreakStatementSyntax _:
            {
                return SyntaxKind.BreakStatement;
            }
            case ContinueStatementSyntax _:
            {
                return SyntaxKind.ContinueStatement;
            }
            case ReturnStatementSyntax returnStatement:
            {
                ExpressionSyntax expression = returnStatement.Expression;

                if (expression is null)
                    return SyntaxKind.ReturnStatement;

                SyntaxKind kind = expression.Kind();

                if (kind.Is(
                    SyntaxKind.NullLiteralExpression,
                    SyntaxKind.DefaultLiteralExpression,
                    SyntaxKind.TrueLiteralExpression,
                    SyntaxKind.FalseLiteralExpression))
                {
                    return kind;
                }

                return SyntaxKind.None;
            }
            case ThrowStatementSyntax throwStatement:
            {
                ExpressionSyntax expression = throwStatement.Expression;

                if (expression is null)
                    return SyntaxKind.ThrowStatement;

                return SyntaxKind.None;
            }
            default:
            {
                return SyntaxKind.None;
            }
        }
    }

    internal static bool IsFixableRecursively(IfStatementSyntax ifStatement, SyntaxKind jumpKind)
    {
        if (ifStatement.Statement is not BlockSyntax block)
            return false;

        SyntaxList<StatementSyntax> statements = block.Statements;

        if (!statements.Any())
            return false;

        StatementSyntax statement = statements.Last();

        if (statement is IfStatementSyntax ifStatement2)
            return IsFixable(ifStatement2);

        return jumpKind == GetJumpKind(statement)
            && (statements.LastButOneOrDefault() is IfStatementSyntax ifStatement3)
            && IsFixable(ifStatement3);
    }

    internal static bool IsFixable(IfStatementSyntax ifStatement)
    {
        if (ifStatement is null)
            return false;

        if (!ifStatement.IsSimpleIf())
            return false;

        if (ifStatement.Condition?.IsMissing != false)
            return false;

        if (ifStatement.Statement is not BlockSyntax block)
            return false;

        SyntaxList<StatementSyntax> statements = block.Statements;

        if (!statements.Any())
            return false;

        return statements.Count > 1
            || GetJumpKind(statements[0]) == SyntaxKind.None;
    }
}
