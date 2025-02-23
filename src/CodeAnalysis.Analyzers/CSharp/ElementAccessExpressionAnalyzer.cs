﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CodeAnalysis.CSharp;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ElementAccessExpressionAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, CodeAnalysisDiagnosticRules.CallLastInsteadOfUsingElementAccess);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeElementAccessExpression(f), SyntaxKind.ElementAccessExpression);
    }

    private static void AnalyzeElementAccessExpression(SyntaxNodeAnalysisContext context)
    {
        var elementAccessExpression = (ElementAccessExpressionSyntax)context.Node;

        ExpressionSyntax expression = elementAccessExpression
            .ArgumentList
            .Arguments
            .SingleOrDefault(shouldThrow: false)?
            .Expression
            .WalkDownParentheses();

        if (expression is null)
            return;

        if (!expression.IsKind(SyntaxKind.SubtractExpression))
            return;

        BinaryExpressionInfo subtractExpressionInfo = SyntaxInfo.BinaryExpressionInfo((BinaryExpressionSyntax)expression);

        if (!subtractExpressionInfo.Right.IsNumericLiteralExpression("1"))
            return;

        if (!subtractExpressionInfo.Left.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            return;

        var memberAccessExpression = (MemberAccessExpressionSyntax)subtractExpressionInfo.Left;

        if (memberAccessExpression.Name is not IdentifierNameSyntax identifierName)
            return;

        if (identifierName.Identifier.ValueText != "Count")
            return;

        if (!CSharpFactory.AreEquivalent(elementAccessExpression.Expression, memberAccessExpression.Expression))
            return;

        ISymbol symbol = context.SemanticModel.GetSymbol(elementAccessExpression, context.CancellationToken);

        if (symbol?.Kind != SymbolKind.Property
            || symbol.IsStatic
            || symbol.DeclaredAccessibility != Accessibility.Public
            || !RoslynSymbolUtility.IsList(symbol.ContainingType.OriginalDefinition))
        {
            return;
        }

        DiagnosticHelpers.ReportDiagnostic(context, CodeAnalysisDiagnosticRules.CallLastInsteadOfUsingElementAccess, elementAccessExpression.ArgumentList);
    }
}
