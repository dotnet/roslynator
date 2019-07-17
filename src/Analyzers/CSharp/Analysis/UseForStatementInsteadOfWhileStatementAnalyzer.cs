// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseForStatementInsteadOfWhileStatementAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.UseForStatementInsteadOfWhileStatement); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeWhileStatement, SyntaxKind.WhileStatement);
        }

        public static void AnalyzeWhileStatement(SyntaxNodeAnalysisContext context)
        {
            var whileStatement = (WhileStatementSyntax)context.Node;

            ExpressionSyntax condition = whileStatement.Condition;

            if (condition.IsMissing)
                return;

            if (!condition.IsSingleLine())
                return;

            StatementSyntax statement = whileStatement.Statement;

            if (!statement.IsKind(SyntaxKind.Block))
                return;

            var block = (BlockSyntax)statement;

            SyntaxList<StatementSyntax> innerStatements = block.Statements;

            if (innerStatements.Count <= 1)
                return;

            ExpressionSyntax incrementedExpression = GetIncrementedExpression(innerStatements.Last());

            if (!incrementedExpression.IsKind(SyntaxKind.IdentifierName))
                return;

            SyntaxList<StatementSyntax> outerStatements = SyntaxInfo.StatementListInfo(whileStatement).Statements;

            int index = outerStatements.IndexOf(whileStatement);

            if (index <= 0)
                return;

            SingleLocalDeclarationStatementInfo localInfo = GetLocalInfo(outerStatements[index - 1]);

            if (!localInfo.Success)
                return;

            if (index > 1)
            {
                SingleLocalDeclarationStatementInfo localInfo2 = GetLocalInfo(outerStatements[index - 2]);

                if (localInfo2.Success)
                {
                    ExpressionSyntax incrementedExpression2 = GetIncrementedExpression(innerStatements[innerStatements.Count - 2]);

                    if (incrementedExpression2.IsKind(SyntaxKind.IdentifierName))
                    {
                        var identifierName2 = (IdentifierNameSyntax)incrementedExpression2;

                        if (string.Equals(localInfo2.Identifier.ValueText, identifierName2.Identifier.ValueText, StringComparison.Ordinal))
                            return;
                    }
                }
            }

            var identifierName = (IdentifierNameSyntax)incrementedExpression;

            if (!string.Equals(localInfo.Identifier.ValueText, identifierName.Identifier.ValueText, StringComparison.Ordinal))
                return;

            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.UseForStatementInsteadOfWhileStatement, whileStatement.WhileKeyword);
        }

        private static ExpressionSyntax GetIncrementedExpression(StatementSyntax statement)
        {
            if (statement.IsKind(SyntaxKind.ExpressionStatement))
            {
                var expressionStatement = (ExpressionStatementSyntax)statement;

                ExpressionSyntax expression = expressionStatement.Expression;

                if (expression.IsKind(SyntaxKind.PostIncrementExpression))
                {
                    var postIncrementExpression = (PostfixUnaryExpressionSyntax)expression;

                    return postIncrementExpression.Operand;
                }
            }

            return null;
        }

        private static SingleLocalDeclarationStatementInfo GetLocalInfo(StatementSyntax statement)
        {
            return (statement.IsKind(SyntaxKind.LocalDeclarationStatement))
                ? SyntaxInfo.SingleLocalDeclarationStatementInfo((LocalDeclarationStatementSyntax)statement)
                : default;
        }
    }
}
