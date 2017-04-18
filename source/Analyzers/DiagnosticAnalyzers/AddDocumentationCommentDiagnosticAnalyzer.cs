// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.CSharp.Refactorings.AddDocumentationCommentRefactoring;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AddDocumentationCommentDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddDocumentationComment); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(f => AnalyzeClassDeclaration(f), SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeConstructorDeclaration(f), SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeConversionOperatorDeclaration(f), SyntaxKind.ConversionOperatorDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeDelegateDeclaration(f), SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeDestructorDeclaration(f), SyntaxKind.DestructorDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeEnumDeclaration(f), SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeEventDeclaration(f), SyntaxKind.EventDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeEventFieldDeclaration(f), SyntaxKind.EventFieldDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeFieldDeclaration(f), SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeIndexerDeclaration(f), SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeInterfaceDeclaration(f), SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeMethodDeclaration(f), SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeOperatorDeclaration(f), SyntaxKind.OperatorDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzePropertyDeclaration(f), SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeStructDeclaration(f), SyntaxKind.StructDeclaration);
        }

        private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (ClassDeclarationSyntax)context.Node;

            if (CanRefactor(declaration, context.SemanticModel, context.CancellationToken))
                context.ReportDiagnostic(DiagnosticDescriptors.AddDocumentationComment, declaration);
        }

        private static void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (ConstructorDeclarationSyntax)context.Node;

            if (CanRefactor(declaration, context.SemanticModel, context.CancellationToken))
                context.ReportDiagnostic(DiagnosticDescriptors.AddDocumentationComment, declaration);
        }

        private static void AnalyzeConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (ConversionOperatorDeclarationSyntax)context.Node;

            if (CanRefactor(declaration, context.SemanticModel, context.CancellationToken))
                context.ReportDiagnostic(DiagnosticDescriptors.AddDocumentationComment, declaration);
        }

        private static void AnalyzeDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (DelegateDeclarationSyntax)context.Node;

            if (CanRefactor(declaration, context.SemanticModel, context.CancellationToken))
                context.ReportDiagnostic(DiagnosticDescriptors.AddDocumentationComment, declaration);
        }

        private static void AnalyzeDestructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (DestructorDeclarationSyntax)context.Node;

            if (CanRefactor(declaration, context.SemanticModel, context.CancellationToken))
                context.ReportDiagnostic(DiagnosticDescriptors.AddDocumentationComment, declaration);
        }

        private static void AnalyzeEnumDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (EnumDeclarationSyntax)context.Node;

            if (CanRefactor(declaration, context.SemanticModel, context.CancellationToken))
                context.ReportDiagnostic(DiagnosticDescriptors.AddDocumentationComment, declaration);
        }

        private static void AnalyzeEventDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (EventDeclarationSyntax)context.Node;

            if (CanRefactor(declaration, context.SemanticModel, context.CancellationToken))
                context.ReportDiagnostic(DiagnosticDescriptors.AddDocumentationComment, declaration);
        }

        private static void AnalyzeEventFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (EventFieldDeclarationSyntax)context.Node;

            if (CanRefactor(declaration, context.SemanticModel, context.CancellationToken))
                context.ReportDiagnostic(DiagnosticDescriptors.AddDocumentationComment, declaration);
        }

        private static void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (FieldDeclarationSyntax)context.Node;

            if (CanRefactor(declaration, context.SemanticModel, context.CancellationToken))
                context.ReportDiagnostic(DiagnosticDescriptors.AddDocumentationComment, declaration);
        }

        private static void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (IndexerDeclarationSyntax)context.Node;

            if (CanRefactor(declaration, context.SemanticModel, context.CancellationToken))
                context.ReportDiagnostic(DiagnosticDescriptors.AddDocumentationComment, declaration);
        }

        private static void AnalyzeInterfaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (InterfaceDeclarationSyntax)context.Node;

            if (CanRefactor(declaration, context.SemanticModel, context.CancellationToken))
                context.ReportDiagnostic(DiagnosticDescriptors.AddDocumentationComment, declaration);
        }

        private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (MethodDeclarationSyntax)context.Node;

            if (CanRefactor(declaration, context.SemanticModel, context.CancellationToken))
                context.ReportDiagnostic(DiagnosticDescriptors.AddDocumentationComment, declaration);
        }

        private static void AnalyzeOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (OperatorDeclarationSyntax)context.Node;

            if (CanRefactor(declaration, context.SemanticModel, context.CancellationToken))
                context.ReportDiagnostic(DiagnosticDescriptors.AddDocumentationComment, declaration);
        }

        private static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (PropertyDeclarationSyntax)context.Node;

            if (CanRefactor(declaration, context.SemanticModel, context.CancellationToken))
                context.ReportDiagnostic(DiagnosticDescriptors.AddDocumentationComment, declaration);
        }

        private static void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (StructDeclarationSyntax)context.Node;

            if (CanRefactor(declaration, context.SemanticModel, context.CancellationToken))
                context.ReportDiagnostic(DiagnosticDescriptors.AddDocumentationComment, declaration);
        }
    }
}
