// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.CSharp.Analysis.Documentation.AddTypeParameterToDocumentationCommentAnalysis;

namespace Roslynator.CSharp.Analysis.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AddTypeParameterToDocumentationCommentAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddTypeParameterToDocumentationComment); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeInterfaceDeclaration, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeStructDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeDelegateDeclaration, SyntaxKind.DelegateDeclaration);
        }

        private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            TypeParameterListSyntax typeParameterList = classDeclaration.TypeParameterList;

            if (typeParameterList != null)
                Analyze(context, classDeclaration, typeParameterList.Parameters);
        }

        private static void AnalyzeInterfaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var interfaceDeclaration = (InterfaceDeclarationSyntax)context.Node;

            TypeParameterListSyntax typeParameterList = interfaceDeclaration.TypeParameterList;

            if (typeParameterList != null)
                Analyze(context, interfaceDeclaration, typeParameterList.Parameters);
        }

        private static void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
        {
            var structDeclaration = (StructDeclarationSyntax)context.Node;

            TypeParameterListSyntax typeParameterList = structDeclaration.TypeParameterList;

            if (typeParameterList != null)
                Analyze(context, structDeclaration, typeParameterList.Parameters);
        }

        private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;

            TypeParameterListSyntax typeParameterList = method.TypeParameterList;

            if (typeParameterList != null)
                Analyze(context, method, typeParameterList.Parameters);
        }

        private static void AnalyzeDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;

            TypeParameterListSyntax typeParameterList = delegateDeclaration.TypeParameterList;

            if (typeParameterList != null)
                Analyze(context, delegateDeclaration, typeParameterList.Parameters);
        }
    }
}
