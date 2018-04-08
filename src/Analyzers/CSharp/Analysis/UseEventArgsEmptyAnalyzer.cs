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
    public class UseEventArgsEmptyAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.UseEventArgsEmpty); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterCompilationStartAction(startContext =>
            {
                INamedTypeSymbol eventArgsSymbol = startContext.Compilation.GetTypeByMetadataName(MetadataNames.System_EventArgs);

                if (eventArgsSymbol != null)
                {
                    startContext.RegisterSyntaxNodeAction(
                        nodeContext => AnalyzeObjectCreationExpression(nodeContext, eventArgsSymbol),
                        SyntaxKind.ObjectCreationExpression);
                }
            });
        }

        public static void AnalyzeObjectCreationExpression(SyntaxNodeAnalysisContext context, INamedTypeSymbol eventArgsSymbol)
        {
            if (context.Node.SpanContainsDirectives())
                return;

            var objectCreation = (ObjectCreationExpressionSyntax)context.Node;

            if (objectCreation.ArgumentList?.Arguments.Count == 0
                && objectCreation.Initializer == null)
            {
                ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(objectCreation, context.CancellationToken);

                if (typeSymbol?.Equals(eventArgsSymbol) == true)
                    context.ReportDiagnostic(DiagnosticDescriptors.UseEventArgsEmpty, objectCreation);
            }
        }
    }
}
