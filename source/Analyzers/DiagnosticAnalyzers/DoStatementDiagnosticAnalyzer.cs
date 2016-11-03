// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DoStatementDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.AvoidUsageOfDoStatementToCreateInfiniteLoop,
                    DiagnosticDescriptors.AddEmptyLineAfterLastStatementInDoStatement);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeDoStatement(f), SyntaxKind.DoStatement);
        }

        private void AnalyzeDoStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var doStatement = (DoStatementSyntax)context.Node;

            if (ReplaceDoStatementWithWhileStatementRefactoring.CanRefactor(doStatement))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.AvoidUsageOfDoStatementToCreateInfiniteLoop,
                    doStatement.DoKeyword.GetLocation());
            }

            AnalyzeAddEmptyLineAfterLastStatementInDoStatement(context, doStatement);
        }

        private static void AnalyzeAddEmptyLineAfterLastStatementInDoStatement(SyntaxNodeAnalysisContext context, DoStatementSyntax doStatement)
        {
            StatementSyntax statement = doStatement.Statement;

            if (statement?.IsKind(SyntaxKind.Block) == true)
            {
                var block = (BlockSyntax)statement;

                SyntaxList<StatementSyntax> statements = block.Statements;

                if (statements.Any())
                {
                    SyntaxToken closeBrace = block.CloseBraceToken;

                    if (!closeBrace.IsMissing)
                    {
                        SyntaxToken whileKeyword = doStatement.WhileKeyword;

                        if (!whileKeyword.IsMissing)
                        {
                            int closeBraceLine = closeBrace.GetSpanEndLine();

                            if (closeBraceLine == whileKeyword.GetSpanStartLine())
                            {
                                StatementSyntax last = statements.Last();

                                int line = last.GetSpanEndLine(context.CancellationToken);

                                if (closeBraceLine - line == 1)
                                {
                                    SyntaxTrivia trivia = last
                                        .GetTrailingTrivia()
                                        .FirstOrDefault(f => f.IsEndOfLineTrivia());

                                    if (trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                                    {
                                        context.ReportDiagnostic(
                                            DiagnosticDescriptors.AddEmptyLineAfterLastStatementInDoStatement,
                                            Location.Create(doStatement.SyntaxTree, trivia.Span));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
