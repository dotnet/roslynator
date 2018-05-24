// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
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

        public static void Analyze(SyntaxNodeAnalysisContext context, in SimpleMemberInvocationExpressionInfo invocationInfo)
        {
            InvocationExpressionSyntax invocationExpression = invocationInfo.InvocationExpression;

            ExpressionSyntax expression = invocationExpression.WalkUpParentheses();

            if (!IsExpressionOfAccessExpression(expression))
                return;

            IMethodSymbol methodSymbol = context.SemanticModel.GetMethodSymbol(invocationExpression, context.CancellationToken);

            if (methodSymbol?.ReturnType.IsReferenceType != true)
                return;

            INamedTypeSymbol containingType = methodSymbol.ContainingType;

            if (containingType == null)
                return;

            if (methodSymbol.IsExtensionMethod)
            {
                if (containingType.HasFullyQualifiedMetadataName(FullyQualifiedMetadataNames.System_Linq_Enumerable))
                    ReportDiagnostic(context, expression);
            }
            else if (!methodSymbol.IsStatic)
            {
                Debug.Assert(containingType.Implements(SpecialType.System_Collections_Generic_IEnumerable_T, allInterfaces: true), "Type does not implement IEnumerable<T>");

                if (containingType.Implements(SpecialType.System_Collections_Generic_IEnumerable_T, allInterfaces: true))
                    ReportDiagnostic(context, expression);
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
