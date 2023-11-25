// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DisposeResourceAsynchronouslyAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.DisposeResourceAsynchronously);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeLocalDeclarationStatement(f), SyntaxKind.LocalDeclarationStatement);
        context.RegisterSyntaxNodeAction(f => AnalyzeUsingStatement(f), SyntaxKind.UsingStatement);
    }

    private static void AnalyzeLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
    {
        var statement = (LocalDeclarationStatementSyntax)context.Node;

        SingleLocalDeclarationStatementInfo localInfo = SyntaxInfo.SingleLocalDeclarationStatementInfo(statement);

        if (!localInfo.Success)
            return;

        if (!localInfo.UsingKeyword.IsKind(SyntaxKind.UsingKeyword))
            return;

        if (localInfo.AwaitKeyword.IsKind(SyntaxKind.AwaitKeyword))
            return;

        ExpressionSyntax value = localInfo.Value;

        if (value is null)
            return;

        Analyze(context, statement, value);
    }

    private static void AnalyzeUsingStatement(SyntaxNodeAnalysisContext context)
    {
        var statement = (UsingStatementSyntax)context.Node;

        if (statement.AwaitKeyword.IsKind(SyntaxKind.AwaitKeyword))
            return;

        VariableDeclaratorSyntax declaration = statement.Declaration?.Variables.SingleOrDefault(shouldThrow: false);

        if (declaration is null)
            return;

        ExpressionSyntax value = declaration.Initializer?.Value;

        if (value is null)
            return;

        Analyze(context, statement, value);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context, StatementSyntax statement, ExpressionSyntax value)
    {
        ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(value, context.CancellationToken);

        if (typeSymbol?.Implements(MetadataNames.System_IAsyncDisposable, allInterfaces: true) != true)
            return;

        for (SyntaxNode node = statement.Parent; node is not null; node = node.Parent)
        {
            if (node is MemberDeclarationSyntax)
            {
                if (node is MethodDeclarationSyntax methodDeclaration)
                    Analyze(context, statement, methodDeclaration.Modifiers, methodDeclaration);

                break;
            }
            else if (node is LocalFunctionStatementSyntax localFunction)
            {
                Analyze(context, statement, localFunction.Modifiers, localFunction);
                break;
            }
            else if (node is LambdaExpressionSyntax lambdaExpression)
            {
                Analyze(context, statement, lambdaExpression.Modifiers, lambdaExpression);
                break;
            }
            else if (node is AnonymousMethodExpressionSyntax anonymousMethod)
            {
                Analyze(context, statement, anonymousMethod.Modifiers, anonymousMethod);
                break;
            }
        }
    }

    private static void Analyze(
        SyntaxNodeAnalysisContext context,
        StatementSyntax statement,
        SyntaxTokenList modifiers,
        SyntaxNode containingMethod)
    {
        if (modifiers.Contains(SyntaxKind.AsyncKeyword))
        {
            ReportDiagnostic(context, statement);
        }
        else
        {
            var methodSymbol = ((containingMethod.IsKind(SyntaxKind.MethodDeclaration, SyntaxKind.LocalFunctionStatement))
                ? context.SemanticModel.GetDeclaredSymbol(containingMethod, context.CancellationToken)
                : context.SemanticModel.GetSymbol(containingMethod, context.CancellationToken)) as IMethodSymbol;

            if (methodSymbol?.IsErrorType() == false
                && SymbolUtility.IsAwaitable(methodSymbol.ReturnType))
            {
                ReportDiagnostic(context, statement);
            }
        }
    }

    private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, StatementSyntax statement)
    {
        context.ReportDiagnostic(DiagnosticRules.DisposeResourceAsynchronously, statement);
    }
}
