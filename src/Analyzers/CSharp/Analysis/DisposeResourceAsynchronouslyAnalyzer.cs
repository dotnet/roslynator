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

        Analyze(context, statement, statement.UsingKeyword, value);
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

        Analyze(context, statement, statement.UsingKeyword, value);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context, StatementSyntax statement, SyntaxToken usingKeyword, ExpressionSyntax value)
    {
        ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(value, context.CancellationToken);

        if (typeSymbol?.Implements(MetadataNames.System_IAsyncDisposable, allInterfaces: true) != true)
            return;

        for (SyntaxNode node = statement.Parent; node is not null; node = node.Parent)
        {
            if (node is MemberDeclarationSyntax)
            {
                if (node is MethodDeclarationSyntax methodDeclaration)
                    Analyze(context, methodDeclaration.Modifiers, usingKeyword, methodDeclaration);

                break;
            }
            else if (node is LocalFunctionStatementSyntax localFunction)
            {
                Analyze(context, localFunction.Modifiers, usingKeyword, localFunction);
                break;
            }
            else if (node is LambdaExpressionSyntax lambdaExpression)
            {
                Analyze(context, lambdaExpression.Modifiers, usingKeyword, lambdaExpression);
                break;
            }
            else if (node is AnonymousMethodExpressionSyntax anonymousMethod)
            {
                Analyze(context, anonymousMethod.Modifiers, usingKeyword, anonymousMethod);
                break;
            }
            else if (node is LockStatementSyntax)
            {
                return;
            }
        }
    }

    private static void Analyze(
        SyntaxNodeAnalysisContext context,
        SyntaxTokenList modifiers,
        SyntaxToken usingKeyword,
        SyntaxNode containingMethod)
    {
        if (modifiers.Contains(SyntaxKind.AsyncKeyword))
        {
            ReportDiagnostic(context, usingKeyword);
        }
        else
        {
            var methodSymbol = ((containingMethod.IsKind(SyntaxKind.MethodDeclaration, SyntaxKind.LocalFunctionStatement))
                ? context.SemanticModel.GetDeclaredSymbol(containingMethod, context.CancellationToken)
                : context.SemanticModel.GetSymbol(containingMethod, context.CancellationToken)) as IMethodSymbol;

            if (methodSymbol?.IsErrorType() == false
                && methodSymbol.ReturnType.IsAwaitable(context.SemanticModel, context.Node.SpanStart))
            {
                ReportDiagnostic(context, usingKeyword);
            }
        }
    }

    private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxToken usingKeyword)
    {
        context.ReportDiagnostic(DiagnosticRules.DisposeResourceAsynchronously, usingKeyword);
    }
}
