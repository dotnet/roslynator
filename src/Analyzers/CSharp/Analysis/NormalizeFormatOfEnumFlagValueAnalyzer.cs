// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class NormalizeFormatOfEnumFlagValueAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                    Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.NormalizeFormatOfEnumFlagValue);

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeEnumDeclaration(f), SyntaxKind.EnumDeclaration);
        }

        private void AnalyzeEnumDeclaration(SyntaxNodeAnalysisContext context)
        {
            var enumDeclaration = (EnumDeclarationSyntax)context.Node;

            if (enumDeclaration.AttributeLists.Count == 0)
                return;

            EnumFlagValueStyle style = context.GetEnumFlagValueStyle();

            if (style == EnumFlagValueStyle.None)
                return;

            if (context.SemanticModel.GetDeclaredSymbol(enumDeclaration, context.CancellationToken) is not INamedTypeSymbol typeSymbol)
                return;

            if (typeSymbol.TypeKind != TypeKind.Enum)
                return;

            if (!typeSymbol.HasAttribute(MetadataNames.System_FlagsAttribute))
                return;

            foreach (EnumMemberDeclarationSyntax member in enumDeclaration.Members)
            {
                ExpressionSyntax value = member.EqualsValue?.Value.WalkDownParentheses();

                if (value != null)
                {
                    if (value.IsKind(SyntaxKind.NumericLiteralExpression))
                    {
                        var literalExpression = (LiteralExpressionSyntax)value;
                        string text = literalExpression.Token.Text;

                        if (style == EnumFlagValueStyle.DecimalNumber)
                        {
                            if (text.StartsWith("0b", StringComparison.OrdinalIgnoreCase)
                                || text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                            {
                                Analyze(context, member, value, style);
                            }
                        }
                        else if (style == EnumFlagValueStyle.ShiftOperator)
                        {
                            Analyze(context, member, value, style);
                        }
                    }
                    else if (value.IsKind(SyntaxKind.LeftShiftExpression)
                        && style == EnumFlagValueStyle.DecimalNumber)
                    {
                        Analyze(context, member, value, style);
                    }
                }
            }
        }

        private static bool IsFlag(SyntaxNodeAnalysisContext context, EnumMemberDeclarationSyntax declaration)
        {
            if (context.SemanticModel.GetDeclaredSymbol(declaration, context.CancellationToken) is IFieldSymbol fieldSymbol
                && fieldSymbol.HasConstantValue)
            {
                EnumFieldSymbolInfo fieldInfo = EnumFieldSymbolInfo.Create(fieldSymbol);

                return fieldInfo.Value > 1
                    && !fieldInfo.HasCompositeValue();
            }

            return false;
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            EnumMemberDeclarationSyntax member,
            ExpressionSyntax value,
            EnumFlagValueStyle style)
        {
            if (IsFlag(context, member))
            {
                DiagnosticHelpers.ReportDiagnostic(
                    context,
                    DiagnosticRules.NormalizeFormatOfEnumFlagValue,
                    value,
                    (style == EnumFlagValueStyle.DecimalNumber) ? "Convert value to decimal number." : "Use '<<' operator.");
            }
        }
    }
}
