// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseNameOfOperatorAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.UseNameOfOperator,
                    DiagnosticDescriptors.UseNameOfOperatorFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeArgument, SyntaxKind.Argument);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, SimpleMemberInvocationExpressionInfo invocationInfo)
        {
            ExpressionSyntax expression = invocationInfo.Expression;

            if (expression.Kind() != SyntaxKind.SimpleMemberAccessExpression)
                return;

            var memberAccessExpression = (MemberAccessExpressionSyntax)expression;

            var fieldSymbol = context.SemanticModel.GetSymbol(memberAccessExpression, context.CancellationToken) as IFieldSymbol;

            if (fieldSymbol == null)
                return;

            INamedTypeSymbol containingType = fieldSymbol.ContainingType;

            if (containingType?.TypeKind != TypeKind.Enum)
                return;

            if (containingType.HasAttribute(context.GetTypeByMetadataName(MetadataNames.System_FlagsAttribute)))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UseNameOfOperator, invocationInfo.InvocationExpression);
        }

        public static void AnalyzeArgument(SyntaxNodeAnalysisContext context)
        {
            var argument = (ArgumentSyntax)context.Node;

            ExpressionSyntax expression = argument.Expression;

            if (expression?.IsKind(SyntaxKind.StringLiteralExpression) != true)
                return;

            ParameterInfo parameterInfo = GetNextParametersInScope(argument);

            if (parameterInfo.Success)
            {
                Analyze(context, argument, expression, parameterInfo);
            }
            else
            {
                Analyze(context, argument, expression);
            }
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            ArgumentSyntax argument,
            ExpressionSyntax expression,
            ParameterInfo parameterInfo)
        {
            var literalExpression = (LiteralExpressionSyntax)expression;

            ParameterSyntax parameter = FindMatchingParameter(parameterInfo, literalExpression.Token.ValueText);

            if (parameter == null)
                return;

            IParameterSymbol parameterSymbol = context.SemanticModel.DetermineParameter(argument, allowParams: true, allowCandidate: false, cancellationToken: context.CancellationToken);

            if (parameterSymbol == null)
                return;

            string parameterName = parameterSymbol.Name;

            if (!string.Equals(parameterName, "paramName", StringComparison.Ordinal)
                && !string.Equals(parameterName, "parameterName", StringComparison.Ordinal))
            {
                return;
            }

            ReportDiagnostic(context, literalExpression, parameter.Identifier.Text);
        }

        private static ParameterSyntax FindMatchingParameter(ParameterInfo parameterInfo, string name)
        {
            do
            {
                if (parameterInfo.Parameter != null)
                {
                    ParameterSyntax parameter = parameterInfo.Parameter;

                    if (string.Equals(parameter.Identifier.ValueText, name, StringComparison.Ordinal))
                        return parameter;
                }
                else if (parameterInfo.ParameterList != null)
                {
                    foreach (ParameterSyntax parameter in parameterInfo.ParameterList.Parameters)
                    {
                        if (string.Equals(parameter.Identifier.ValueText, name, StringComparison.Ordinal))
                            return parameter;
                    }
                }

                parameterInfo = GetNextParametersInScope(parameterInfo.Node);

            } while (parameterInfo.Success);

            return null;
        }

        private static ParameterInfo GetNextParametersInScope(SyntaxNode node)
        {
            for (SyntaxNode current = node.Parent; current != null; current = current.Parent)
            {
                ParameterInfo info = ParameterInfo.Create(current);

                if (info.Success)
                    return info;
            }

            return ParameterInfo.Empty;
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, ArgumentSyntax argument, ExpressionSyntax expression)
        {
            AccessorDeclarationSyntax accessor = argument.FirstAncestor<AccessorDeclarationSyntax>();

            if (accessor?.IsKind(SyntaxKind.SetAccessorDeclaration) != true)
                return;

            PropertyDeclarationSyntax property = accessor.FirstAncestor<PropertyDeclarationSyntax>();

            if (property == null)
                return;

            var literalExpression = (LiteralExpressionSyntax)expression;

            string text = literalExpression.Token.ValueText;

            if (!string.Equals(property.Identifier.ValueText, text, StringComparison.Ordinal))
                return;

            IParameterSymbol parameterSymbol = context.SemanticModel.DetermineParameter(argument, allowParams: true, allowCandidate: false, cancellationToken: context.CancellationToken);

            if (parameterSymbol == null)
                return;

            if (!string.Equals(parameterSymbol.Name, "propertyName", StringComparison.Ordinal))
                return;

            ReportDiagnostic(context, literalExpression, property.Identifier.Text);
        }

        private static void ReportDiagnostic(
            SyntaxNodeAnalysisContext context,
            LiteralExpressionSyntax literalExpression,
            string identifier)
        {
            context.ReportDiagnostic(
                DiagnosticDescriptors.UseNameOfOperator,
                literalExpression.GetLocation(),
                ImmutableDictionary.CreateRange(new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("Identifier", identifier) }));

            string text = literalExpression.Token.Text;

            if (text.Length >= 2)
            {
                SyntaxTree syntaxTree = literalExpression.SyntaxTree;
                TextSpan span = literalExpression.Span;

                context.ReportDiagnostic(
                    DiagnosticDescriptors.UseNameOfOperatorFadeOut,
                    Location.Create(syntaxTree, new TextSpan(span.Start, (text[0] == '@') ? 2 : 1)));

                context.ReportDiagnostic(
                    DiagnosticDescriptors.UseNameOfOperatorFadeOut,
                    Location.Create(syntaxTree, new TextSpan(span.End - 1, 1)));
            }
        }

        private readonly struct ParameterInfo
        {
            public static ParameterInfo Empty { get; } = new ParameterInfo();

            private ParameterInfo(SyntaxNode node, ParameterSyntax parameter)
            {
                Node = node;
                Parameter = parameter;
                ParameterList = null;
            }

            private ParameterInfo(SyntaxNode node, BaseParameterListSyntax parameterList)
            {
                Node = node;
                Parameter = null;
                ParameterList = parameterList;
            }

            public ParameterSyntax Parameter { get; }

            public BaseParameterListSyntax ParameterList { get; }

            public SyntaxNode Node { get; }

            public bool Success
            {
                get { return Node != null; }
            }

            public static ParameterInfo Create(SyntaxNode node)
            {
                switch (node.Kind())
                {
                    case SyntaxKind.MethodDeclaration:
                        return new ParameterInfo(node, ((MethodDeclarationSyntax)node).ParameterList);
                    case SyntaxKind.ConstructorDeclaration:
                        return new ParameterInfo(node, ((ConstructorDeclarationSyntax)node).ParameterList);
                    case SyntaxKind.IndexerDeclaration:
                        return new ParameterInfo(node, ((IndexerDeclarationSyntax)node).ParameterList);
                    case SyntaxKind.SimpleLambdaExpression:
                        return new ParameterInfo(node, ((SimpleLambdaExpressionSyntax)node).Parameter);
                    case SyntaxKind.ParenthesizedLambdaExpression:
                        return new ParameterInfo(node, ((ParenthesizedLambdaExpressionSyntax)node).ParameterList);
                    case SyntaxKind.AnonymousMethodExpression:
                        return new ParameterInfo(node, ((AnonymousMethodExpressionSyntax)node).ParameterList);
                    case SyntaxKind.LocalFunctionStatement:
                        return new ParameterInfo(node, ((LocalFunctionStatementSyntax)node).ParameterList);
                }

                return Empty;
            }
        }
    }
}
