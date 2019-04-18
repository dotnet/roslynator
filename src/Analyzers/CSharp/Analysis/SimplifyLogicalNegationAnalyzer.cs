// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SimplifyLogicalNegationAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.SimplifyLogicalNegation); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeLogicalNotExpression, SyntaxKind.LogicalNotExpression);
        }

        public static void AnalyzeLogicalNotExpression(SyntaxNodeAnalysisContext context)
        {
            var logicalNot = (PrefixUnaryExpressionSyntax)context.Node;

            ExpressionSyntax expression = logicalNot.Operand?.WalkDownParentheses();

            if (expression?.IsMissing != false)
                return;

            switch (expression.Kind())
            {
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                case SyntaxKind.LogicalNotExpression:
                    {
                        ReportDiagnostic();
                        break;
                    }
                case SyntaxKind.EqualsExpression:
                    {
                        MemberDeclarationSyntax memberDeclaration = logicalNot.FirstAncestor<MemberDeclarationSyntax>();

                        if (memberDeclaration is OperatorDeclarationSyntax operatorDeclaration
                            && operatorDeclaration.OperatorToken.IsKind(SyntaxKind.ExclamationEqualsToken))
                        {
                            return;
                        }

                        ReportDiagnostic();
                        break;
                    }
                case SyntaxKind.NotEqualsExpression:
                    {
                        MemberDeclarationSyntax memberDeclaration = logicalNot.FirstAncestor<MemberDeclarationSyntax>();

                        if (memberDeclaration is OperatorDeclarationSyntax operatorDeclaration
                            && operatorDeclaration.OperatorToken.IsKind(SyntaxKind.EqualsEqualsToken))
                        {
                            return;
                        }

                        ReportDiagnostic();
                        break;
                    }
            }

            void ReportDiagnostic()
            {
                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.SimplifyLogicalNegation, logicalNot);
            }
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, in SimpleMemberInvocationExpressionInfo invocationInfo)
        {
            SyntaxNode parent = invocationInfo.InvocationExpression.WalkUpParentheses().Parent;

            if (parent.Kind() != SyntaxKind.LogicalNotExpression)
                return;

            SingleParameterLambdaExpressionInfo lambdaInfo = SyntaxInfo.SingleParameterLambdaExpressionInfo(invocationInfo.Arguments.First().Expression.WalkDownParentheses());

            if (!lambdaInfo.Success)
                return;

            ExpressionSyntax expression = GetReturnExpression(lambdaInfo.Body)?.WalkDownParentheses();

            if (expression?.IsKind(SyntaxKind.LogicalNotExpression) != true)
                return;

            IMethodSymbol methodSymbol = context.SemanticModel.GetReducedExtensionMethodInfo(invocationInfo.InvocationExpression, context.CancellationToken).Symbol;

            if (methodSymbol == null)
                return;

            if (!SymbolUtility.IsLinqExtensionOfIEnumerableOfTWithPredicate(methodSymbol))
                return;

            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.SimplifyLogicalNegation, parent);
        }

        internal static ExpressionSyntax GetReturnExpression(CSharpSyntaxNode node)
        {
            if (node is BlockSyntax block)
            {
                StatementSyntax statement = block.Statements.SingleOrDefault(shouldThrow: false);

                return (statement as ReturnStatementSyntax)?.Expression;
            }

            return node as ExpressionSyntax;
        }
    }
}
