// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MarkContainingClassAsAbstractDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.MarkContainingClassAsAbstract); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(f => AnalyzeMethodDeclaration(f), SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzePropertyDeclaration(f), SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeIndexerDeclaration(f), SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeEventFieldDeclaration(f), SyntaxKind.EventFieldDeclaration);
        }

        private void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            if (MarkContainingClassAsAbstractRefactoring.CanRefactor(methodDeclaration))
                context.ReportDiagnostic(DiagnosticDescriptors.MarkContainingClassAsAbstract, methodDeclaration.Identifier);
        }

        private void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

            if (MarkContainingClassAsAbstractRefactoring.CanRefactor(propertyDeclaration))
                ReportDiagnostic(context, propertyDeclaration.AccessorList);
        }

        private void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            if (MarkContainingClassAsAbstractRefactoring.CanRefactor(indexerDeclaration))
                ReportDiagnostic(context, indexerDeclaration.AccessorList);
        }

        private void AnalyzeEventFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var eventFieldDeclaration = (EventFieldDeclarationSyntax)context.Node;

            if (MarkContainingClassAsAbstractRefactoring.CanRefactor(eventFieldDeclaration))
            {
                VariableDeclaratorSyntax declarator = eventFieldDeclaration.Declaration?.SingleVariableOrDefault();

                if (declarator != null)
                    context.ReportDiagnostic(DiagnosticDescriptors.MarkContainingClassAsAbstract, declarator.Identifier);
            }
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, AccessorListSyntax accessorList)
        {
            AccessorDeclarationSyntax accessor = accessorList?.Accessors.FirstOrDefault();

            if (accessor != null)
                context.ReportDiagnostic(DiagnosticDescriptors.MarkContainingClassAsAbstract, accessor.Keyword);
        }
    }
}
