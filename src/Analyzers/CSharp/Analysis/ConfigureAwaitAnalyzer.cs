// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.CodeStyle;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ConfigureAwaitAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                {
                    Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.ConfigureAwait);
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
                    ConfigureAwaitStyle style = c.GetConfigureAwaitStyle();

                    if (style == ConfigureAwaitStyle.Omit)
                    {
                        RemoveCallToConfigureAwait(c);
                    }
                    else if (style == ConfigureAwaitStyle.Include
                        && c.Compilation.GetTypeByMetadataName("System.Runtime.CompilerServices.ConfiguredTaskAwaitable`1") != null)
                    {
                        AddCallToConfigureAwait(c);
                    }
                },
                SyntaxKind.AwaitExpression);
        }

        private static void AddCallToConfigureAwait(SyntaxNodeAnalysisContext context)
        {
            var awaitExpression = (AwaitExpressionSyntax)context.Node;

            ExpressionSyntax expression = awaitExpression.Expression;

            if (IsConfigureAwait(expression))
                return;

            ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(expression, context.CancellationToken);

            if (typeSymbol == null)
                return;

            if (!SymbolUtility.IsAwaitable(typeSymbol))
                return;

            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.ConfigureAwait, awaitExpression.Expression, "Add");
        }

        private static void RemoveCallToConfigureAwait(SyntaxNodeAnalysisContext context)
        {
            var awaitExpression = (AwaitExpressionSyntax)context.Node;

            ExpressionSyntax expression = awaitExpression.Expression;

            SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(expression);

            if (!IsConfigureAwait(expression))
                return;

            ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(expression, context.CancellationToken);

            if (typeSymbol == null)
                return;

            switch (typeSymbol.MetadataName)
            {
                case "ConfiguredTaskAwaitable":
                case "ConfiguredTaskAwaitable`1":
                case "ConfiguredValueTaskAwaitable":
                case "ConfiguredValueTaskAwaitable`1":
                    {
                        if (typeSymbol.ContainingNamespace.HasMetadataName(MetadataNames.System_Runtime_CompilerServices))
                        {
                            DiagnosticHelpers.ReportDiagnostic(
                                context,
                                DiagnosticRules.ConfigureAwait,
                                Location.Create(
                                    awaitExpression.SyntaxTree,
                                    TextSpan.FromBounds(invocationInfo.OperatorToken.SpanStart, expression.Span.End)),
                                "Remove");
                        }

                        break;
                    }
            }
        }

        public static bool IsConfigureAwait(ExpressionSyntax expression)
        {
            SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(expression);

            return IsConfigureAwait(invocationInfo);
        }

        private static bool IsConfigureAwait(SimpleMemberInvocationExpressionInfo invocationInfo)
        {
            return invocationInfo.Success
                && invocationInfo.Name.IsKind(SyntaxKind.IdentifierName)
                && string.Equals(invocationInfo.NameText, "ConfigureAwait")
                && invocationInfo.Arguments.Count == 1;
        }
    }
}
