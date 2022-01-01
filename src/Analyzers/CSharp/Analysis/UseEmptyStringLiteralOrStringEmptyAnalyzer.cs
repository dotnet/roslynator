// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class UseEmptyStringLiteralOrStringEmptyAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                {
                    Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.UseEmptyStringLiteralOrStringEmpty);
                }

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(
                c =>
                {
                    if (c.GetEmptyStringStyle() == EmptyStringStyle.Literal)
                        AnalyzeSimpleMemberAccessExpression(c);
                },
                SyntaxKind.SimpleMemberAccessExpression);

            context.RegisterSyntaxNodeAction(
                c =>
                {
                    if (c.GetEmptyStringStyle() == EmptyStringStyle.Field)
                        AnalyzeStringLiteralExpression(c);
                },
                SyntaxKind.StringLiteralExpression);

            context.RegisterSyntaxNodeAction(
                c =>
                {
                    if (c.GetEmptyStringStyle() == EmptyStringStyle.Field)
                        AnalyzeInterpolatedStringExpression(c);
                },
                SyntaxKind.InterpolatedStringExpression);
        }

        private static void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var memberAccess = (MemberAccessExpressionSyntax)context.Node;

            if (memberAccess.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
                return;

            if (memberAccess.Expression == null)
                return;

            if (memberAccess.Name?.Identifier.ValueText != "Empty")
                return;

            var fieldSymbol = context.SemanticModel.GetSymbol(memberAccess.Name, context.CancellationToken) as IFieldSymbol;

            if (!SymbolUtility.IsPublicStaticReadOnly(fieldSymbol))
                return;

            if (fieldSymbol.ContainingType?.SpecialType != SpecialType.System_String)
                return;

            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticRules.UseEmptyStringLiteralOrStringEmpty,
                memberAccess,
                "empty string literal");
        }

        private static void AnalyzeStringLiteralExpression(SyntaxNodeAnalysisContext context)
        {
            var literalExpression = (LiteralExpressionSyntax)context.Node;

            if (literalExpression.Token.ValueText.Length > 0)
                return;

            if (CSharpUtility.IsPartOfExpressionThatMustBeConstant(literalExpression))
                return;

            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticRules.UseEmptyStringLiteralOrStringEmpty,
                literalExpression,
                "'string.Empty'");
        }

        private static void AnalyzeInterpolatedStringExpression(SyntaxNodeAnalysisContext context)
        {
            var interpolatedString = (InterpolatedStringExpressionSyntax)context.Node;

            if (interpolatedString.Contents.Any())
                return;

            if (CSharpUtility.IsPartOfExpressionThatMustBeConstant(interpolatedString))
                return;

            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticRules.UseEmptyStringLiteralOrStringEmpty,
                interpolatedString,
                "'string.Empty'");
        }
    }
}
