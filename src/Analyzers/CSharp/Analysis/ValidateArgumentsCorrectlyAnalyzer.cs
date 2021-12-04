// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;
using Roslynator.CSharp.SyntaxWalkers;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ValidateArgumentsCorrectlyAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                    Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.ValidateArgumentsCorrectly);

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeMethodDeclaration(f), SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeConstructorDeclaration(f), SyntaxKind.ConstructorDeclaration);
        }

        private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            AnalyzeParameterList(context, methodDeclaration.ParameterList, methodDeclaration.Body);
        }

        private static void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;

            AnalyzeParameterList(context, constructorDeclaration.ParameterList, constructorDeclaration.Body);
        }

        private static void AnalyzeParameterList(SyntaxNodeAnalysisContext context, ParameterListSyntax parameterList, BlockSyntax body)
        {
            if (body == null)
                return;

            if (parameterList == null)
                return;

            SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

            if (!parameters.Any())
                return;

            SyntaxList<StatementSyntax> statements = body.Statements;

            int statementCount = statements.Count;

            if (statementCount == 0)
                return;

            AnalyzeUnnecessaryNullCheck(context, parameters, statements);

            int index = -1;
            for (int i = 0; i < statementCount; i++)
            {
                if (IsConditionWithThrow(statements[i]))
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

            ContainsYieldWalker walker = ContainsYieldWalker.GetInstance();

            walker.VisitBlock(body);

            YieldStatementSyntax yieldStatement = walker.YieldStatement;

            ContainsYieldWalker.Free(walker);

            if (yieldStatement == null)
                return;

            if (yieldStatement.SpanStart < statements[index].Span.End)
                return;

            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticRules.ValidateArgumentsCorrectly,
                Location.Create(body.SyntaxTree, new TextSpan(statements[index + 1].SpanStart, 0)));
        }

        private static void AnalyzeUnnecessaryNullCheck(
            SyntaxNodeAnalysisContext context,
            SeparatedSyntaxList<ParameterSyntax> parameters,
            SyntaxList<StatementSyntax> statements)
        {
            int lastIndex = -1;

            foreach (ParameterSyntax parameter in parameters)
            {
                if (parameter.IsParams())
                    break;

                lastIndex++;
            }

            if (lastIndex == -1)
                return;

            foreach (StatementSyntax statement in statements)
            {
                if (statement is IfStatementSyntax ifStatement
                    && ifStatement.IsSimpleIf()
                    && ifStatement.SingleNonBlockStatementOrDefault() is ThrowStatementSyntax throwStatement)
                {
                    NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(ifStatement.Condition, context.SemanticModel, NullCheckStyles.CheckingNull);

                    if (nullCheck.Success)
                    {
                        ParameterSyntax parameter = GetParameter(nullCheck.Expression);

                        if (parameter?.Default?.Value.IsKind(
                            SyntaxKind.NullLiteralExpression,
                            SyntaxKind.DefaultLiteralExpression,
                            SyntaxKind.DefaultExpression) == true)
                        {
                            ITypeSymbol exceptionSymbol = context.SemanticModel.GetTypeSymbol(throwStatement.Expression, context.CancellationToken);

                            if (exceptionSymbol.HasMetadataName(MetadataNames.System_ArgumentNullException))
                            {
                                context.ReportDiagnostic(DiagnosticRules.ValidateArgumentsCorrectly, ifStatement);
                            }
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            ParameterSyntax GetParameter(ExpressionSyntax expression)
            {
                if (expression is IdentifierNameSyntax identifierName)
                {
                    string identifierText = identifierName.Identifier.ValueText;
                    for (int i = 0; i <= lastIndex; i++)
                    {
                        if (parameters[i].Identifier.ValueText == identifierText)
                            return parameters[i];
                    }
                }

                return null;
            }
        }

        private static bool IsConditionWithThrow(StatementSyntax statement)
        {
            return statement is IfStatementSyntax ifStatement
                && ifStatement.IsSimpleIf()
                && ifStatement.SingleNonBlockStatementOrDefault().IsKind(SyntaxKind.ThrowStatement);
        }
    }
}
