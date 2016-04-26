// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Pihrtsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ArgumentDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.UseNameOfOperator);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(AnalyzeArgument, SyntaxKind.Argument);
        }

        private void AnalyzeArgument(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var argument = (ArgumentSyntax)context.Node;

            using (IEnumerator<ParameterSyntax> en = GetParametersInScope(argument).GetEnumerator())
            {
                if (!en.MoveNext())
                    return;

                if (argument.Expression?.IsKind(SyntaxKind.StringLiteralExpression) != true)
                    return;

                var literalExpression = (LiteralExpressionSyntax)argument.Expression;

                string parameterName = (literalExpression).Token.ValueText;

                if (!IsAnyParameter(en, parameterName))
                    return;

                ArgumentInfo argumentInfo = GetArgumentInfo(argument, context.SemanticModel);

                if (argumentInfo.Parameter == null)
                    return;

                if (IsAllowedParameterName(argumentInfo.Parameter.Name))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.UseNameOfOperator,
                        literalExpression.GetLocation(),
                        parameterName);
                }
            }
        }

        private static IEnumerable<ParameterSyntax> GetParametersInScope(SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            foreach (SyntaxNode ancestor in node.AncestorsAndSelf())
            {
                if (ancestor.IsKind(SyntaxKind.SimpleLambdaExpression))
                {
                    yield return ((SimpleLambdaExpressionSyntax)ancestor).Parameter;
                }
                else
                {
                    BaseParameterListSyntax parameterList = ancestor.GetParameterList();
                    if (parameterList != null)
                    {
                        for (int i = 0; i < parameterList.Parameters.Count; i++)
                            yield return parameterList.Parameters[i];
                    }
                }
            }
        }

        private static bool IsAnyParameter(IEnumerator<ParameterSyntax> enumerator, string parameterName)
        {
            do
            {
                if (string.Equals(parameterName, enumerator.Current.Identifier.ValueText, StringComparison.Ordinal))
                    return true;

            } while (enumerator.MoveNext());

            return false;
        }

        private static ArgumentInfo GetArgumentInfo(ArgumentSyntax argument, SemanticModel semanticModel)
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (argument == null)
                throw new ArgumentNullException(nameof(argument));

            var argumentList = argument.Parent as ArgumentListSyntax;
            if (argumentList == null)
                return default(ArgumentInfo);

            var expression = argumentList.Parent as ExpressionSyntax;
            if (expression == null)
                return default(ArgumentInfo);

            ISymbol methodOrProperty = semanticModel.GetSymbolInfo(expression).Symbol;
            if (methodOrProperty == null)
                return default(ArgumentInfo);

            ImmutableArray<IParameterSymbol> parameters = methodOrProperty.GetParameters();
            if (parameters.Length == 0)
                return default(ArgumentInfo);

            if (argument.NameColon != null)
            {
                if (argument.NameColon.Name == null)
                    return default(ArgumentInfo);

                string nameText = argument.NameColon.Name.Identifier.ValueText;

                if (nameText == null)
                    return default(ArgumentInfo);

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (string.Equals(parameters[i].Name, nameText, StringComparison.Ordinal))
                        return new ArgumentInfo(methodOrProperty, parameters[i]);
                }
            }
            else
            {
                int index = argumentList.Arguments.IndexOf(argument);

                if (index < 0)
                    return default(ArgumentInfo);

                if (index < parameters.Length)
                    return new ArgumentInfo(methodOrProperty, parameters[index]);

                if (index >= parameters.Length
                    && parameters[parameters.Length - 1].IsParams)
                {
                    return new ArgumentInfo(methodOrProperty, parameters[parameters.Length - 1]);
                }
            }

            return default(ArgumentInfo);
        }

        private static bool IsAllowedParameterName(string value)
        {
            switch (value)
            {
                case "paramName":
                    return true;
                case "parameterName":
                    return true;
                default:
                    return false;
            }
        }
    }
}
