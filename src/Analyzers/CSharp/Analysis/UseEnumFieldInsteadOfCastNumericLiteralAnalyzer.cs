// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseEnumFieldInsteadOfCastNumericLiteralAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.UseEnumFieldInsteadOfCastNumericLiteral);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeCastExpression, SyntaxKind.CastExpression);
        }

        public static void AnalyzeCastExpression(SyntaxNodeAnalysisContext context)
        {
            var castExpression = (CastExpressionSyntax)context.Node;
            if (!(castExpression.Expression is LiteralExpressionSyntax expression))
            {
                return;
            }

            if (!(context.SemanticModel.GetTypeSymbol(castExpression.Type, context.CancellationToken) is INamedTypeSymbol namedTypeSymbol)
                || namedTypeSymbol.TypeKind != TypeKind.Enum)
            {
                return;
            }

            var numericLiteralValue = SymbolUtility.GetEnumValueAsUInt64(expression.Token.Value, namedTypeSymbol);
            if (EnumHasDefinedFieldWithNumericLiteralValue(namedTypeSymbol, numericLiteralValue))
            {
                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.UseEnumFieldInsteadOfCastNumericLiteral, context.Node);
            }
        }

        private static bool EnumHasDefinedFieldWithNumericLiteralValue(INamedTypeSymbol enumSymbol, ulong value)
        {
            foreach (var fieldSymbol in enumSymbol.GetMembers().Where(f => f.Kind == SymbolKind.Field).Cast<IFieldSymbol>())
            {
                var fieldInfo = EnumFieldSymbolInfo.Create(fieldSymbol);
                if (fieldInfo.Value == value)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
