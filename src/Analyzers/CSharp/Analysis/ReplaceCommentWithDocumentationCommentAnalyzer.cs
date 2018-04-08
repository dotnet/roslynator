// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using static Roslynator.CSharp.Analysis.ReplaceCommentWithDocumentationCommentAnalysis;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReplaceCommentWithDocumentationCommentAnalyzer : BaseDiagnosticAnalyzer
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
            (TextSpan span, bool containsDocumentationComment) = AnalyzeLeadingTrivia(context.Node);

            if (!span.IsEmpty)
            {
                ReportDiagnostic(context, span);
                return;
            }

            if (containsDocumentationComment)
                return;

            var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;

            (TextSpan span, bool containsEndOfLine) result = AnalyzeTrailingTrivia(namespaceDeclaration.Name);

            if (!result.span.IsEmpty)
                ReportDiagnostic(context, result.span);
        }

        private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            (TextSpan span, bool containsDocumentationComment) = AnalyzeLeadingTrivia(context.Node);

            if (!span.IsEmpty)
            {
                ReportDiagnostic(context, span);
                return;
            }

            if (containsDocumentationComment)
                return;

            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            (TextSpan span, bool containsEndOfLine) result = AnalyzeTrailingTrivia(classDeclaration.Identifier);

            if (result.containsEndOfLine)
                return;

            if (result.span.IsEmpty)
                result = AnalyzeTrailingTrivia(classDeclaration.TypeParameterList);

            if (result.containsEndOfLine)
                return;

            if (result.span.IsEmpty)
                result = AnalyzeTrailingTrivia(classDeclaration.BaseList);

            if (result.containsEndOfLine)
                return;

            if (result.span.IsEmpty)
                result = AnalyzeTrailingTrivia(classDeclaration.ConstraintClauses);

            if (!result.span.IsEmpty)
                ReportDiagnostic(context, result.span);
        }

        private static void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
        {
            (TextSpan span, bool containsDocumentationComment) = AnalyzeLeadingTrivia(context.Node);

            if (!span.IsEmpty)
            {
                ReportDiagnostic(context, span);
                return;
            }

            if (containsDocumentationComment)
                return;

            var structDeclaration = (StructDeclarationSyntax)context.Node;

            (TextSpan span, bool containsEndOfLine) result = AnalyzeTrailingTrivia(structDeclaration.Identifier);

            if (result.containsEndOfLine)
                return;

            if (result.span.IsEmpty)
                result = AnalyzeTrailingTrivia(structDeclaration.TypeParameterList);

            if (result.containsEndOfLine)
                return;

            if (result.span.IsEmpty)
                result = AnalyzeTrailingTrivia(structDeclaration.BaseList);

            if (result.containsEndOfLine)
                return;

            if (result.span.IsEmpty)
                result = AnalyzeTrailingTrivia(structDeclaration.ConstraintClauses);

            if (!result.span.IsEmpty)
                ReportDiagnostic(context, result.span);
        }

        private static void AnalyzeInterfaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            (TextSpan span, bool containsDocumentationComment) = AnalyzeLeadingTrivia(context.Node);

            if (!span.IsEmpty)
            {
                ReportDiagnostic(context, span);
                return;
            }

            if (containsDocumentationComment)
                return;

            var interfaceDeclaration = (InterfaceDeclarationSyntax)context.Node;

            (TextSpan span, bool containsEndOfLine) result = AnalyzeTrailingTrivia(interfaceDeclaration.Identifier);

            if (result.containsEndOfLine)
                return;

            if (result.span.IsEmpty)
                result = AnalyzeTrailingTrivia(interfaceDeclaration.TypeParameterList);

            if (result.containsEndOfLine)
                return;

            if (result.span.IsEmpty)
                result = AnalyzeTrailingTrivia(interfaceDeclaration.BaseList);

            if (result.containsEndOfLine)
                return;

            if (result.span.IsEmpty)
                result = AnalyzeTrailingTrivia(interfaceDeclaration.ConstraintClauses);

            if (!result.span.IsEmpty)
                ReportDiagnostic(context, result.span);
        }

        private static void AnalyzeEnumDeclaration(SyntaxNodeAnalysisContext context)
        {
            (TextSpan span, bool containsDocumentationComment) = AnalyzeLeadingTrivia(context.Node);

            if (!span.IsEmpty)
            {
                ReportDiagnostic(context, span);
                return;
            }

            if (containsDocumentationComment)
                return;

            var enumDeclaration = (EnumDeclarationSyntax)context.Node;

            (TextSpan span, bool containsEndOfLine) result = AnalyzeTrailingTrivia(enumDeclaration.Identifier);

            if (!result.span.IsEmpty)
                ReportDiagnostic(context, result.span);
        }

        private static void AnalyzeEnumMemberDeclaration(SyntaxNodeAnalysisContext context)
        {
            (TextSpan span, bool containsDocumentationComment) = AnalyzeLeadingTrivia(context.Node);

            if (!span.IsEmpty)
            {
                ReportDiagnostic(context, span);
                return;
            }

            if (containsDocumentationComment)
                return;

            var enumMemberDeclaration = (EnumMemberDeclarationSyntax)context.Node;

            (TextSpan span, bool containsEndOfLine) result = AnalyzeTrailingTrivia(enumMemberDeclaration);

            if (!result.span.IsEmpty)
                ReportDiagnostic(context, result.span);
        }

        private static void AnalyzeDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            (TextSpan span, bool containsDocumentationComment) = AnalyzeLeadingTrivia(context.Node);

            if (!span.IsEmpty)
            {
                ReportDiagnostic(context, span);
                return;
            }

            if (containsDocumentationComment)
                return;

            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;

            (TextSpan span, bool containsEndOfLine) result = AnalyzeTrailingTrivia(delegateDeclaration);

            if (!result.span.IsEmpty)
                ReportDiagnostic(context, result.span);
        }

        private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            (TextSpan span, bool containsDocumentationComment) = AnalyzeLeadingTrivia(context.Node);

            if (!span.IsEmpty)
            {
                ReportDiagnostic(context, span);
                return;
            }

            if (containsDocumentationComment)
                return;

            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            (TextSpan span, bool containsEndOfLine) result = AnalyzeTrailingTrivia(methodDeclaration.ParameterList);

            if (result.containsEndOfLine)
                return;

            if (result.span.IsEmpty)
            {
                result = AnalyzeTrailingTrivia(methodDeclaration.ConstraintClauses);

                if (result.containsEndOfLine)
                    return;

                if (result.span.IsEmpty)
                {
                    ArrowExpressionClauseSyntax expressionBody = methodDeclaration.ExpressionBody;

                    if (expressionBody != null)
                    {
                        result = AnalyzeTrailingTrivia(expressionBody.ArrowToken);

                        if (result.containsEndOfLine)
                            return;
                    }
                }
            }

            if (result.span.IsEmpty)
                result = AnalyzeTrailingTrivia(methodDeclaration);

            if (!result.span.IsEmpty)
                ReportDiagnostic(context, result.span);
        }

        private static void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            (TextSpan span, bool containsDocumentationComment) = AnalyzeLeadingTrivia(context.Node);

            if (!span.IsEmpty)
            {
                ReportDiagnostic(context, span);
                return;
            }

            if (containsDocumentationComment)
                return;

            var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;

            (TextSpan span, bool containsEndOfLine) result = AnalyzeTrailingTrivia(constructorDeclaration.ParameterList);

            if (result.containsEndOfLine)
                return;

            if (result.span.IsEmpty)
            {
                result = AnalyzeTrailingTrivia(constructorDeclaration.Initializer);

                if (result.containsEndOfLine)
                    return;

                if (result.span.IsEmpty)
                {
                    ArrowExpressionClauseSyntax expressionBody = constructorDeclaration.ExpressionBody;

                    if (expressionBody != null)
                    {
                        result = AnalyzeTrailingTrivia(expressionBody.ArrowToken);

                        if (result.containsEndOfLine)
                            return;
                    }
                }
            }

            if (result.span.IsEmpty)
                result = AnalyzeTrailingTrivia(constructorDeclaration);

            if (!result.span.IsEmpty)
                ReportDiagnostic(context, result.span);
        }

        private static void AnalyzeDestructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            (TextSpan span, bool containsDocumentationComment) = AnalyzeLeadingTrivia(context.Node);

            if (!span.IsEmpty)
            {
                ReportDiagnostic(context, span);
                return;
            }

            if (containsDocumentationComment)
                return;

            var destructorDeclaration = (DestructorDeclarationSyntax)context.Node;

            (TextSpan span, bool containsEndOfLine) result = AnalyzeTrailingTrivia(destructorDeclaration.ParameterList);

            if (result.containsEndOfLine)
                return;

            if (result.span.IsEmpty)
            {
                ArrowExpressionClauseSyntax expressionBody = destructorDeclaration.ExpressionBody;

                if (expressionBody != null)
                {
                    result = AnalyzeTrailingTrivia(expressionBody.ArrowToken);

                    if (result.containsEndOfLine)
                        return;
                }
            }

            if (result.span.IsEmpty)
                result = AnalyzeTrailingTrivia(destructorDeclaration);

            if (!result.span.IsEmpty)
                ReportDiagnostic(context, result.span);
        }

        private static void AnalyzeOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            (TextSpan span, bool containsDocumentationComment) = AnalyzeLeadingTrivia(context.Node);

            if (!span.IsEmpty)
            {
                ReportDiagnostic(context, span);
                return;
            }

            if (containsDocumentationComment)
                return;

            var operatorDeclaration = (OperatorDeclarationSyntax)context.Node;

            (TextSpan span, bool containsEndOfLine) result = AnalyzeTrailingTrivia(operatorDeclaration.ParameterList);

            if (result.containsEndOfLine)
                return;

            if (result.span.IsEmpty)
            {
                ArrowExpressionClauseSyntax expressionBody = operatorDeclaration.ExpressionBody;

                if (expressionBody != null)
                {
                    result = AnalyzeTrailingTrivia(expressionBody.ArrowToken);

                    if (result.containsEndOfLine)
                        return;
                }
            }

            if (result.span.IsEmpty)
                result = AnalyzeTrailingTrivia(operatorDeclaration);

            if (!result.span.IsEmpty)
                ReportDiagnostic(context, result.span);
        }

        private static void AnalyzeConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            (TextSpan span, bool containsDocumentationComment) = AnalyzeLeadingTrivia(context.Node);

            if (!span.IsEmpty)
            {
                ReportDiagnostic(context, span);
                return;
            }

            if (containsDocumentationComment)
                return;

            var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax)context.Node;

            (TextSpan span, bool containsEndOfLine) result = AnalyzeTrailingTrivia(conversionOperatorDeclaration.ParameterList);

            if (result.containsEndOfLine)
                return;

            if (result.span.IsEmpty)
            {
                ArrowExpressionClauseSyntax expressionBody = conversionOperatorDeclaration.ExpressionBody;

                if (expressionBody != null)
                {
                    result = AnalyzeTrailingTrivia(expressionBody.ArrowToken);

                    if (result.containsEndOfLine)
                        return;
                }
            }

            if (result.span.IsEmpty)
                result = AnalyzeTrailingTrivia(conversionOperatorDeclaration);

            if (!result.span.IsEmpty)
                ReportDiagnostic(context, result.span);
        }

        private static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            (TextSpan span, bool containsDocumentationComment) = AnalyzeLeadingTrivia(context.Node);

            if (!span.IsEmpty)
            {
                ReportDiagnostic(context, span);
                return;
            }

            if (containsDocumentationComment)
                return;

            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

            (TextSpan span, bool containsEndOfLine) result = AnalyzeTrailingTrivia(propertyDeclaration.Identifier);

            if (result.containsEndOfLine)
                return;

            if (result.span.IsEmpty)
            {
                ArrowExpressionClauseSyntax expressionBody = propertyDeclaration.ExpressionBody;

                if (expressionBody != null)
                {
                    result = AnalyzeTrailingTrivia(expressionBody.ArrowToken);

                    if (result.containsEndOfLine)
                        return;
                }
            }

            if (result.span.IsEmpty)
                result = AnalyzeTrailingTrivia(propertyDeclaration);

            if (!result.span.IsEmpty)
                ReportDiagnostic(context, result.span);
        }

        private static void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            (TextSpan span, bool containsDocumentationComment) = AnalyzeLeadingTrivia(context.Node);

            if (!span.IsEmpty)
            {
                ReportDiagnostic(context, span);
                return;
            }

            if (containsDocumentationComment)
                return;

            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            (TextSpan span, bool containsEndOfLine) result = AnalyzeTrailingTrivia(indexerDeclaration.ParameterList);

            if (result.containsEndOfLine)
                return;

            if (result.span.IsEmpty)
            {
                ArrowExpressionClauseSyntax expressionBody = indexerDeclaration.ExpressionBody;

                if (expressionBody != null)
                {
                    result = AnalyzeTrailingTrivia(expressionBody.ArrowToken);

                    if (result.containsEndOfLine)
                        return;
                }
            }

            if (result.span.IsEmpty)
                result = AnalyzeTrailingTrivia(indexerDeclaration);

            if (!result.span.IsEmpty)
                ReportDiagnostic(context, result.span);
        }

        private static void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            (TextSpan span, bool containsDocumentationComment) = AnalyzeLeadingTrivia(context.Node);

            if (!span.IsEmpty)
            {
                ReportDiagnostic(context, span);
                return;
            }

            if (containsDocumentationComment)
                return;

            var fieldDeclaration = (FieldDeclarationSyntax)context.Node;

            (TextSpan span, bool containsEndOfLine) result = AnalyzeTrailingTrivia(fieldDeclaration);

            if (!result.span.IsEmpty)
                ReportDiagnostic(context, result.span);
        }

        private static void AnalyzeEventFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            (TextSpan span, bool containsDocumentationComment) = AnalyzeLeadingTrivia(context.Node);

            if (!span.IsEmpty)
            {
                ReportDiagnostic(context, span);
                return;
            }

            if (containsDocumentationComment)
                return;

            var eventFieldDeclaration = (EventFieldDeclarationSyntax)context.Node;

            (TextSpan span, bool containsEndOfLine) result = AnalyzeTrailingTrivia(eventFieldDeclaration);

            if (!result.span.IsEmpty)
                ReportDiagnostic(context, result.span);
        }

        private static void AnalyzeEventDeclaration(SyntaxNodeAnalysisContext context)
        {
            (TextSpan span, bool containsDocumentationComment) = AnalyzeLeadingTrivia(context.Node);

            if (!span.IsEmpty)
            {
                ReportDiagnostic(context, span);
                return;
            }

            if (containsDocumentationComment)
                return;

            var eventDeclaration = (EventDeclarationSyntax)context.Node;

            (TextSpan span, bool containsEndOfLine) result = AnalyzeTrailingTrivia(eventDeclaration.Identifier);

            if (result.containsEndOfLine)
                return;

            if (result.span.IsEmpty)
                result = AnalyzeTrailingTrivia(eventDeclaration.AccessorList);

            if (!result.span.IsEmpty)
                ReportDiagnostic(context, result.span);
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, TextSpan span)
        {
            context.ReportDiagnostic(
                DiagnosticDescriptors.ReplaceCommentWithDocumentationComment,
                Location.Create(context.Node.SyntaxTree, span));
        }
    }
}

