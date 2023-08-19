// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseEnumFieldExplicitlyAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.UseEnumFieldExplicitly);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(c => AnalyzeCastExpression(c), SyntaxKind.CastExpression);
    }

    private static void AnalyzeCastExpression(SyntaxNodeAnalysisContext context)
    {
        var castExpression = (CastExpressionSyntax)context.Node;

        ExpressionSyntax expression = castExpression.Expression;

        if (expression is not LiteralExpressionSyntax literalExpression)
            return;

        string s = literalExpression.Token.Text;

        if (s.Length == 0)
            return;

        if (!s.StartsWith("0x")
            && !s.StartsWith("0X")
            && !s.StartsWith("0b")
            && !s.StartsWith("0B")
            && !char.IsDigit(s[0]))
        {
            return;
        }

        Optional<object> constantValueOpt = context.SemanticModel.GetConstantValue(literalExpression, context.CancellationToken);

        if (!constantValueOpt.HasValue)
            return;

        var enumSymbol = context.SemanticModel.GetTypeSymbol(castExpression.Type, context.CancellationToken) as INamedTypeSymbol;

        if (enumSymbol?.EnumUnderlyingType is null)
            return;

        ulong value = SymbolUtility.GetEnumValueAsUInt64(constantValueOpt.Value, enumSymbol);

        foreach (ISymbol member in enumSymbol.GetMembers())
        {
            if (member is IFieldSymbol fieldSymbol
                && fieldSymbol.HasConstantValue
                && value == SymbolUtility.GetEnumValueAsUInt64(fieldSymbol.ConstantValue, enumSymbol))
            {
                context.ReportDiagnostic(DiagnosticRules.UseEnumFieldExplicitly, castExpression);
                return;
            }
        }

        if (enumSymbol.HasAttribute(MetadataNames.System_FlagsAttribute)
            && FlagsUtility<ulong>.Instance.IsComposite(value))
        {
            EnumSymbolInfo enumInfo = EnumSymbolInfo.Create(enumSymbol);

            foreach (ulong flag in FlagsUtility<ulong>.Instance.GetFlags(value))
            {
                if (!enumInfo.Contains(flag))
                    return;
            }

            context.ReportDiagnostic(DiagnosticRules.UseEnumFieldExplicitly, castExpression);
        }
    }
}
