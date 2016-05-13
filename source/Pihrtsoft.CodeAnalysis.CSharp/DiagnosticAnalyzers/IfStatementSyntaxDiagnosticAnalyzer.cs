// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Pihrtsoft.CodeAnalysis;
using Pihrtsoft.CodeAnalysis.CSharp.Analysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IfStatementSyntaxDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.AddBracesToIfElseChain,
                    DiagnosticDescriptors.RemoveBracesFromIfElseChain,
                    DiagnosticDescriptors.RemoveBracesFromStatementFadeOut,
                    DiagnosticDescriptors.MergeIfStatementWithContainedIfStatement,
                    DiagnosticDescriptors.MergeIfStatementWithContainedIfStatementFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeIfStatement(f), SyntaxKind.IfStatement);
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            if (context.Node.Parent?.IsKind(SyntaxKind.ElseClause) == true)
                return;

            var ifStatement = (IfStatementSyntax)context.Node;

            if (ifStatement.Else != null)
            {
                var result = new IfElseChainAnalysisResult(ifStatement);

                if (result.AddBracesToChain)
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.AddBracesToIfElseChain,
                        ifStatement.GetLocation());
                }

                if (result.RemoveBracesFromChain)
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.RemoveBracesFromIfElseChain,
                        ifStatement.GetLocation());

                    foreach (SyntaxNode node in ifStatement.DescendantNodes())
                    {
                        if (node.IsKind(SyntaxKind.Block))
                        {
                            DiagnosticHelper.FadeOutBraces(
                                context,
                                (BlockSyntax)node,
                                DiagnosticDescriptors.RemoveBracesFromStatementFadeOut);
                        }
                    }
                }
            }
            else if (IfElseChainAnalysis.IsTopmostIf(ifStatement) && ifStatement.Condition?.IsMissing == false)
            {
                IfStatementSyntax ifStatement2 = GetContainedIfStatement(ifStatement);

                if (ifStatement2 != null
                    && ifStatement2.Else == null
                    && ifStatement2.Condition?.IsMissing == false)
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.MergeIfStatementWithContainedIfStatement,
                        ifStatement.GetLocation());

                    MergeIfStatementWithContainedIfStatementFadeOut(context, ifStatement, ifStatement2);
                }
            }
        }

        private static IfStatementSyntax GetContainedIfStatement(IfStatementSyntax ifStatement)
        {
            StatementSyntax statement = ifStatement.Statement;

            switch (statement?.Kind())
            {
                case SyntaxKind.Block:
                    {
                        var block = (BlockSyntax)statement;

                        if (block.Statements.Count == 1
                            && block.Statements[0].IsKind(SyntaxKind.IfStatement))
                        {
                            return (IfStatementSyntax)block.Statements[0];
                        }

                        break;
                    }
                case SyntaxKind.IfStatement:
                    {
                        return (IfStatementSyntax)statement;
                    }
            }

            return null;
        }

        private static void MergeIfStatementWithContainedIfStatementFadeOut(
            SyntaxNodeAnalysisContext context,
            IfStatementSyntax ifStatement,
            IfStatementSyntax ifStatement2)
        {
            DiagnosticDescriptor descriptor = DiagnosticDescriptors.MergeIfStatementWithContainedIfStatementFadeOut;

            DiagnosticHelper.FadeOutToken(context, ifStatement2.IfKeyword, descriptor);
            DiagnosticHelper.FadeOutToken(context, ifStatement2.OpenParenToken, descriptor);
            DiagnosticHelper.FadeOutToken(context, ifStatement2.CloseParenToken, descriptor);

            if (ifStatement.Statement.IsKind(SyntaxKind.Block)
                && ifStatement2.Statement.IsKind(SyntaxKind.Block))
            {
                DiagnosticHelper.FadeOutBraces(context, (BlockSyntax)ifStatement2.Statement, descriptor);
            }
        }
    }
}
