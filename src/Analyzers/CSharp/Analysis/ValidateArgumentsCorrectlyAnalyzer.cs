// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.SyntaxWalkers;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ValidateArgumentsCorrectlyAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.ValidateArgumentsCorrectly); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
        }

        public static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            BlockSyntax body = methodDeclaration.Body;

            if (body == null)
                return;

            ParameterListSyntax parameterList = methodDeclaration.ParameterList;

            if (parameterList == null)
                return;

            if (parameterList.Parameters.Count == 0)
                return;

            SyntaxList<StatementSyntax> statements = body.Statements;

            int statementCount = statements.Count;

            int index = -1;
            for (int i = 0; i < statementCount; i++)
            {
                if (IsNullCheck(statements[i]))
                {
                    index++;
                }
                else
                {
                    break;
                }
            }

            if (index == -1)
                return;

            if (index == statementCount - 1)
                return;

            TextSpan span = TextSpan.FromBounds(statements[index + 1].SpanStart, statements.Last().Span.End);

            if (body.ContainsUnbalancedIfElseDirectives(span))
                return;

            context.CancellationToken.ThrowIfCancellationRequested();

            ContainsYieldWalker walker = ContainsYieldWalker.Cache.GetInstance();

            walker.VisitBlock(body);

            YieldStatementSyntax yieldStatement = walker.YieldStatement;

            ContainsYieldWalker.Cache.Free(walker);

            if (yieldStatement == null)
                return;

            if (yieldStatement.SpanStart < statements[index].Span.End)
                return;

            DiagnosticHelpers.ReportDiagnostic(context,
                DiagnosticDescriptors.ValidateArgumentsCorrectly,
                Location.Create(body.SyntaxTree, new TextSpan(statements[index + 1].SpanStart, 0)));
        }

        private static bool IsNullCheck(StatementSyntax statement)
        {
            return statement.IsKind(SyntaxKind.IfStatement)
                && ((IfStatementSyntax)statement).SingleNonBlockStatementOrDefault().IsKind(SyntaxKind.ThrowStatement);
        }
    }
}
