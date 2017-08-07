// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseEventArgsEmptyDiagnosticAnalyzer : BaseDiagnosticAnalyzer
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
                        nodeContext => UseEventArgsEmptyRefactoring.AnalyzeObjectCreationExpression(nodeContext, eventArgsSymbol),
                        SyntaxKind.ObjectCreationExpression);
                }
            });
        }
    }
}
