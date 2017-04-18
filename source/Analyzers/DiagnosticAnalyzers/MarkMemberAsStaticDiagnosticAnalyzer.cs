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
    public class MarkMemberAsStaticDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.MarkMemberAsStatic); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeFieldDeclaration(f), SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeMethodDeclaration(f), SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzePropertyDeclaration(f), SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeEventDeclaration(f), SyntaxKind.EventDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeEventFieldDeclaration(f), SyntaxKind.EventFieldDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeConstructorDeclaration(f), SyntaxKind.ConstructorDeclaration);
        }

        private void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var fieldDeclaration = (FieldDeclarationSyntax)context.Node;

            if (MarkMemberAsStaticRefactoring.CanRefactor(fieldDeclaration))
            {
                VariableDeclaratorSyntax declarator = fieldDeclaration.Declaration?.SingleVariableOrDefault();

                if (declarator != null)
                {
                    context.ReportDiagnostic(
                       DiagnosticDescriptors.MarkMemberAsStatic,
                       declarator.Identifier);
                }
            }
        }

        private void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            if (MarkMemberAsStaticRefactoring.CanRefactor(methodDeclaration))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.MarkMemberAsStatic,
                    methodDeclaration.Identifier);
            }
        }

        private void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

            if (MarkMemberAsStaticRefactoring.CanRefactor(propertyDeclaration))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.MarkMemberAsStatic,
                    propertyDeclaration.Identifier);
            }
        }

        private void AnalyzeEventDeclaration(SyntaxNodeAnalysisContext context)
        {
            var eventDeclaration = (EventDeclarationSyntax)context.Node;

            if (MarkMemberAsStaticRefactoring.CanRefactor(eventDeclaration))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.MarkMemberAsStatic,
                    eventDeclaration.Identifier);
            }
        }

        private void AnalyzeEventFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var eventFieldDeclaration = (EventFieldDeclarationSyntax)context.Node;

            if (MarkMemberAsStaticRefactoring.CanRefactor(eventFieldDeclaration))
            {
                VariableDeclaratorSyntax declarator = eventFieldDeclaration.Declaration?.SingleVariableOrDefault();

                if (declarator != null)
                {
                    context.ReportDiagnostic(
                       DiagnosticDescriptors.MarkMemberAsStatic,
                       declarator.Identifier);
                }
            }
        }

        private void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;

            if (MarkMemberAsStaticRefactoring.CanRefactor(constructorDeclaration))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.MarkMemberAsStatic,
                    constructorDeclaration.Identifier);
            }
        }
    }
}
