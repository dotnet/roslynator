// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Internal.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InvocationExpressionDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.ReplaceIsKindMethodInvocation);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeInvocation(f), SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            if (invocation.Expression != null
                && invocation.ArgumentList?.Arguments.Count == 1)
            {
                SimpleNameSyntax methodName = GetMethodName(invocation.Expression);

                if (string.Equals(methodName.Identifier.ValueText, "IsKind", StringComparison.Ordinal))
                {
                    ArgumentSyntax argument = invocation.ArgumentList.Arguments[0];

                    if (argument?.Expression != null)
                    {
                        SemanticModel semanticModel = context.SemanticModel;

                        INamedTypeSymbol syntaxKindSymbol = semanticModel.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.CSharp.SyntaxKind");

                        if (syntaxKindSymbol != null)
                        {
                            ISymbol argumentSymbol = semanticModel.GetSymbolInfo(argument.Expression, context.CancellationToken).Symbol;

                            if (argumentSymbol.IsField()
                                && argumentSymbol.ContainingType?.Equals(syntaxKindSymbol) == true
                                && CanRefactor(methodName, argumentSymbol.Name, syntaxKindSymbol, semanticModel, context.CancellationToken))
                            {
                                Location location = Location.Create(
                                    invocation.SyntaxTree,
                                    TextSpan.FromBounds(methodName.Span.Start, invocation.Span.End));

                                context.ReportDiagnostic(
                                    DiagnosticDescriptors.ReplaceIsKindMethodInvocation,
                                    location,
                                    CreateProperties(argumentSymbol.Name));
                            }
                        }
                    }
                }
            }
        }

        private static bool CanRefactor(
            SimpleNameSyntax methodName,
            string elementName,
            INamedTypeSymbol syntaxKindSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            INamedTypeSymbol syntaxNodeSymbol = semanticModel.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.SyntaxNode");

            if (CanRefactor(
                methodName,
                elementName,
                syntaxKindSymbol,
                syntaxNodeSymbol,
                semanticModel,
                cancellationToken))
            {
                return true;
            }

            INamedTypeSymbol syntaxTriviaSymbol = semanticModel.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.SyntaxTrivia");

            return CanRefactor(
                methodName,
                elementName,
                syntaxKindSymbol,
                syntaxTriviaSymbol,
                semanticModel,
                cancellationToken);
        }

        private static bool CanRefactor(
            SimpleNameSyntax methodName,
            string elementName,
            INamedTypeSymbol syntaxKindSymbol,
            INamedTypeSymbol typeSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (typeSymbol != null)
            {
                INamedTypeSymbol newExtensionsClassSymbol = semanticModel.Compilation.GetTypeByMetadataName($"Roslynator.{typeSymbol.Name}Extensions");

                if (newExtensionsClassSymbol != null
                    && NewExtensionMethodExists(elementName, typeSymbol, newExtensionsClassSymbol))
                {
                    var methodSymbol = semanticModel.GetSymbolInfo(methodName, cancellationToken).Symbol as IMethodSymbol;

                    if (methodSymbol != null
                        && methodSymbol.MethodKind == MethodKind.ReducedExtension
                        && methodSymbol.ReducedFrom.Parameters.Length == 2
                        && methodSymbol.ReducedFrom.Parameters[0].Type.Equals(typeSymbol)
                        && methodSymbol.ReducedFrom.Parameters[1].Type.Equals(syntaxKindSymbol))
                    {
                        INamedTypeSymbol extensionsClassSymbol = semanticModel.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.CSharpExtensions");

                        if (extensionsClassSymbol != null
                            && methodSymbol.ContainingType?.Equals(extensionsClassSymbol) == true)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static bool NewExtensionMethodExists(
            string elementName,
            INamedTypeSymbol typeSymbol,
            INamedTypeSymbol newExtensionsClassSymbol)
        {
            if (newExtensionsClassSymbol != null)
            {
                foreach (ISymbol member in newExtensionsClassSymbol.GetMembers("Is" + elementName))
                {
                    if (member.IsMethod())
                    {
                        var methodSymbol = (IMethodSymbol)member;

                        if (methodSymbol.IsPublic()
                            && methodSymbol.IsStatic
                            && methodSymbol.ReturnType.IsBoolean()
                            && methodSymbol.Parameters.Length == 1
                            && methodSymbol.Parameters[0].Type.Equals(typeSymbol))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static ImmutableDictionary<string, string> CreateProperties(string name)
        {
            return ImmutableDictionary.CreateRange(
                new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("MethodName", "Is" + name) });
        }

        internal static SimpleNameSyntax GetMethodName(ExpressionSyntax expression)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.SimpleMemberAccessExpression:
                    return ((MemberAccessExpressionSyntax)expression).Name;
                case SyntaxKind.MemberBindingExpression:
                    return ((MemberBindingExpressionSyntax)expression).Name;
                default:
                    return null;
            }
        }
    }
}
