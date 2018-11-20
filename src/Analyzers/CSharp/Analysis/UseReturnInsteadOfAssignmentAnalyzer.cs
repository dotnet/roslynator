// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseReturnInsteadOfAssignmentAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.UseReturnInsteadOfAssignment); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeAction(AnalyzeSwitchStatement, SyntaxKind.SwitchStatement);
        }

        public static void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax)context.Node;

            if (ifStatement.IsSimpleIf())
                return;

            StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(ifStatement);

            if (!statementsInfo.Success)
                return;

            ReturnStatementSyntax returnStatement = FindReturnStatementBelow(statementsInfo.Statements, ifStatement);

            ExpressionSyntax expression = returnStatement?.Expression;

            if (expression == null)
                return;

            if (ifStatement.SpanOrTrailingTriviaContainsDirectives())
                return;

            if (returnStatement.SpanOrLeadingTriviaContainsDirectives())
                return;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            ISymbol symbol = semanticModel.GetSymbol(expression, cancellationToken);

            if (symbol == null)
                return;

            if (!IsLocalDeclaredInScopeOrNonRefOrOutParameterOfEnclosingSymbol(symbol, statementsInfo.Parent, semanticModel, cancellationToken))
                return;

            foreach (IfStatementOrElseClause ifOrElse in ifStatement.AsCascade())
            {
                StatementSyntax statement = ifOrElse.Statement;

                if (statement.IsKind(SyntaxKind.Block))
                    statement = ((BlockSyntax)statement).Statements.LastOrDefault();

                if (!IsSymbolAssignedInStatement(symbol, statement, semanticModel, cancellationToken))
                    return;
            }

            context.ReportDiagnostic(DiagnosticDescriptors.UseReturnInsteadOfAssignment, ifStatement);
        }

        public static void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            var switchStatement = (SwitchStatementSyntax)context.Node;

            StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(switchStatement);

            if (!statementsInfo.Success)
                return;

            ReturnStatementSyntax returnStatement = FindReturnStatementBelow(statementsInfo.Statements, switchStatement);

            ExpressionSyntax expression = returnStatement?.Expression;

            if (expression == null)
                return;

            if (switchStatement.SpanOrTrailingTriviaContainsDirectives())
                return;

            if (returnStatement.SpanOrLeadingTriviaContainsDirectives())
                return;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            ISymbol symbol = semanticModel.GetSymbol(expression, cancellationToken);

            if (symbol == null)
                return;

            if (!IsLocalDeclaredInScopeOrNonRefOrOutParameterOfEnclosingSymbol(symbol, statementsInfo.Parent, semanticModel, cancellationToken))
                return;

            foreach (SwitchSectionSyntax section in switchStatement.Sections)
            {
                SyntaxList<StatementSyntax> statements = section.GetStatements();

                if (statements.Count <= 1)
                    return;

                if (!statements.Last().IsKind(SyntaxKind.BreakStatement))
                    return;

                if (!IsSymbolAssignedInStatement(symbol, statements[statements.Count - 2], semanticModel, cancellationToken))
                    return;
            }

            context.ReportDiagnostic(DiagnosticDescriptors.UseReturnInsteadOfAssignment, switchStatement);
        }

        internal static ReturnStatementSyntax FindReturnStatementBelow(SyntaxList<StatementSyntax> statements, StatementSyntax statement)
        {
            int index = statements.IndexOf(statement);

            if (index < statements.Count - 1)
            {
                StatementSyntax nextStatement = statements[index + 1];

                if (nextStatement.IsKind(SyntaxKind.ReturnStatement))
                    return (ReturnStatementSyntax)nextStatement;
            }

            return null;
        }

        private static bool IsLocalDeclaredInScopeOrNonRefOrOutParameterOfEnclosingSymbol(ISymbol symbol, SyntaxNode containingNode, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Local:
                    {
                        var localSymbol = (ILocalSymbol)symbol;

                        var localDeclarationStatement = localSymbol.GetSyntax(cancellationToken).Parent.Parent as LocalDeclarationStatementSyntax;

                        return localDeclarationStatement?.Parent == containingNode;
                    }
                case SymbolKind.Parameter:
                    {
                        var parameterSymbol = (IParameterSymbol)symbol;

                        if (parameterSymbol.RefKind == RefKind.None)
                        {
                            ISymbol enclosingSymbol = semanticModel.GetEnclosingSymbol(containingNode.SpanStart, cancellationToken);

                            if (enclosingSymbol != null)
                            {
                                ImmutableArray<IParameterSymbol> parameters = enclosingSymbol.ParametersOrDefault();

                                return !parameters.IsDefault
                                    && parameters.Contains(parameterSymbol);
                            }
                        }

                        break;
                    }
            }

            return false;
        }

        private static bool IsSymbolAssignedInStatement(ISymbol symbol, StatementSyntax statement, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            SimpleAssignmentStatementInfo assignmentInfo = SyntaxInfo.SimpleAssignmentStatementInfo(statement);

            return assignmentInfo.Success
                && semanticModel.GetSymbol(assignmentInfo.Left, cancellationToken)?.Equals(symbol) == true;
        }
    }
}
