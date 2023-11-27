// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidArgumentNullCheckAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.InvalidArgumentNullCheck);

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
        if (body is null)
            return;

        if (parameterList is null)
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
            ArgumentNullCheckAnalysis nullCheck = ArgumentNullCheckAnalysis.Create(statement, context.SemanticModel, context.CancellationToken);

            if (nullCheck.Success)
            {
                ParameterSyntax parameter = FindParameter(nullCheck.Name);

                if (parameter is not null)
                {
                    if (parameter.Default?.Value.IsKind(
                        SyntaxKind.NullLiteralExpression,
                        SyntaxKind.DefaultLiteralExpression,
                        SyntaxKind.DefaultExpression) == true
                        || IsNullableReferenceType(context, parameter))
                    {
                        if (statement is IfStatementSyntax ifStatement)
                        {
                            context.ReportDiagnostic(DiagnosticRules.InvalidArgumentNullCheck, ifStatement.IfKeyword);
                        }
                        else
                        {
                            context.ReportDiagnostic(DiagnosticRules.InvalidArgumentNullCheck, statement);
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

        ParameterSyntax FindParameter(string name)
        {
            for (int i = 0; i <= lastIndex; i++)
            {
                if (parameters[i].Identifier.ValueText == name)
                    return parameters[i];
            }

            return null;
        }
    }
}
