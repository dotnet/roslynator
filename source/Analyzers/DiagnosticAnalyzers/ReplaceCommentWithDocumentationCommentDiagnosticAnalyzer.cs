// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using static Roslynator.CSharp.Refactorings.ReplaceCommentWithDocumentationCommentRefactoring;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReplaceCommentWithDocumentationCommentDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.ReplaceCommentWithDocumentationComment); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeNamespaceDeclaration, SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeStructDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeInterfaceDeclaration, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeEnumDeclaration, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeEnumMemberDeclaration, SyntaxKind.EnumMemberDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeDelegateDeclaration, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeDestructorDeclaration, SyntaxKind.DestructorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeOperatorDeclaration, SyntaxKind.OperatorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeConversionOperatorDeclaration, SyntaxKind.ConversionOperatorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeIndexerDeclaration, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeFieldDeclaration, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeEventFieldDeclaration, SyntaxKind.EventFieldDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeEventDeclaration, SyntaxKind.EventDeclaration);
        }

        private static void AnalyzeNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingTriviaAnalysis lta = AnalyzeLeadingTrivia(context.Node);

            if (!lta.Span.IsEmpty)
            {
                ReportDiagnostic(context, lta.Span);
                return;
            }

            if (lta.HasDocumentationComment)
                return;

            var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;

            TrailingTriviaAnalysis trailingAnalysis = AnalyzeTrailingTrivia(namespaceDeclaration.Name);

            if (!trailingAnalysis.Span.IsEmpty)
                ReportDiagnostic(context, trailingAnalysis.Span);
        }

        private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingTriviaAnalysis lta = AnalyzeLeadingTrivia(context.Node);

            if (!lta.Span.IsEmpty)
            {
                ReportDiagnostic(context, lta.Span);
                return;
            }

            if (lta.HasDocumentationComment)
                return;

            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            TrailingTriviaAnalysis tta = AnalyzeTrailingTrivia(classDeclaration.Identifier);

            if (tta.ContainsEndOfLine)
                return;

            if (tta.Span.IsEmpty)
                tta = AnalyzeTrailingTrivia(classDeclaration.TypeParameterList);

            if (tta.ContainsEndOfLine)
                return;

            if (tta.Span.IsEmpty)
                tta = AnalyzeTrailingTrivia(classDeclaration.BaseList);

            if (tta.ContainsEndOfLine)
                return;

            if (tta.Span.IsEmpty)
                tta = AnalyzeTrailingTrivia(classDeclaration.ConstraintClauses);

            if (!tta.Span.IsEmpty)
                ReportDiagnostic(context, tta.Span);
        }

        private static void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingTriviaAnalysis lta = AnalyzeLeadingTrivia(context.Node);

            if (!lta.Span.IsEmpty)
            {
                ReportDiagnostic(context, lta.Span);
                return;
            }

            if (lta.HasDocumentationComment)
                return;

            var structDeclaration = (StructDeclarationSyntax)context.Node;

            TrailingTriviaAnalysis tta = AnalyzeTrailingTrivia(structDeclaration.Identifier);

            if (tta.ContainsEndOfLine)
                return;

            if (tta.Span.IsEmpty)
                tta = AnalyzeTrailingTrivia(structDeclaration.TypeParameterList);

            if (tta.ContainsEndOfLine)
                return;

            if (tta.Span.IsEmpty)
                tta = AnalyzeTrailingTrivia(structDeclaration.BaseList);

            if (tta.ContainsEndOfLine)
                return;

            if (tta.Span.IsEmpty)
                tta = AnalyzeTrailingTrivia(structDeclaration.ConstraintClauses);

            if (!tta.Span.IsEmpty)
                ReportDiagnostic(context, tta.Span);
        }

        private static void AnalyzeInterfaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingTriviaAnalysis lta = AnalyzeLeadingTrivia(context.Node);

            if (!lta.Span.IsEmpty)
            {
                ReportDiagnostic(context, lta.Span);
                return;
            }

            if (lta.HasDocumentationComment)
                return;

            var interfaceDeclaration = (InterfaceDeclarationSyntax)context.Node;

            TrailingTriviaAnalysis tta = AnalyzeTrailingTrivia(interfaceDeclaration.Identifier);

            if (tta.ContainsEndOfLine)
                return;

            if (tta.Span.IsEmpty)
                tta = AnalyzeTrailingTrivia(interfaceDeclaration.TypeParameterList);

            if (tta.ContainsEndOfLine)
                return;

            if (tta.Span.IsEmpty)
                tta = AnalyzeTrailingTrivia(interfaceDeclaration.BaseList);

            if (tta.ContainsEndOfLine)
                return;

            if (tta.Span.IsEmpty)
                tta = AnalyzeTrailingTrivia(interfaceDeclaration.ConstraintClauses);

            if (!tta.Span.IsEmpty)
                ReportDiagnostic(context, tta.Span);
        }

        private static void AnalyzeEnumDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingTriviaAnalysis lta = AnalyzeLeadingTrivia(context.Node);

            if (!lta.Span.IsEmpty)
            {
                ReportDiagnostic(context, lta.Span);
                return;
            }

            if (lta.HasDocumentationComment)
                return;

            var enumDeclaration = (EnumDeclarationSyntax)context.Node;

            TrailingTriviaAnalysis tta = AnalyzeTrailingTrivia(enumDeclaration.Identifier);

            if (!tta.Span.IsEmpty)
                ReportDiagnostic(context, tta.Span);
        }

        private static void AnalyzeEnumMemberDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingTriviaAnalysis lta = AnalyzeLeadingTrivia(context.Node);

            if (!lta.Span.IsEmpty)
            {
                ReportDiagnostic(context, lta.Span);
                return;
            }

            if (lta.HasDocumentationComment)
                return;

            var enumMemberDeclaration = (EnumMemberDeclarationSyntax)context.Node;

            TrailingTriviaAnalysis tta = AnalyzeTrailingTrivia(enumMemberDeclaration);

            if (!tta.Span.IsEmpty)
                ReportDiagnostic(context, tta.Span);
        }

        private static void AnalyzeDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingTriviaAnalysis lta = AnalyzeLeadingTrivia(context.Node);

            if (!lta.Span.IsEmpty)
            {
                ReportDiagnostic(context, lta.Span);
                return;
            }

            if (lta.HasDocumentationComment)
                return;

            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;

            TrailingTriviaAnalysis tta = AnalyzeTrailingTrivia(delegateDeclaration);

            if (!tta.Span.IsEmpty)
                ReportDiagnostic(context, tta.Span);
        }

        private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingTriviaAnalysis lta = AnalyzeLeadingTrivia(context.Node);

            if (!lta.Span.IsEmpty)
            {
                ReportDiagnostic(context, lta.Span);
                return;
            }

            if (lta.HasDocumentationComment)
                return;

            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            TrailingTriviaAnalysis tta = AnalyzeTrailingTrivia(methodDeclaration.ParameterList);

            if (tta.ContainsEndOfLine)
                return;

            if (tta.Span.IsEmpty)
            {
                tta = AnalyzeTrailingTrivia(methodDeclaration.ConstraintClauses);

                if (tta.ContainsEndOfLine)
                    return;

                if (tta.Span.IsEmpty)
                {
                    ArrowExpressionClauseSyntax expressionBody = methodDeclaration.ExpressionBody;

                    if (expressionBody != null)
                    {
                        tta = AnalyzeTrailingTrivia(expressionBody.ArrowToken);

                        if (tta.ContainsEndOfLine)
                            return;
                    }
                }
            }

            if (tta.Span.IsEmpty)
                tta = AnalyzeTrailingTrivia(methodDeclaration);

            if (!tta.Span.IsEmpty)
                ReportDiagnostic(context, tta.Span);
        }

        private static void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingTriviaAnalysis lta = AnalyzeLeadingTrivia(context.Node);

            if (!lta.Span.IsEmpty)
            {
                ReportDiagnostic(context, lta.Span);
                return;
            }

            if (lta.HasDocumentationComment)
                return;

            var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;

            TrailingTriviaAnalysis tta = AnalyzeTrailingTrivia(constructorDeclaration.ParameterList);

            if (tta.ContainsEndOfLine)
                return;

            if (tta.Span.IsEmpty)
            {
                tta = AnalyzeTrailingTrivia(constructorDeclaration.Initializer);

                if (tta.ContainsEndOfLine)
                    return;
            }

            if (tta.Span.IsEmpty)
                tta = AnalyzeTrailingTrivia(constructorDeclaration);

            if (!tta.Span.IsEmpty)
                ReportDiagnostic(context, tta.Span);
        }

        private static void AnalyzeDestructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingTriviaAnalysis lta = AnalyzeLeadingTrivia(context.Node);

            if (!lta.Span.IsEmpty)
            {
                ReportDiagnostic(context, lta.Span);
                return;
            }

            if (lta.HasDocumentationComment)
                return;

            var destructorDeclaration = (DestructorDeclarationSyntax)context.Node;

            TrailingTriviaAnalysis tta = AnalyzeTrailingTrivia(destructorDeclaration.ParameterList);

            if (tta.ContainsEndOfLine)
                return;

            if (tta.Span.IsEmpty)
                tta = AnalyzeTrailingTrivia(destructorDeclaration);

            if (!tta.Span.IsEmpty)
                ReportDiagnostic(context, tta.Span);
        }

        private static void AnalyzeOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingTriviaAnalysis lta = AnalyzeLeadingTrivia(context.Node);

            if (!lta.Span.IsEmpty)
            {
                ReportDiagnostic(context, lta.Span);
                return;
            }

            if (lta.HasDocumentationComment)
                return;

            var operatorDeclaration = (OperatorDeclarationSyntax)context.Node;

            TrailingTriviaAnalysis tta = AnalyzeTrailingTrivia(operatorDeclaration.ParameterList);

            if (tta.ContainsEndOfLine)
                return;

            if (tta.Span.IsEmpty)
            {
                ArrowExpressionClauseSyntax expressionBody = operatorDeclaration.ExpressionBody;

                if (expressionBody != null)
                {
                    tta = AnalyzeTrailingTrivia(expressionBody.ArrowToken);

                    if (tta.ContainsEndOfLine)
                        return;
                }
            }

            if (tta.Span.IsEmpty)
                tta = AnalyzeTrailingTrivia(operatorDeclaration);

            if (!tta.Span.IsEmpty)
                ReportDiagnostic(context, tta.Span);
        }

        private static void AnalyzeConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingTriviaAnalysis lta = AnalyzeLeadingTrivia(context.Node);

            if (!lta.Span.IsEmpty)
            {
                ReportDiagnostic(context, lta.Span);
                return;
            }

            if (lta.HasDocumentationComment)
                return;

            var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax)context.Node;

            TrailingTriviaAnalysis tta = AnalyzeTrailingTrivia(conversionOperatorDeclaration.ParameterList);

            if (tta.ContainsEndOfLine)
                return;

            if (tta.Span.IsEmpty)
            {
                ArrowExpressionClauseSyntax expressionBody = conversionOperatorDeclaration.ExpressionBody;

                if (expressionBody != null)
                {
                    tta = AnalyzeTrailingTrivia(expressionBody.ArrowToken);

                    if (tta.ContainsEndOfLine)
                        return;
                }
            }

            if (tta.Span.IsEmpty)
                tta = AnalyzeTrailingTrivia(conversionOperatorDeclaration);

            if (!tta.Span.IsEmpty)
                ReportDiagnostic(context, tta.Span);
        }

        private static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingTriviaAnalysis lta = AnalyzeLeadingTrivia(context.Node);

            if (!lta.Span.IsEmpty)
            {
                ReportDiagnostic(context, lta.Span);
                return;
            }

            if (lta.HasDocumentationComment)
                return;

            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

            TrailingTriviaAnalysis tta = AnalyzeTrailingTrivia(propertyDeclaration.Identifier);

            if (tta.ContainsEndOfLine)
                return;

            if (tta.Span.IsEmpty)
            {
                ArrowExpressionClauseSyntax expressionBody = propertyDeclaration.ExpressionBody;

                if (expressionBody != null)
                {
                    tta = AnalyzeTrailingTrivia(expressionBody.ArrowToken);

                    if (tta.ContainsEndOfLine)
                        return;
                }
            }

            if (tta.Span.IsEmpty)
                tta = AnalyzeTrailingTrivia(propertyDeclaration);

            if (!tta.Span.IsEmpty)
                ReportDiagnostic(context, tta.Span);
        }

        private static void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingTriviaAnalysis lta = AnalyzeLeadingTrivia(context.Node);

            if (!lta.Span.IsEmpty)
            {
                ReportDiagnostic(context, lta.Span);
                return;
            }

            if (lta.HasDocumentationComment)
                return;

            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            TrailingTriviaAnalysis tta = AnalyzeTrailingTrivia(indexerDeclaration.ParameterList);

            if (tta.ContainsEndOfLine)
                return;

            if (tta.Span.IsEmpty)
            {
                ArrowExpressionClauseSyntax expressionBody = indexerDeclaration.ExpressionBody;

                if (expressionBody != null)
                {
                    tta = AnalyzeTrailingTrivia(expressionBody.ArrowToken);

                    if (tta.ContainsEndOfLine)
                        return;
                }
            }

            if (tta.Span.IsEmpty)
                tta = AnalyzeTrailingTrivia(indexerDeclaration);

            if (!tta.Span.IsEmpty)
                ReportDiagnostic(context, tta.Span);
        }

        private static void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingTriviaAnalysis lta = AnalyzeLeadingTrivia(context.Node);

            if (!lta.Span.IsEmpty)
            {
                ReportDiagnostic(context, lta.Span);
                return;
            }

            if (lta.HasDocumentationComment)
                return;

            var fieldDeclaration = (FieldDeclarationSyntax)context.Node;

            TrailingTriviaAnalysis tta = AnalyzeTrailingTrivia(fieldDeclaration);

            if (!tta.Span.IsEmpty)
                ReportDiagnostic(context, tta.Span);
        }

        private static void AnalyzeEventFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingTriviaAnalysis lta = AnalyzeLeadingTrivia(context.Node);

            if (!lta.Span.IsEmpty)
            {
                ReportDiagnostic(context, lta.Span);
                return;
            }

            if (lta.HasDocumentationComment)
                return;

            var eventFieldDeclaration = (EventFieldDeclarationSyntax)context.Node;

            TrailingTriviaAnalysis tta = AnalyzeTrailingTrivia(eventFieldDeclaration);

            if (!tta.Span.IsEmpty)
                ReportDiagnostic(context, tta.Span);
        }

        private static void AnalyzeEventDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingTriviaAnalysis lta = AnalyzeLeadingTrivia(context.Node);

            if (!lta.Span.IsEmpty)
            {
                ReportDiagnostic(context, lta.Span);
                return;
            }

            if (lta.HasDocumentationComment)
                return;

            var eventDeclaration = (EventDeclarationSyntax)context.Node;

            TrailingTriviaAnalysis tta = AnalyzeTrailingTrivia(eventDeclaration.Identifier);

            if (tta.ContainsEndOfLine)
                return;

            if (tta.Span.IsEmpty)
                tta = AnalyzeTrailingTrivia(eventDeclaration.AccessorList);

            if (!tta.Span.IsEmpty)
                ReportDiagnostic(context, tta.Span);
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, TextSpan span)
        {
            context.ReportDiagnostic(
                DiagnosticDescriptors.ReplaceCommentWithDocumentationComment,
                Location.Create(context.SyntaxTree(), span));
        }
    }
}
