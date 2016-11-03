// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ArgumentDiagnosticAnalyzer : BaseDiagnosticAnalyzer
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

            context.RegisterSyntaxNodeAction(AnalyzeArgument, SyntaxKind.Argument);
        }

        private void AnalyzeArgument(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var argument = (ArgumentSyntax)context.Node;

            if (argument.Expression?.IsKind(SyntaxKind.StringLiteralExpression) != true)
                return;

            using (IEnumerator<ParameterSyntax> en = GetParametersInScope(argument).GetEnumerator())
            {
                if (en.MoveNext())
                {
                    var literalExpression = (LiteralExpressionSyntax)argument.Expression;

                    string text = (literalExpression).Token.ValueText;

                    if (ExistsParameterWithName(en, text))
                    {
                        IParameterSymbol parameterSymbol = GetParameterSymbol(argument, context.SemanticModel);

                        if (parameterSymbol != null
                            && (IdentifierEquals(parameterSymbol.Name, "paramName")
                                || IdentifierEquals(parameterSymbol.Name, "parameterName")))
                        {
                            ReportDiagnostic(context, literalExpression, text);
                        }
                    }
                }
                else
                {
                    AccessorDeclarationSyntax setter = argument
                        .FirstAncestor<AccessorDeclarationSyntax>();

                    if (setter?.IsKind(SyntaxKind.SetAccessorDeclaration) == true)
                    {
                        PropertyDeclarationSyntax property = setter
                            .FirstAncestor<PropertyDeclarationSyntax>();

                        if (property != null)
                        {
                            var literalExpression = (LiteralExpressionSyntax)argument.Expression;

                            string text = (literalExpression).Token.ValueText;

                            if (IdentifierEquals(property.Identifier.ValueText, text))
                            {
                                IParameterSymbol parameterSymbol = GetParameterSymbol(argument, context.SemanticModel);

                                if (parameterSymbol != null
                                    && IdentifierEquals(parameterSymbol.Name, "propertyName"))
                                {
                                    ReportDiagnostic(context, literalExpression, text);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static IEnumerable<ParameterSyntax> GetParametersInScope(SyntaxNode node)
        {
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

        private static bool ExistsParameterWithName(IEnumerator<ParameterSyntax> enumerator, string name)
        {
            do
            {
                if (IdentifierEquals(enumerator.Current.Identifier.ValueText, name))
                    return true;

            } while (enumerator.MoveNext());

            return false;
        }

        private static IParameterSymbol GetParameterSymbol(ArgumentSyntax argument, SemanticModel semanticModel)
        {
            var argumentList = argument.Parent as ArgumentListSyntax;

            if (argumentList == null)
                return null;

            var expression = argumentList.Parent as ExpressionSyntax;

            if (expression == null)
                return null;

            ISymbol symbol = semanticModel.GetSymbolInfo(expression).Symbol;

            if (symbol == null)
                return null;

            ImmutableArray<IParameterSymbol> parameters = symbol.GetParameters();

            if (parameters.Length == 0)
                return null;

            if (argument.NameColon != null)
            {
                string name = argument.NameColon.Name?.Identifier.ValueText;

                if (name != null)
                {
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        if (IdentifierEquals(parameters[i].Name, name))
                            return parameters[i];
                    }
                }
            }
            else
            {
                int index = argumentList.Arguments.IndexOf(argument);

                if (index < parameters.Length)
                    return parameters[index];

                if (index >= parameters.Length
                    && parameters[parameters.Length - 1].IsParams)
                {
                    return parameters[parameters.Length - 1];
                }
            }

            return null;
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, LiteralExpressionSyntax literalExpression, string text)
        {
            context.ReportDiagnostic(
                DiagnosticDescriptors.UseNameOfOperator,
                literalExpression.GetLocation(),
                text);

            text = literalExpression.Token.Text;

            if (text.Length >= 2)
            {
                ReportDiagnostic(context, literalExpression, new TextSpan(literalExpression.Span.Start, (text[0] == '@') ? 2 : 1));
                ReportDiagnostic(context, literalExpression, new TextSpan(literalExpression.Span.End - 1, 1));
            }
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, LiteralExpressionSyntax literalExpression, TextSpan span)
        {
            context.ReportDiagnostic(
                DiagnosticDescriptors.UseNameOfOperatorFadeOut,
                Location.Create(literalExpression.SyntaxTree, span));
        }

        private static bool IdentifierEquals(string a, string b)
        {
            return string.Equals(a, b, StringComparison.Ordinal);
        }
    }
}
