// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AvoidNullReferenceExceptionAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AvoidNullReferenceException); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeAsExpression, SyntaxKind.AsExpression);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, SimpleMemberInvocationExpressionInfo invocationInfo)
        {
            switch (invocationInfo.NameText)
            {
                case "ElementAtOrDefault":
                case "FirstOrDefault":
                case "LastOrDefault":
                    {
                        InvocationExpressionSyntax invocationExpression = invocationInfo.InvocationExpression;

                        ExpressionSyntax expression = invocationExpression.WalkUpParentheses();

                        if (!IsExpressionOfAccessExpression(expression))
                            break;

                        IMethodSymbol methodSymbol = context.SemanticModel.GetMethodSymbol(invocationExpression, context.CancellationToken);

                        if (methodSymbol?.ReturnType.IsReferenceType != true)
                            break;

                        if (methodSymbol.ContainingType?.Equals(context.SemanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_Enumerable)) != true)
                            break;

                        ReportDiagnostic(context, expression);
                        break;
                    }
            }
        }

        public static void AnalyzeAsExpression(SyntaxNodeAnalysisContext context)
        {
            var asExpression = (BinaryExpressionSyntax)context.Node;

            ExpressionSyntax expression = asExpression.WalkUpParentheses();

            if (asExpression == expression)
                return;

            if (!IsExpressionOfAccessExpression(expression))
                return;

            if (context.SemanticModel
                .GetTypeSymbol(asExpression, context.CancellationToken)?
                .IsReferenceType != true)
            {
                return;
            }

            ReportDiagnostic(context, expression);
        }

        private static bool IsExpressionOfAccessExpression(ExpressionSyntax expression)
        {
            SyntaxNode parent = expression.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.SimpleMemberAccessExpression:
                    return expression == ((MemberAccessExpressionSyntax)parent).Expression;
                case SyntaxKind.ElementAccessExpression:
                    return expression == ((ElementAccessExpressionSyntax)parent).Expression;
            }

            return false;
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
        {
            context.ReportDiagnostic(
                DiagnosticDescriptors.AvoidNullReferenceException,
                Location.Create(expression.SyntaxTree, new TextSpan(expression.Span.End, 1)));
        }
    }
}
