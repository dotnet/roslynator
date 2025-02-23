﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CodeAnalysis.CSharp;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvocationExpressionAnalyzer : BaseDiagnosticAnalyzer
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
                    CodeAnalysisDiagnosticRules.UnnecessaryNullCheck,
                    CodeAnalysisDiagnosticRules.UseElementAccess,
                    CodeAnalysisDiagnosticRules.UseReturnValue);
            }

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeInvocationExpression(f), SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
    {
        var invocationExpression = (InvocationExpressionSyntax)context.Node;

        if (invocationExpression.ContainsDiagnostics)
            return;

        SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocationExpression);

        if (!invocationInfo.Success)
            return;

        ISymbol symbol = null;

        string methodName = invocationInfo.NameText;

        switch (invocationInfo.Arguments.Count)
        {
            case 0:
            {
                switch (methodName)
                {
                    case "First":
                    {
                        if (CodeAnalysisDiagnosticRules.UseElementAccess.IsEffective(context))
                            UseElementAccessInsteadOfCallingFirst();

                        break;
                    }
                }

                break;
            }
            case 1:
            {
                switch (methodName)
                {
                    case "ElementAt":
                    {
                        if (CodeAnalysisDiagnosticRules.UseElementAccess.IsEffective(context))
                            UseElementAccessInsteadOfCallingElementAt();

                        break;
                    }
                    case "IsKind":
                    {
                        if (CodeAnalysisDiagnosticRules.UnnecessaryNullCheck.IsEffective(context))
                            AnalyzeUnnecessaryNullCheck();

                        break;
                    }
                }

                break;
            }
        }

        if (CodeAnalysisDiagnosticRules.UseReturnValue.IsEffective(context)
            && invocationExpression.IsParentKind(SyntaxKind.ExpressionStatement))
        {
            UseReturnValue();
        }

        void AnalyzeUnnecessaryNullCheck()
        {
            ExpressionSyntax expression = invocationInfo.InvocationExpression.WalkUpParentheses();

            SyntaxNode parent = expression.Parent;

            if (!parent.IsKind(SyntaxKind.LogicalAndExpression))
                return;

            var binaryExpression = (BinaryExpressionSyntax)parent;

            if (expression != binaryExpression.Right)
                return;

            if (binaryExpression.Left.ContainsDirectives)
                return;

            if (binaryExpression.OperatorToken.ContainsDirectives)
                return;

            NullCheckExpressionInfo nullCheckInfo = SyntaxInfo.NullCheckExpressionInfo(binaryExpression.Left, NullCheckStyles.CheckingNotNull & ~NullCheckStyles.HasValue);

            if (!nullCheckInfo.Success)
                return;

            if (!CSharpFactory.AreEquivalent(invocationInfo.Expression, nullCheckInfo.Expression))
                return;

            if (!CSharpSymbolUtility.IsIsKindExtensionMethod(invocationExpression, context.SemanticModel, context.CancellationToken))
                return;

            TextSpan span = TextSpan.FromBounds(binaryExpression.Left.SpanStart, binaryExpression.OperatorToken.Span.End);

            DiagnosticHelpers.ReportDiagnostic(
                context,
                CodeAnalysisDiagnosticRules.UnnecessaryNullCheck,
                Location.Create(invocationInfo.InvocationExpression.SyntaxTree, span));
        }

        void UseElementAccessInsteadOfCallingFirst()
        {
            if (!invocationInfo.Expression.GetTrailingTrivia().IsEmptyOrWhitespace())
                return;

            symbol = context.SemanticModel.GetSymbol(invocationExpression, context.CancellationToken);

            if (symbol?.Kind != SymbolKind.Method
                || symbol.IsStatic
                || symbol.DeclaredAccessibility != Accessibility.Public
                || !RoslynSymbolUtility.IsList(symbol.ContainingType.OriginalDefinition))
            {
                return;
            }

            TextSpan span = TextSpan.FromBounds(invocationInfo.Name.SpanStart, invocationExpression.Span.End);

            DiagnosticHelpers.ReportDiagnostic(
                context,
                CodeAnalysisDiagnosticRules.UseElementAccess,
                Location.Create(invocationExpression.SyntaxTree, span));
        }

        void UseElementAccessInsteadOfCallingElementAt()
        {
            if (!invocationInfo.Expression.GetTrailingTrivia().IsEmptyOrWhitespace())
                return;

            symbol = context.SemanticModel.GetSymbol(invocationExpression, context.CancellationToken);

            if (symbol?.Kind != SymbolKind.Method
                || symbol.IsStatic
                || symbol.DeclaredAccessibility != Accessibility.Public
                || !symbol.ContainingType.OriginalDefinition.HasMetadataName(RoslynMetadataNames.Microsoft_CodeAnalysis_SyntaxTriviaList))
            {
                return;
            }

            TextSpan span = TextSpan.FromBounds(invocationInfo.Name.SpanStart, invocationExpression.Span.End);

            DiagnosticHelpers.ReportDiagnostic(
                context,
                CodeAnalysisDiagnosticRules.UseElementAccess,
                Location.Create(invocationExpression.SyntaxTree, span));
        }

        void UseReturnValue()
        {
            if (symbol is null)
                symbol = context.SemanticModel.GetSymbol(invocationExpression, context.CancellationToken);

            if (symbol?.Kind != SymbolKind.Method)
                return;

            if (!RoslynSymbolUtility.IsRoslynType(symbol.ContainingType))
                return;

            var methodSymbol = (IMethodSymbol)symbol;

            if (!RoslynSymbolUtility.IsRoslynType(methodSymbol.ReturnType))
                return;

            DiagnosticHelpers.ReportDiagnostic(context, CodeAnalysisDiagnosticRules.UseReturnValue, invocationExpression);
        }
    }
}
