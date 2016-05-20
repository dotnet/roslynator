// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InvocationExpressionDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        private static readonly HashSet<string> _methodNames = new HashSet<string>(new string[]
        {
            "Any",
            "Count",
            "First",
            "FirstOrDefault",
            "Last",
            "LastOrDefault",
            "LongCount",
            "Single",
            "SingleOrDefault"
        },
        StringComparer.Ordinal);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.SimplifyLinqMethodChain);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeInvocationExpression(f), SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var invocation = (InvocationExpressionSyntax)context.Node;

            if (invocation.ArgumentList?.Arguments.Count == 0
                && invocation.Expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
            {
                var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

                string methodName = memberAccess.Name?.Identifier.ValueText;

                if (_methodNames.Contains(methodName)
                    && memberAccess.Expression?.IsKind(SyntaxKind.InvocationExpression) == true)
                {
                    var invocation2 = (InvocationExpressionSyntax)memberAccess.Expression;

                    if (invocation2.ArgumentList?.Arguments.Count == 1
                        && invocation2.Expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                    {
                        var memberAccess2 = (MemberAccessExpressionSyntax)invocation2.Expression;

                        if (memberAccess2.Name?.Identifier.ValueText == "Where"
                            && IsEnumerableMethod(context, invocation, methodName)
                            && IsEnumerableWhere(context, invocation2))
                        {
                            TextSpan span = TextSpan.FromBounds(invocation2.Span.End, invocation.Span.End);

                            if (invocation
                                .DescendantTrivia(span)
                                .All(f => f.IsWhitespaceOrEndOfLine()))
                            {
                                context.ReportDiagnostic(
                                    DiagnosticDescriptors.SimplifyLinqMethodChain,
                                    memberAccess2.Name.GetLocation());
                            }
                        }
                    }
                }
            }
        }

        private static bool IsEnumerableMethod(
            SyntaxNodeAnalysisContext context,
            InvocationExpressionSyntax invocation,
            string methodName)
        {
            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;

            if (methodSymbol?.ReducedFrom != null)
            {
                methodSymbol = methodSymbol.ReducedFrom;

                return methodSymbol.MetadataName == methodName
                    && methodSymbol.ContainingType?.Equals(
                        context.SemanticModel.Compilation.GetTypeByMetadataName("System.Linq.Enumerable")) == true
                    && methodSymbol.Parameters.Length == 1
                    && IsIEnumerableOfT(methodSymbol.Parameters[0]);
            }

            return false;
        }

        private static bool IsEnumerableWhere(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation)
        {
            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;

            if (methodSymbol?.ReducedFrom != null)
            {
                methodSymbol = methodSymbol.ReducedFrom;

                if (methodSymbol.MetadataName == "Where"
                    && methodSymbol.ContainingType?.Equals(
                        context.SemanticModel.Compilation.GetTypeByMetadataName("System.Linq.Enumerable")) == true
                    && methodSymbol.Parameters.Length == 2
                    && IsIEnumerableOfT(methodSymbol.Parameters[0]))
                {
                    IParameterSymbol parameterSymbol = methodSymbol.Parameters[1];

                    if (parameterSymbol.Type?.IsKind(SymbolKind.NamedType) == true)
                    {
                        var parameterTypeSymbol = (INamedTypeSymbol)parameterSymbol.Type;

                        return parameterTypeSymbol.ConstructedFrom != null
                            && parameterTypeSymbol.ConstructedFrom.Equals(
                                context.SemanticModel.Compilation.GetTypeByMetadataName("System.Func`2"));
                    }
                }
            }

            return false;
        }

        private static bool IsIEnumerableOfT(IParameterSymbol parameterSymbol)
        {
            if (parameterSymbol.Type?.IsKind(SymbolKind.NamedType) == true)
            {
                var parameterTypeSymbol = (INamedTypeSymbol)parameterSymbol.Type;

                return parameterTypeSymbol.ConstructedFrom?.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T;
            }

            return false;
        }
    }
}
