// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Analysis.AddExceptionToDocumentationComment;
using static Roslynator.CSharp.Analysis.AddExceptionToDocumentationComment.AddExceptionToDocumentationCommentAnalysis;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AddExceptionToDocumentationCommentAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddExceptionToDocumentationComment); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(startContext =>
            {
                INamedTypeSymbol exceptionSymbol = startContext.Compilation.GetTypeByMetadataName(MetadataNames.System_Exception);

                if (exceptionSymbol == null)
                    return;

                startContext.RegisterSyntaxNodeAction(f => AnalyzeThrowStatement(f, exceptionSymbol), SyntaxKind.ThrowStatement);
                startContext.RegisterSyntaxNodeAction(f => AnalyzeThrowExpression(f, exceptionSymbol), SyntaxKind.ThrowExpression);
            });
        }

        private static void AnalyzeThrowStatement(SyntaxNodeAnalysisContext context, INamedTypeSymbol exceptionSymbol)
        {
            var throwStatement = (ThrowStatementSyntax)context.Node;

            AddExceptionToDocumentationCommentAnalysisResult analysis = Analyze(throwStatement, exceptionSymbol, context.SemanticModel, context.CancellationToken);

            if (!analysis.Success)
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.AddExceptionToDocumentationComment, throwStatement);
        }

        private static void AnalyzeThrowExpression(SyntaxNodeAnalysisContext context, INamedTypeSymbol exceptionSymbol)
        {
            var throwExpression = (ThrowExpressionSyntax)context.Node;

            AddExceptionToDocumentationCommentAnalysisResult analysis = Analyze(throwExpression, exceptionSymbol, context.SemanticModel, context.CancellationToken);

            if (!analysis.Success)
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.AddExceptionToDocumentationComment, throwExpression);
        }
    }
}
