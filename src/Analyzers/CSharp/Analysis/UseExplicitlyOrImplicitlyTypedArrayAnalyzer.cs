// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class UseExplicitlyOrImplicitlyTypedArrayAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                {
                    Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray);
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
                    ArrayCreationTypeStyle style = c.GetArrayCreationTypeStyle();

                    if (style == ArrayCreationTypeStyle.Explicit
                        || style == ArrayCreationTypeStyle.ImplicitWhenTypeIsObvious)
                    {
                        AnalyzeImplicitArrayCreationExpression(c, style);
                    }
                },
                SyntaxKind.ImplicitArrayCreationExpression);

            context.RegisterSyntaxNodeAction(
                c =>
                {
                    ArrayCreationTypeStyle style = c.GetArrayCreationTypeStyle();

                    if (style == ArrayCreationTypeStyle.Implicit
                        || style == ArrayCreationTypeStyle.ImplicitWhenTypeIsObvious)
                    {
                        AnalyzeArrayCreationExpression(c, style);
                    }
                },
                SyntaxKind.ArrayCreationExpression);
        }

        private static void AnalyzeImplicitArrayCreationExpression(SyntaxNodeAnalysisContext context, ArrayCreationTypeStyle kind)
        {
            var expression = (ImplicitArrayCreationExpressionSyntax)context.Node;

            if (expression.ContainsDiagnostics)
                return;

            if (expression.NewKeyword.ContainsDirectives)
                return;

            if (expression.OpenBracketToken.ContainsDirectives)
                return;

            if (expression.CloseBracketToken.ContainsDirectives)
                return;

            if (kind == ArrayCreationTypeStyle.ImplicitWhenTypeIsObvious)
            {
                InitializerExpressionSyntax initializer = expression.Initializer;

                if (initializer != null)
                {
                    var isObvious = false;

                    foreach (ExpressionSyntax expression2 in initializer.Expressions)
                    {
                        isObvious = CSharpTypeAnalysis.IsTypeObvious(expression2, null, context.SemanticModel, context.CancellationToken);

                        if (!isObvious)
                            break;
                    }

                    if (isObvious)
                        return;
                }
            }

            if (context.SemanticModel.GetTypeSymbol(expression, context.CancellationToken) is not IArrayTypeSymbol arrayTypeSymbol)
                return;

            if (!arrayTypeSymbol.ElementType.SupportsExplicitDeclaration())
                return;

            Location location = Location.Create(expression.SyntaxTree, TextSpan.FromBounds(expression.NewKeyword.SpanStart, expression.CloseBracketToken.Span.End));

            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray,
                location,
                "explicitly");
        }

        private static void AnalyzeArrayCreationExpression(SyntaxNodeAnalysisContext context, ArrayCreationTypeStyle kind)
        {
            var arrayCreation = (ArrayCreationExpressionSyntax)context.Node;

            if (arrayCreation.ContainsDiagnostics)
                return;

            ArrayTypeSyntax arrayType = arrayCreation.Type;

            if (arrayType.ContainsDirectives)
                return;

            SeparatedSyntaxList<ExpressionSyntax> expressions = arrayCreation.Initializer?.Expressions ?? default;

            if (!expressions.Any())
                return;

            ITypeSymbol typeSymbol = null;

            if (kind == ArrayCreationTypeStyle.ImplicitWhenTypeIsObvious)
            {
                foreach (ExpressionSyntax expression in expressions)
                {
                    if (typeSymbol == null)
                    {
                        typeSymbol = context.SemanticModel.GetTypeSymbol(arrayCreation.Type.ElementType, context.CancellationToken);

                        if (typeSymbol?.IsErrorType() != false)
                            return;
                    }

                    if (!CSharpTypeAnalysis.IsTypeObvious(expression, typeSymbol, context.SemanticModel, context.CancellationToken))
                        return;
                }
            }

            TypeSyntax elementType = arrayType.ElementType;
            SyntaxList<ArrayRankSpecifierSyntax> rankSpecifiers = arrayType.RankSpecifiers;

            TextSpan textSpan = TextSpan.FromBounds(
                elementType.SpanStart,
                ((rankSpecifiers.Count > 1) ? rankSpecifiers.LastButOne() : (SyntaxNode)elementType).Span.End);

            Location location = Location.Create(arrayCreation.SyntaxTree, textSpan);

            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray,
                location,
                "implicitly");
        }
    }
}
