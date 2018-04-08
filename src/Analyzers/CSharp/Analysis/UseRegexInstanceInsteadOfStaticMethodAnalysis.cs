// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class UseRegexInstanceInsteadOfStaticMethodAnalysis
    {
        internal static void Analyze(SyntaxNodeAnalysisContext context, SimpleMemberInvocationExpressionInfo invocationInfo)
        {
            if (!ValidateMethodNameAndArgumentCount())
                return;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(invocationInfo.InvocationExpression, cancellationToken);

            if (!SymbolUtility.IsPublicStaticNonGeneric(methodSymbol))
                return;

            if (methodSymbol.ContainingType?.Equals(context.GetTypeByMetadataName(MetadataNames.System_Text_RegularExpressions_Regex)) != true)
                return;

            SeparatedSyntaxList<ArgumentSyntax> arguments = invocationInfo.Arguments;

            if (!ValidateArgument(arguments[1]))
                return;

            if (methodSymbol.Name == "Replace")
            {
                if (arguments.Count == 4
                    && !ValidateArgument(arguments[3]))
                {
                    return;
                }
            }
            else if (arguments.Count == 3
                && !ValidateArgument(arguments[2]))
            {
                return;
            }

            context.ReportDiagnostic(DiagnosticDescriptors.UseRegexInstanceInsteadOfStaticMethod, invocationInfo.Name);

            bool ValidateMethodNameAndArgumentCount()
            {
                switch (invocationInfo.NameText)
                {
                    case "IsMatch":
                    case "Match":
                    case "Matches":
                    case "Split":
                        {
                            int count = invocationInfo.Arguments.Count;

                            return count >= 2
                                && count <= 3;
                        }
                    case "Replace":
                        {
                            int count = invocationInfo.Arguments.Count;

                            return count >= 3
                                && count <= 4;
                        }
                }

                return false;
            }

            bool ValidateArgument(ArgumentSyntax argument)
            {
                ExpressionSyntax expression = argument.Expression;

                if (expression == null)
                    return false;

                if (expression.WalkDownParentheses() is LiteralExpressionSyntax)
                    return true;

                if (!semanticModel.HasConstantValue(expression, cancellationToken))
                    return false;

                ISymbol symbol = semanticModel.GetSymbol(expression, cancellationToken);

                Debug.Assert(symbol != null);

                if (symbol == null)
                    return true;

                switch (symbol.Kind)
                {
                    case SymbolKind.Field:
                        {
                            return ((IFieldSymbol)symbol).HasConstantValue;
                        }
                    case SymbolKind.Method:
                        {
                            if (((IMethodSymbol)symbol).MethodKind != MethodKind.BuiltinOperator)
                                return false;

                            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken);

                            if (typeSymbol == null)
                                return false;

                            return typeSymbol.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_Text_RegularExpressions_RegexOptions));
                        }
                    default:
                        {
                            return false;
                        }
                }
            }
        }

        private readonly struct ArgumentListPair
        {
            public ArgumentListPair(ArgumentListSyntax argumentList1, ArgumentListSyntax argumentList2)
            {
                ArgumentList1 = argumentList1;
                ArgumentList2 = argumentList2;
            }

            public ArgumentListSyntax ArgumentList1 { get; }
            public ArgumentListSyntax ArgumentList2 { get; }
        }
    }
}
