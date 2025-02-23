﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CodeAnalysis.CSharp;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnnecessaryConditionalAccessAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
            {
                Immutable.InterlockedInitialize(
                    ref _supportedDiagnostics,
                    CodeAnalysisDiagnosticRules.UnnecessaryConditionalAccess,
                    CodeAnalysisDiagnosticRules.UnnecessaryConditionalAccessFadeOut);
            }

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeBinaryExpression(f), SyntaxKind.EqualsExpression);
    }

    private static void AnalyzeBinaryExpression(SyntaxNodeAnalysisContext context)
    {
        var binaryExpression = (BinaryExpressionSyntax)context.Node;

        BinaryExpressionInfo binaryExpressionInfo = SyntaxInfo.BinaryExpressionInfo(binaryExpression);

        if (!binaryExpressionInfo.Success)
            return;

        if (binaryExpressionInfo.Right.Kind() != SyntaxKind.TrueLiteralExpression)
            return;

        ExpressionSyntax left = binaryExpressionInfo.Left;

        if (left.Kind() != SyntaxKind.ConditionalAccessExpression)
            return;

        var conditionalAccess = (ConditionalAccessExpressionSyntax)left;

        ExpressionSyntax whenNotNull = conditionalAccess.WhenNotNull;

        if (whenNotNull.Kind() != SyntaxKind.InvocationExpression)
            return;

        var invocationExpression = (InvocationExpressionSyntax)whenNotNull;

        if (invocationExpression.ArgumentList.Arguments.Count != 1)
            return;

        ExpressionSyntax expression = invocationExpression.Expression;

        if (expression.Kind() != SyntaxKind.MemberBindingExpression)
            return;

        var memberBindingExpression = (MemberBindingExpressionSyntax)expression;

        SimpleNameSyntax name = memberBindingExpression.Name;

        if (name.Kind() != SyntaxKind.IdentifierName)
            return;

        var identifierName = (IdentifierNameSyntax)name;

        if (!string.Equals(identifierName.Identifier.ValueText, "IsKind", StringComparison.Ordinal))
            return;

        ISymbol symbol = context.SemanticModel.GetSymbol(invocationExpression, context.CancellationToken);

        if (symbol?.Kind != SymbolKind.Method)
            return;

        var methodSymbol = (IMethodSymbol)symbol;

        if (methodSymbol.MethodKind != MethodKind.ReducedExtension)
            return;

        if (methodSymbol.ReturnType.SpecialType != SpecialType.System_Boolean)
            return;

        if (methodSymbol.ContainingType?.HasMetadataName(RoslynMetadataNames.Microsoft_CodeAnalysis_CSharpExtensions) != true)
            return;

        ImmutableArray<IParameterSymbol> parameters = methodSymbol
            .ReducedFrom
            .Parameters;

        if (parameters.Length != 2)
            return;

        if (!parameters[0].Type.HasMetadataName(MetadataNames.Microsoft_CodeAnalysis_SyntaxNode))
            return;

        if (!parameters[1].Type.HasMetadataName(RoslynMetadataNames.Microsoft_CodeAnalysis_CSharp_SyntaxKind))
            return;

        DiagnosticHelpers.ReportDiagnostic(context, CodeAnalysisDiagnosticRules.UnnecessaryConditionalAccess, conditionalAccess.OperatorToken);
        DiagnosticHelpers.ReportDiagnostic(context, CodeAnalysisDiagnosticRules.UnnecessaryConditionalAccessFadeOut, binaryExpression.Right);
    }
}
