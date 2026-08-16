// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.SyntaxWalkers;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseExceptionFilterAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.UseExceptionFilter);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterCompilationStartAction(startContext =>
        {
            if (((CSharpCompilation)startContext.Compilation).LanguageVersion < LanguageVersion.CSharp6)
                return;

            startContext.RegisterSyntaxNodeAction(f => AnalyzeCatchClause(f), SyntaxKind.CatchClause);
        });
    }

    private static void AnalyzeCatchClause(SyntaxNodeAnalysisContext context)
    {
        var catchClause = (CatchClauseSyntax)context.Node;

        if (catchClause.Filter is not null)
            return;

        if (catchClause.Block.Statements.FirstOrDefault() is not IfStatementSyntax ifStatement)
            return;

        if (IsThrowStatementWithoutExpression(ifStatement.Statement.SingleNonBlockStatementOrDefault())
            ^ IsThrowStatementWithoutExpression(ifStatement.Else?.Statement.SingleNonBlockStatementOrDefault()))
        {
            var walker = new UseExceptionFilterWalker(context.SemanticModel, context.CancellationToken);

            walker.Visit(ifStatement.Condition);

            if (!walker.CanUseExceptionFilter)
                return;

            if (ifStatement.ContainsUnbalancedIfElseDirectives())
                return;

            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.UseExceptionFilter, ifStatement.IfKeyword);
        }
    }

    private static bool IsThrowStatementWithoutExpression(StatementSyntax statement)
    {
        return (statement is ThrowStatementSyntax throwStatement)
            && throwStatement.Expression is null;
    }

    private class UseExceptionFilterWalker : BaseCSharpSyntaxWalker
    {
        private static readonly Regex _exceptionElementRegex = new(@"\<(?i:exception)\ +cref=(?:""|')");

        public UseExceptionFilterWalker(SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            SemanticModel = semanticModel;
            CancellationToken = cancellationToken;
        }

        public bool CanUseExceptionFilter { get; private set; } = true;

        public SemanticModel SemanticModel { get; }

        public CancellationToken CancellationToken { get; }

        protected override bool ShouldVisit => CanUseExceptionFilter;

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            switch (node.Expression)
            {
                case SimpleNameSyntax simpleName:
                {
                    AnalyzeSimpleName(simpleName);
                    break;
                }
                case MemberBindingExpressionSyntax memberBindingExpression:
                {
                    AnalyzeSimpleName(memberBindingExpression.Name);
                    break;
                }
                case MemberAccessExpressionSyntax memberAccessExpression:
                {
                    AnalyzeSimpleName(memberAccessExpression.Name);
                    break;
                }
                default:
                {
                    SyntaxDebug.Fail(node);
                    break;
                }
            }

            base.VisitInvocationExpression(node);
        }

        private void AnalyzeSimpleName(SimpleNameSyntax simpleName)
        {
            if (simpleName.Identifier.ValueText.StartsWith("ThrowIf", StringComparison.Ordinal))
                CanUseExceptionFilter = false;

            ISymbol symbol = SemanticModel.GetSymbol(simpleName, CancellationToken);

            string xml = symbol?.GetDocumentationCommentXml(cancellationToken: CancellationToken);

            if (xml is not null
                && _exceptionElementRegex.IsMatch(xml))
            {
                CanUseExceptionFilter = false;
            }
        }

        public override void VisitAwaitExpression(AwaitExpressionSyntax node)
        {
            CanUseExceptionFilter = false;
        }

        public override void VisitThrowExpression(ThrowExpressionSyntax node)
        {
            CanUseExceptionFilter = false;
        }

        public override void VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
        {
        }

        public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
        }

        public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
        }
    }
}
