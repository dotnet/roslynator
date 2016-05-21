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

        private static readonly ImmutableDictionary<string, string> _propertiesCount
            = ImmutableDictionary.CreateRange(new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("PropertyName", "Count") });

        private static readonly ImmutableDictionary<string, string> _propertiesLength
            = ImmutableDictionary.CreateRange(new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("PropertyName", "Length") });

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.SimplifyLinqMethodChain,
                    DiagnosticDescriptors.UseCountOrLengthPropertyInsteadOfAnyMethod);
            }
        }

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

                if (invocation.Parent?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == false
                    && methodName == "Any"
                    && IsEnumerableMethod(context, invocation, "Any"))
                {
                    ITypeSymbol typeSymbol = context
                        .SemanticModel
                        .GetTypeInfo(memberAccess.Expression, context.CancellationToken)
                        .Type;

                    if (typeSymbol != null
                        && !typeSymbol.IsKind(SymbolKind.ErrorType)
                        && !IsIEnumerableOfT(typeSymbol))
                    {
                        string propertyName = GetPropertyName(typeSymbol, context);

                        if (propertyName != null)
                        {
                            string messageArg = null;

                            TextSpan span = TextSpan.FromBounds(memberAccess.Name.Span.Start, invocation.Span.End);

                            if (invocation.DescendantTrivia(span).All(f => f.IsWhitespaceOrEndOfLine()))
                            {
                                if (invocation.Parent?.IsKind(SyntaxKind.LogicalNotExpression) == true)
                                {
                                    var logicalNot = (PrefixUnaryExpressionSyntax)invocation.Parent;

                                    if (logicalNot.OperatorToken.TrailingTrivia.IsWhitespaceOrEndOfLine()
                                        && logicalNot.Operand.GetLeadingTrivia().IsWhitespaceOrEndOfLine())
                                    {
                                        messageArg = $"{propertyName} == 0";
                                    }
                                }
                                else
                                {
                                    messageArg = $"{propertyName} > 0";
                                }
                            }

                            if (messageArg != null)
                            {
                                Diagnostic diagnostic = Diagnostic.Create(
                                    DiagnosticDescriptors.UseCountOrLengthPropertyInsteadOfAnyMethod,
                                    Location.Create(context.Node.SyntaxTree, span),
                                    (propertyName == "Count") ? _propertiesCount : _propertiesLength,
                                    messageArg);

                                context.ReportDiagnostic(diagnostic);
                            }
                        }
                    }
                }
            }
        }

        private static string GetPropertyName(ITypeSymbol typeSymbol, SyntaxNodeAnalysisContext context)
        {
            if (typeSymbol.BaseType?.SpecialType == SpecialType.System_Array)
                return "Length";

            for (int i = 0; i < typeSymbol.AllInterfaces.Length; i++)
            {
                if (typeSymbol.AllInterfaces[i].ConstructedFrom.SpecialType == SpecialType.System_Collections_Generic_ICollection_T)
                {
                    foreach (ISymbol members in typeSymbol.GetMembers("Count"))
                    {
                        if (members.IsKind(SymbolKind.Property)
                            && members.DeclaredAccessibility == Accessibility.Public)
                        {
                            return "Count";
                        }
                    }
                }
            }

            return null;
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
                    && IsIEnumerableOfT(methodSymbol.Parameters[0].Type);
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
                    && IsIEnumerableOfT(methodSymbol.Parameters[0].Type))
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

        private static bool IsIEnumerableOfT(ITypeSymbol typeSymbol)
        {
            return typeSymbol?.Kind == SymbolKind.NamedType
                && ((INamedTypeSymbol)typeSymbol).ConstructedFrom.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T;
        }
    }
}
