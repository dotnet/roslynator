// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.CSharp.Analysis.Documentation.AddParameterToDocumentationCommentAnalysis;

namespace Roslynator.CSharp.Analysis.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AddParameterToDocumentationCommentAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddParameterToDocumentationComment); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeOperatorDeclaration, SyntaxKind.OperatorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeConversionOperatorDeclaration, SyntaxKind.ConversionOperatorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeDelegateDeclaration, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeIndexerDeclaration, SyntaxKind.IndexerDeclaration);
        }

        private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;

            ParameterListSyntax parameterList = method.ParameterList;

            if (parameterList != null)
                Analyze(context, method, parameterList.Parameters);
        }

        private static void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructor = (ConstructorDeclarationSyntax)context.Node;

            ParameterListSyntax parameterList = constructor.ParameterList;

            if (parameterList != null)
                Analyze(context, constructor, parameterList.Parameters);
        }

        private static void AnalyzeOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var operatorDeclaration = (OperatorDeclarationSyntax)context.Node;

            ParameterListSyntax parameterList = operatorDeclaration.ParameterList;

            if (parameterList != null)
                Analyze(context, operatorDeclaration, parameterList.Parameters);
        }

        private static void AnalyzeConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax)context.Node;

            ParameterListSyntax parameterList = conversionOperatorDeclaration.ParameterList;

            if (parameterList != null)
                Analyze(context, conversionOperatorDeclaration, parameterList.Parameters);
        }

        private static void AnalyzeDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;

            ParameterListSyntax parameterList = delegateDeclaration.ParameterList;

            if (parameterList != null)
                Analyze(context, delegateDeclaration, parameterList.Parameters);
        }

        private static void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            BracketedParameterListSyntax parameterList = indexerDeclaration.ParameterList;

            if (parameterList != null)
                Analyze(context, indexerDeclaration, parameterList.Parameters);
        }
    }
}
