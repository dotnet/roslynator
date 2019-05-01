// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using static Roslynator.CSharp.Analysis.ConvertCommentToDocumentationCommentAnalysis;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConvertCommentToDocumentationCommentAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.ConvertCommentToDocumentationComment); }
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
            LeadingAnalysis leadingAnalysis = AnalyzeLeadingTrivia(context.Node);

            if (!leadingAnalysis.Span.IsEmpty)
            {
                ReportDiagnostic(context, leadingAnalysis.Span);
                return;
            }

            if (leadingAnalysis.HasDocumentationComment)
                return;

            var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;

            TrailingAnalysis trailingAnalysis = AnalyzeTrailingTrivia(namespaceDeclaration.Name);

            if (!trailingAnalysis.Span.IsEmpty)
                ReportDiagnostic(context, trailingAnalysis.Span);
        }

        private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingAnalysis leadingAnalysis = AnalyzeLeadingTrivia(context.Node);

            if (!leadingAnalysis.Span.IsEmpty)
            {
                ReportDiagnostic(context, leadingAnalysis.Span);
                return;
            }

            if (leadingAnalysis.HasDocumentationComment)
                return;

            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            TrailingAnalysis trailingAnalysis = AnalyzeTrailingTrivia(classDeclaration.Identifier);

            if (trailingAnalysis.ContainsEndOfLine)
                return;

            if (trailingAnalysis.Span.IsEmpty)
                trailingAnalysis = AnalyzeTrailingTrivia(classDeclaration.TypeParameterList);

            if (trailingAnalysis.ContainsEndOfLine)
                return;

            if (trailingAnalysis.Span.IsEmpty)
                trailingAnalysis = AnalyzeTrailingTrivia(classDeclaration.BaseList);

            if (trailingAnalysis.ContainsEndOfLine)
                return;

            if (trailingAnalysis.Span.IsEmpty)
                trailingAnalysis = AnalyzeTrailingTrivia(classDeclaration.ConstraintClauses);

            if (!trailingAnalysis.Span.IsEmpty)
                ReportDiagnostic(context, trailingAnalysis.Span);
        }

        private static void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingAnalysis leadingAnalysis = AnalyzeLeadingTrivia(context.Node);

            if (!leadingAnalysis.Span.IsEmpty)
            {
                ReportDiagnostic(context, leadingAnalysis.Span);
                return;
            }

            if (leadingAnalysis.HasDocumentationComment)
                return;

            var structDeclaration = (StructDeclarationSyntax)context.Node;

            TrailingAnalysis trailingAnalysis = AnalyzeTrailingTrivia(structDeclaration.Identifier);

            if (trailingAnalysis.ContainsEndOfLine)
                return;

            if (trailingAnalysis.Span.IsEmpty)
                trailingAnalysis = AnalyzeTrailingTrivia(structDeclaration.TypeParameterList);

            if (trailingAnalysis.ContainsEndOfLine)
                return;

            if (trailingAnalysis.Span.IsEmpty)
                trailingAnalysis = AnalyzeTrailingTrivia(structDeclaration.BaseList);

            if (trailingAnalysis.ContainsEndOfLine)
                return;

            if (trailingAnalysis.Span.IsEmpty)
                trailingAnalysis = AnalyzeTrailingTrivia(structDeclaration.ConstraintClauses);

            if (!trailingAnalysis.Span.IsEmpty)
                ReportDiagnostic(context, trailingAnalysis.Span);
        }

        private static void AnalyzeInterfaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingAnalysis leadingAnalysis = AnalyzeLeadingTrivia(context.Node);

            if (!leadingAnalysis.Span.IsEmpty)
            {
                ReportDiagnostic(context, leadingAnalysis.Span);
                return;
            }

            if (leadingAnalysis.HasDocumentationComment)
                return;

            var interfaceDeclaration = (InterfaceDeclarationSyntax)context.Node;

            TrailingAnalysis trailingAnalysis = AnalyzeTrailingTrivia(interfaceDeclaration.Identifier);

            if (trailingAnalysis.ContainsEndOfLine)
                return;

            if (trailingAnalysis.Span.IsEmpty)
                trailingAnalysis = AnalyzeTrailingTrivia(interfaceDeclaration.TypeParameterList);

            if (trailingAnalysis.ContainsEndOfLine)
                return;

            if (trailingAnalysis.Span.IsEmpty)
                trailingAnalysis = AnalyzeTrailingTrivia(interfaceDeclaration.BaseList);

            if (trailingAnalysis.ContainsEndOfLine)
                return;

            if (trailingAnalysis.Span.IsEmpty)
                trailingAnalysis = AnalyzeTrailingTrivia(interfaceDeclaration.ConstraintClauses);

            if (!trailingAnalysis.Span.IsEmpty)
                ReportDiagnostic(context, trailingAnalysis.Span);
        }

        private static void AnalyzeEnumDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingAnalysis leadingAnalysis = AnalyzeLeadingTrivia(context.Node);

            if (!leadingAnalysis.Span.IsEmpty)
            {
                ReportDiagnostic(context, leadingAnalysis.Span);
                return;
            }

            if (leadingAnalysis.HasDocumentationComment)
                return;

            var enumDeclaration = (EnumDeclarationSyntax)context.Node;

            TrailingAnalysis trailingAnalysis = AnalyzeTrailingTrivia(enumDeclaration.Identifier);

            if (!trailingAnalysis.Span.IsEmpty)
                ReportDiagnostic(context, trailingAnalysis.Span);
        }

        private static void AnalyzeEnumMemberDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingAnalysis leadingAnalysis = AnalyzeLeadingTrivia(context.Node);

            if (!leadingAnalysis.Span.IsEmpty)
            {
                ReportDiagnostic(context, leadingAnalysis.Span);
                return;
            }

            if (leadingAnalysis.HasDocumentationComment)
                return;

            var enumMemberDeclaration = (EnumMemberDeclarationSyntax)context.Node;

            TrailingAnalysis trailingAnalysis = AnalyzeTrailingTrivia(enumMemberDeclaration);

            if (!trailingAnalysis.Span.IsEmpty)
                ReportDiagnostic(context, trailingAnalysis.Span);
        }

        private static void AnalyzeDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingAnalysis leadingAnalysis = AnalyzeLeadingTrivia(context.Node);

            if (!leadingAnalysis.Span.IsEmpty)
            {
                ReportDiagnostic(context, leadingAnalysis.Span);
                return;
            }

            if (leadingAnalysis.HasDocumentationComment)
                return;

            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;

            TrailingAnalysis trailingAnalysis = AnalyzeTrailingTrivia(delegateDeclaration);

            if (!trailingAnalysis.Span.IsEmpty)
                ReportDiagnostic(context, trailingAnalysis.Span);
        }

        private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingAnalysis leadingAnalysis = AnalyzeLeadingTrivia(context.Node);

            if (!leadingAnalysis.Span.IsEmpty)
            {
                ReportDiagnostic(context, leadingAnalysis.Span);
                return;
            }

            if (leadingAnalysis.HasDocumentationComment)
                return;

            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            TrailingAnalysis trailingAnalysis = AnalyzeTrailingTrivia(methodDeclaration.ParameterList);

            if (trailingAnalysis.ContainsEndOfLine)
                return;

            if (trailingAnalysis.Span.IsEmpty)
            {
                trailingAnalysis = AnalyzeTrailingTrivia(methodDeclaration.ConstraintClauses);

                if (trailingAnalysis.ContainsEndOfLine)
                    return;

                if (trailingAnalysis.Span.IsEmpty)
                {
                    ArrowExpressionClauseSyntax expressionBody = methodDeclaration.ExpressionBody;

                    if (expressionBody != null)
                    {
                        trailingAnalysis = AnalyzeTrailingTrivia(expressionBody.ArrowToken);

                        if (trailingAnalysis.ContainsEndOfLine)
                            return;
                    }
                }
            }

            if (trailingAnalysis.Span.IsEmpty)
                trailingAnalysis = AnalyzeTrailingTrivia(methodDeclaration);

            if (!trailingAnalysis.Span.IsEmpty)
                ReportDiagnostic(context, trailingAnalysis.Span);
        }

        private static void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingAnalysis leadingAnalysis = AnalyzeLeadingTrivia(context.Node);

            if (!leadingAnalysis.Span.IsEmpty)
            {
                ReportDiagnostic(context, leadingAnalysis.Span);
                return;
            }

            if (leadingAnalysis.HasDocumentationComment)
                return;

            var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;

            TrailingAnalysis trailingAnalysis = AnalyzeTrailingTrivia(constructorDeclaration.ParameterList);

            if (trailingAnalysis.ContainsEndOfLine)
                return;

            if (trailingAnalysis.Span.IsEmpty)
            {
                trailingAnalysis = AnalyzeTrailingTrivia(constructorDeclaration.Initializer);

                if (trailingAnalysis.ContainsEndOfLine)
                    return;

                if (trailingAnalysis.Span.IsEmpty)
                {
                    ArrowExpressionClauseSyntax expressionBody = constructorDeclaration.ExpressionBody;

                    if (expressionBody != null)
                    {
                        trailingAnalysis = AnalyzeTrailingTrivia(expressionBody.ArrowToken);

                        if (trailingAnalysis.ContainsEndOfLine)
                            return;
                    }
                }
            }

            if (trailingAnalysis.Span.IsEmpty)
                trailingAnalysis = AnalyzeTrailingTrivia(constructorDeclaration);

            if (!trailingAnalysis.Span.IsEmpty)
                ReportDiagnostic(context, trailingAnalysis.Span);
        }

        private static void AnalyzeDestructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingAnalysis leadingAnalysis = AnalyzeLeadingTrivia(context.Node);

            if (!leadingAnalysis.Span.IsEmpty)
            {
                ReportDiagnostic(context, leadingAnalysis.Span);
                return;
            }

            if (leadingAnalysis.HasDocumentationComment)
                return;

            var destructorDeclaration = (DestructorDeclarationSyntax)context.Node;

            TrailingAnalysis trailingAnalysis = AnalyzeTrailingTrivia(destructorDeclaration.ParameterList);

            if (trailingAnalysis.ContainsEndOfLine)
                return;

            if (trailingAnalysis.Span.IsEmpty)
            {
                ArrowExpressionClauseSyntax expressionBody = destructorDeclaration.ExpressionBody;

                if (expressionBody != null)
                {
                    trailingAnalysis = AnalyzeTrailingTrivia(expressionBody.ArrowToken);

                    if (trailingAnalysis.ContainsEndOfLine)
                        return;
                }
            }

            if (trailingAnalysis.Span.IsEmpty)
                trailingAnalysis = AnalyzeTrailingTrivia(destructorDeclaration);

            if (!trailingAnalysis.Span.IsEmpty)
                ReportDiagnostic(context, trailingAnalysis.Span);
        }

        private static void AnalyzeOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingAnalysis leadingAnalysis = AnalyzeLeadingTrivia(context.Node);

            if (!leadingAnalysis.Span.IsEmpty)
            {
                ReportDiagnostic(context, leadingAnalysis.Span);
                return;
            }

            if (leadingAnalysis.HasDocumentationComment)
                return;

            var operatorDeclaration = (OperatorDeclarationSyntax)context.Node;

            TrailingAnalysis trailingAnalysis = AnalyzeTrailingTrivia(operatorDeclaration.ParameterList);

            if (trailingAnalysis.ContainsEndOfLine)
                return;

            if (trailingAnalysis.Span.IsEmpty)
            {
                ArrowExpressionClauseSyntax expressionBody = operatorDeclaration.ExpressionBody;

                if (expressionBody != null)
                {
                    trailingAnalysis = AnalyzeTrailingTrivia(expressionBody.ArrowToken);

                    if (trailingAnalysis.ContainsEndOfLine)
                        return;
                }
            }

            if (trailingAnalysis.Span.IsEmpty)
                trailingAnalysis = AnalyzeTrailingTrivia(operatorDeclaration);

            if (!trailingAnalysis.Span.IsEmpty)
                ReportDiagnostic(context, trailingAnalysis.Span);
        }

        private static void AnalyzeConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingAnalysis leadingAnalysis = AnalyzeLeadingTrivia(context.Node);

            if (!leadingAnalysis.Span.IsEmpty)
            {
                ReportDiagnostic(context, leadingAnalysis.Span);
                return;
            }

            if (leadingAnalysis.HasDocumentationComment)
                return;

            var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax)context.Node;

            TrailingAnalysis trailingAnalysis = AnalyzeTrailingTrivia(conversionOperatorDeclaration.ParameterList);

            if (trailingAnalysis.ContainsEndOfLine)
                return;

            if (trailingAnalysis.Span.IsEmpty)
            {
                ArrowExpressionClauseSyntax expressionBody = conversionOperatorDeclaration.ExpressionBody;

                if (expressionBody != null)
                {
                    trailingAnalysis = AnalyzeTrailingTrivia(expressionBody.ArrowToken);

                    if (trailingAnalysis.ContainsEndOfLine)
                        return;
                }
            }

            if (trailingAnalysis.Span.IsEmpty)
                trailingAnalysis = AnalyzeTrailingTrivia(conversionOperatorDeclaration);

            if (!trailingAnalysis.Span.IsEmpty)
                ReportDiagnostic(context, trailingAnalysis.Span);
        }

        private static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingAnalysis leadingAnalysis = AnalyzeLeadingTrivia(context.Node);

            if (!leadingAnalysis.Span.IsEmpty)
            {
                ReportDiagnostic(context, leadingAnalysis.Span);
                return;
            }

            if (leadingAnalysis.HasDocumentationComment)
                return;

            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

            TrailingAnalysis trailingAnalysis = AnalyzeTrailingTrivia(propertyDeclaration.Identifier);

            if (trailingAnalysis.ContainsEndOfLine)
                return;

            if (trailingAnalysis.Span.IsEmpty)
            {
                ArrowExpressionClauseSyntax expressionBody = propertyDeclaration.ExpressionBody;

                if (expressionBody != null)
                {
                    trailingAnalysis = AnalyzeTrailingTrivia(expressionBody.ArrowToken);

                    if (trailingAnalysis.ContainsEndOfLine)
                        return;
                }
            }

            if (trailingAnalysis.Span.IsEmpty)
                trailingAnalysis = AnalyzeTrailingTrivia(propertyDeclaration);

            if (!trailingAnalysis.Span.IsEmpty)
                ReportDiagnostic(context, trailingAnalysis.Span);
        }

        private static void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingAnalysis leadingAnalysis = AnalyzeLeadingTrivia(context.Node);

            if (!leadingAnalysis.Span.IsEmpty)
            {
                ReportDiagnostic(context, leadingAnalysis.Span);
                return;
            }

            if (leadingAnalysis.HasDocumentationComment)
                return;

            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            TrailingAnalysis trailingAnalysis = AnalyzeTrailingTrivia(indexerDeclaration.ParameterList);

            if (trailingAnalysis.ContainsEndOfLine)
                return;

            if (trailingAnalysis.Span.IsEmpty)
            {
                ArrowExpressionClauseSyntax expressionBody = indexerDeclaration.ExpressionBody;

                if (expressionBody != null)
                {
                    trailingAnalysis = AnalyzeTrailingTrivia(expressionBody.ArrowToken);

                    if (trailingAnalysis.ContainsEndOfLine)
                        return;
                }
            }

            if (trailingAnalysis.Span.IsEmpty)
                trailingAnalysis = AnalyzeTrailingTrivia(indexerDeclaration);

            if (!trailingAnalysis.Span.IsEmpty)
                ReportDiagnostic(context, trailingAnalysis.Span);
        }

        private static void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingAnalysis leadingAnalysis = AnalyzeLeadingTrivia(context.Node);

            if (!leadingAnalysis.Span.IsEmpty)
            {
                ReportDiagnostic(context, leadingAnalysis.Span);
                return;
            }

            if (leadingAnalysis.HasDocumentationComment)
                return;

            var fieldDeclaration = (FieldDeclarationSyntax)context.Node;

            TrailingAnalysis trailingAnalysis = AnalyzeTrailingTrivia(fieldDeclaration);

            if (!trailingAnalysis.Span.IsEmpty)
                ReportDiagnostic(context, trailingAnalysis.Span);
        }

        private static void AnalyzeEventFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingAnalysis leadingAnalysis = AnalyzeLeadingTrivia(context.Node);

            if (!leadingAnalysis.Span.IsEmpty)
            {
                ReportDiagnostic(context, leadingAnalysis.Span);
                return;
            }

            if (leadingAnalysis.HasDocumentationComment)
                return;

            var eventFieldDeclaration = (EventFieldDeclarationSyntax)context.Node;

            TrailingAnalysis trailingAnalysis = AnalyzeTrailingTrivia(eventFieldDeclaration);

            if (!trailingAnalysis.Span.IsEmpty)
                ReportDiagnostic(context, trailingAnalysis.Span);
        }

        private static void AnalyzeEventDeclaration(SyntaxNodeAnalysisContext context)
        {
            LeadingAnalysis leadingAnalysis = AnalyzeLeadingTrivia(context.Node);

            if (!leadingAnalysis.Span.IsEmpty)
            {
                ReportDiagnostic(context, leadingAnalysis.Span);
                return;
            }

            if (leadingAnalysis.HasDocumentationComment)
                return;

            var eventDeclaration = (EventDeclarationSyntax)context.Node;

            TrailingAnalysis trailingAnalysis = AnalyzeTrailingTrivia(eventDeclaration.Identifier);

            if (trailingAnalysis.ContainsEndOfLine)
                return;

            if (trailingAnalysis.Span.IsEmpty)
                trailingAnalysis = AnalyzeTrailingTrivia(eventDeclaration.AccessorList);

            if (!trailingAnalysis.Span.IsEmpty)
                ReportDiagnostic(context, trailingAnalysis.Span);
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, TextSpan span)
        {
            DiagnosticHelpers.ReportDiagnostic(context,
                DiagnosticDescriptors.ConvertCommentToDocumentationComment,
                Location.Create(context.Node.SyntaxTree, span));
        }
    }
}

