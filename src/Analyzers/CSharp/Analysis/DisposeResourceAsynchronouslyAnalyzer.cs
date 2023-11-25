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

        ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(value, context.CancellationToken);

        if (typeSymbol?.Implements(MetadataNames.System_IAsyncDisposable, allInterfaces: true) != true)
            return;

        for (SyntaxNode node = statement.Parent; node is not null; node = node.Parent)
        {
            if (node is MemberDeclarationSyntax)
            {
                if (node is MethodDeclarationSyntax methodDeclaration)
                    Analyze(context, statement, methodDeclaration.Modifiers);

                break;
            }
            else if (node is LocalFunctionStatementSyntax localFunction)
            {
                Analyze(context, statement, localFunction.Modifiers);
                break;
            }
        }
    }

    private static void Analyze(
        SyntaxNodeAnalysisContext context,
        LocalDeclarationStatementSyntax statement,
        SyntaxTokenList modifiers)
    {
        if (modifiers.Contains(SyntaxKind.AsyncKeyword))
            context.ReportDiagnostic(DiagnosticRules.DisposeResourceAsynchronously, statement);
    }
}
