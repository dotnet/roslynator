// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseNameOfOperatorRefactoring
    {
        public static void AnalyzeArgument(SyntaxNodeAnalysisContext context)
        {
            var argument = (ArgumentSyntax)context.Node;

            ExpressionSyntax expression = argument.Expression;

            if (expression?.IsKind(SyntaxKind.StringLiteralExpression) == true)
            {
                using (IEnumerator<ParameterSyntax> en = GetParametersInScope(argument).GetEnumerator())
                {
                    if (en.MoveNext())
                    {
                        var literalExpression = (LiteralExpressionSyntax)expression;

                        string text = literalExpression.Token.ValueText;

                        if (ExistsParameterWithName(en, text))
                        {
                            IParameterSymbol parameterSymbol = context.SemanticModel.DetermineParameter(argument, allowParams: true, allowCandidate: false, cancellationToken: context.CancellationToken);

                            if (parameterSymbol != null
                                && (NameEquals(parameterSymbol.Name, "paramName")
                                    || NameEquals(parameterSymbol.Name, "parameterName")))
                            {
                                ReportDiagnostic(context, literalExpression, text);
                            }
                        }
                    }
                    else
                    {
                        AccessorDeclarationSyntax accessor = argument.FirstAncestor<AccessorDeclarationSyntax>();

                        if (accessor?.IsKind(SyntaxKind.SetAccessorDeclaration) == true)
                        {
                            PropertyDeclarationSyntax property = accessor.FirstAncestor<PropertyDeclarationSyntax>();

                            if (property != null)
                            {
                                var literalExpression = (LiteralExpressionSyntax)expression;

                                string text = literalExpression.Token.ValueText;

                                if (NameEquals(property.Identifier.ValueText, text))
                                {
                                    IParameterSymbol parameterSymbol = context.SemanticModel.DetermineParameter(argument, allowParams: true, allowCandidate: false, cancellationToken: context.CancellationToken);

                                    if (parameterSymbol != null
                                        && NameEquals(parameterSymbol.Name, "propertyName"))
                                    {
                                        ReportDiagnostic(context, literalExpression, text);
                                    }
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
                    BaseParameterListSyntax parameterList = GetParameterList(ancestor);

                    if (parameterList != null)
                    {
                        foreach (ParameterSyntax parameter in parameterList.Parameters)
                            yield return parameter;
                    }
                }
            }
        }

        private static BaseParameterListSyntax GetParameterList(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)node).ParameterList;
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)node).ParameterList;
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)node).ParameterList;
                case SyntaxKind.ParenthesizedLambdaExpression:
                    return ((ParenthesizedLambdaExpressionSyntax)node).ParameterList;
                case SyntaxKind.AnonymousMethodExpression:
                    return ((AnonymousMethodExpressionSyntax)node).ParameterList;
                case SyntaxKind.LocalFunctionStatement:
                    return ((LocalFunctionStatementSyntax)node).ParameterList;
                default:
                    return null;
            }
        }

        private static bool ExistsParameterWithName(IEnumerator<ParameterSyntax> enumerator, string name)
        {
            do
            {
                if (NameEquals(enumerator.Current.Identifier.ValueText, name))
                    return true;

            } while (enumerator.MoveNext());

            return false;
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, LiteralExpressionSyntax literalExpression, string text)
        {
            context.ReportDiagnostic(
                DiagnosticDescriptors.UseNameOfOperator,
                literalExpression,
                text);

            text = literalExpression.Token.Text;

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

        private static bool NameEquals(string a, string b)
        {
            return string.Equals(a, b, StringComparison.Ordinal);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax newNode = CSharpFactory.NameOfExpression(literalExpression.Token.ValueText)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(literalExpression, newNode, cancellationToken);
        }
    }
}
