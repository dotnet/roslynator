// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class InvalidNullCheckAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                    Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.InvalidNullCheck);

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
                if (statement is not IfStatementSyntax ifStatement
                    || !ifStatement.IsSimpleIf()
                    || ifStatement.SingleNonBlockStatementOrDefault() is not ThrowStatementSyntax throwStatement)
                {
                    break;
                }

                if (throwStatement.Expression.IsKind(SyntaxKind.ObjectCreationExpression)
                    && !ifStatement.SpanContainsDirectives())
                {
                    NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(ifStatement.Condition, context.SemanticModel, NullCheckStyles.CheckingNull);

                    if (nullCheck.Success)
                    {
                        ParameterSyntax parameter = FindParameter(nullCheck.Expression);

                        if (parameter != null)
                        {
                            if (parameter.Default?.Value.IsKind(
                                SyntaxKind.NullLiteralExpression,
                                SyntaxKind.DefaultLiteralExpression,
                                SyntaxKind.DefaultExpression) == true
                                || IsNullableReferenceType(context, parameter))
                            {
                                ITypeSymbol exceptionSymbol = context.SemanticModel.GetTypeSymbol(throwStatement.Expression, context.CancellationToken);

                                if (exceptionSymbol.HasMetadataName(MetadataNames.System_ArgumentNullException))
                                    context.ReportDiagnostic(DiagnosticRules.InvalidNullCheck, ifStatement);
                            }
                        }
                    }
                }
            }

            bool IsNullableReferenceType(SyntaxNodeAnalysisContext context, ParameterSyntax parameter)
            {
                TypeSyntax type = parameter.Type;

                if (type.IsKind(SyntaxKind.NullableType))
                {
                    ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(type, context.CancellationToken);

                    if (typeSymbol?.IsKind(SymbolKind.ErrorType) == false
                        && typeSymbol.IsReferenceType)
                    {
                        return true;
                    }
                }

                return false;
            }

            ParameterSyntax FindParameter(ExpressionSyntax expression)
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
    }
}
