// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Diagnostics.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BitwiseOperatorDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.BitwiseOperationOnEnumWithoutFlagsAttribute); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(f => AnalyzeBinaryExpression(f),
                SyntaxKind.BitwiseAndExpression,
                SyntaxKind.BitwiseOrExpression,
                SyntaxKind.ExclusiveOrExpression);

            context.RegisterSyntaxNodeAction(f => AnalyzePrefixUnaryExpression(f),
                SyntaxKind.BitwiseNotExpression);
        }

        private void AnalyzeBinaryExpression(SyntaxNodeAnalysisContext context)
        {
            var binaryExpression = (BinaryExpressionSyntax)context.Node;

            if (IsEnumWithoutFlags(context, binaryExpression.Left)
                || IsEnumWithoutFlags(context, binaryExpression.Right))
            {
                ReportDiagnostic(context, binaryExpression);
            }
        }

        private void AnalyzePrefixUnaryExpression(SyntaxNodeAnalysisContext context)
        {
            var prefixUnaryExpression = (PrefixUnaryExpressionSyntax)context.Node;

            if (IsEnumWithoutFlags(context, prefixUnaryExpression.Operand))
                ReportDiagnostic(context, prefixUnaryExpression);
        }

        private static bool IsEnumWithoutFlags(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
        {
            if (expression?.IsMissing == false)
            {
                ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(expression, context.CancellationToken);

                return IsEnumWithoutFlags(typeSymbol, context.SemanticModel);
            }

            return false;
        }

        public static bool IsEnumWithoutFlags(ITypeSymbol typeSymbol, SemanticModel semanticModel)
        {
            if (typeSymbol?.IsEnum() == true)
            {
                ImmutableArray<AttributeData> attributes = typeSymbol.GetAttributes();

                if (attributes.Any())
                {
                    INamedTypeSymbol flagsAttribute = semanticModel.GetTypeByMetadataName("System.FlagsAttribute");

                    for (int i = 0; i < attributes.Length; i++)
                    {
                        if (attributes[i].AttributeClass.Equals(flagsAttribute))
                            return false;
                    }
                }

                return true;
            }

            return false;
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
        {
            context.ReportDiagnostic(
                DiagnosticDescriptors.BitwiseOperationOnEnumWithoutFlagsAttribute,
                expression);
        }
    }
}
