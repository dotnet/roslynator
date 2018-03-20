// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.CSharp.Refactorings.NonAsynchronousMethodNameShouldNotEndWithAsyncRefactoring;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NonAsynchronousMethodNameShouldNotEndWithAsyncDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.NonAsynchronousMethodNameShouldNotEndWithAsync,
                    DiagnosticDescriptors.NonAsynchronousMethodNameShouldNotEndWithAsyncFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterCompilationStartAction(startContext =>
            {
                INamedTypeSymbol taskType = startContext.Compilation.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task);

                INamedTypeSymbol valueTaskType = startContext.Compilation.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_ValueTask_T);

                var windowsRuntimeTypes = default(WindowsRuntimeAsyncTypes);

                INamedTypeSymbol asyncAction = startContext.Compilation.GetTypeByMetadataName("Windows.Foundation.IAsyncAction");

                if (asyncAction != null)
                {
                    windowsRuntimeTypes = new WindowsRuntimeAsyncTypes(
                        asyncAction: asyncAction,
                        asyncActionWithProgress: startContext.Compilation.GetTypeByMetadataName("Windows.Foundation.IAsyncActionWithProgress`1"),
                        asyncOperation: startContext.Compilation.GetTypeByMetadataName("Windows.Foundation.IAsyncOperation`1"),
                        asyncOperationWithProgress: startContext.Compilation.GetTypeByMetadataName("Windows.Foundation.IAsyncOperationWithProgress`2"));
                }

                startContext.RegisterSyntaxNodeAction(nodeContext => AnalyzeMethodDeclaration(nodeContext, taskType, valueTaskType, windowsRuntimeTypes), SyntaxKind.MethodDeclaration);
            });
        }
    }
}
