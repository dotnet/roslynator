// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class AddNewLineBeforeTypeParameterConstraintAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddNewLineBeforeTypeParameterConstraint); }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeTypeDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeTypeDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeTypeDeclaration, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeDelegateDeclaration, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeLocalFunctionStatement, SyntaxKind.LocalFunctionStatement);
        }

        private static void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (TypeDeclarationSyntax)context.Node;

            SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses = typeDeclaration.ConstraintClauses;

            TypeParameterListSyntax typeParameterList = typeDeclaration.TypeParameterList;

            if (typeParameterList == null)
                return;

            Analyze(context, typeParameterList.GreaterThanToken, constraintClauses);
        }

        private static void AnalyzeDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;

            SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses = delegateDeclaration.ConstraintClauses;

            Analyze(context, delegateDeclaration.ParameterList.CloseParenToken, constraintClauses);
        }

        private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses = methodDeclaration.ConstraintClauses;

            Analyze(context, methodDeclaration.ParameterList.CloseParenToken, constraintClauses);
        }

        private static void AnalyzeLocalFunctionStatement(SyntaxNodeAnalysisContext context)
        {
            var localFunction = (LocalFunctionStatementSyntax)context.Node;

            SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses = localFunction.ConstraintClauses;

            Analyze(context, localFunction.ParameterList.CloseParenToken, constraintClauses);
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            SyntaxToken previousToken,
            SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            if (constraintClauses.Count <= 1)
                return;

            foreach (TypeParameterConstraintClauseSyntax constraintClause in constraintClauses)
            {
                if (!constraintClause.GetLeadingTrivia().Any()
                    && previousToken.TrailingTrivia.SingleOrDefault().IsWhitespaceTrivia())
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.AddNewLineBeforeTypeParameterConstraint,
                        Location.Create(constraintClause.SyntaxTree, new TextSpan(constraintClause.SpanStart, 0)));
                }

                previousToken = constraintClause.GetLastToken();
            }
        }
    }
}
