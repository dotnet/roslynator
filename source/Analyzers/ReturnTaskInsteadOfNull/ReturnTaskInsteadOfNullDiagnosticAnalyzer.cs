// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.DiagnosticAnalyzers;
using static Roslynator.CSharp.Analyzers.ReturnTaskInsteadOfNull.ReturnTaskInsteadOfNullRefactoring;

namespace Roslynator.CSharp.Analyzers.ReturnTaskInsteadOfNull
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReturnTaskInsteadOfReturningNullDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.ReturnTaskInsteadOfNull); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(startContext =>
            {
                INamedTypeSymbol taskOfTSymbol = startContext.Compilation.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task_T);

                if (taskOfTSymbol == null)
                    return;

                startContext.RegisterSyntaxNodeAction(nodeContext => AnalyzeMethodDeclaration(nodeContext, taskOfTSymbol), SyntaxKind.MethodDeclaration);
                startContext.RegisterSyntaxNodeAction(nodeContext => AnalyzePropertyDeclaration(nodeContext, taskOfTSymbol), SyntaxKind.PropertyDeclaration);
                startContext.RegisterSyntaxNodeAction(nodeContext => AnalyzeIndexerDeclaration(nodeContext, taskOfTSymbol), SyntaxKind.IndexerDeclaration);
                startContext.RegisterSyntaxNodeAction(nodeContext => AnalyzeLocalFunction(nodeContext, taskOfTSymbol), SyntaxKind.LocalFunctionStatement);
                startContext.RegisterSyntaxNodeAction(nodeContext => AnalyzeLambdaExpression(nodeContext, taskOfTSymbol), SyntaxKind.SimpleLambdaExpression);
                startContext.RegisterSyntaxNodeAction(nodeContext => AnalyzeLambdaExpression(nodeContext, taskOfTSymbol), SyntaxKind.ParenthesizedLambdaExpression);
                startContext.RegisterSyntaxNodeAction(nodeContext => AnalyzeAnonymousMethod(nodeContext, taskOfTSymbol), SyntaxKind.AnonymousMethodExpression);
            });
        }
    }
}
