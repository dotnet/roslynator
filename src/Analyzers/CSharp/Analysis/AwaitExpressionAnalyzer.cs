// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AwaitExpressionAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.CallConfigureAwait); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterCompilationStartAction(startContext =>
            {
                INamedTypeSymbol taskSymbol = startContext.Compilation.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task);

                if (taskSymbol == null)
                    return;

                INamedTypeSymbol valueTaskOfTSymbol = startContext.Compilation.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_ValueTask_T);

                if (startContext.Compilation.GetTypeByMetadataName(MetadataNames.System_Runtime_CompilerServices_ConfiguredTaskAwaitable_T) == null)
                    return;

                startContext.RegisterSyntaxNodeAction(nodeContext => AnalyzeAwaitExpression(nodeContext, taskSymbol, valueTaskOfTSymbol), SyntaxKind.AwaitExpression);
            });
        }

        private static void AnalyzeAwaitExpression(SyntaxNodeAnalysisContext context, INamedTypeSymbol taskSymbol, INamedTypeSymbol valueTaskOfTSymbol)
        {
            var awaitExpression = (AwaitExpressionSyntax)context.Node;

            ExpressionSyntax expression = awaitExpression.Expression;

            if (expression?.IsKind(SyntaxKind.InvocationExpression) != true)
                return;

            if (!(context.SemanticModel.GetSymbol(expression, context.CancellationToken) is IMethodSymbol methodSymbol))
                return;

            if (!(methodSymbol.ReturnType is INamedTypeSymbol returnType))
                return;

            INamedTypeSymbol constructedFrom = returnType.ConstructedFrom;

            if (constructedFrom.Equals(valueTaskOfTSymbol)
                || constructedFrom.EqualsOrInheritsFrom(taskSymbol))
            {
                context.ReportDiagnostic(DiagnosticDescriptors.CallConfigureAwait, expression);
            }
        }
    }
}
