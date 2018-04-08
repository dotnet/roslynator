// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CallDebugFailInsteadOfDebugAssertAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.CallDebugFailInsteadOfDebugAssert); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterCompilationStartAction(startContext =>
            {
                INamedTypeSymbol debugSymbol = startContext.Compilation.GetTypeByMetadataName(MetadataNames.System_Diagnostics_Debug);

                if (debugSymbol == null)
                    return;

                startContext.RegisterSyntaxNodeAction(nodeContext => AnalyzeInvocationExpression(nodeContext, debugSymbol), SyntaxKind.InvocationExpression);
            });
        }

        public static void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context, INamedTypeSymbol debugSymbol)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            ExpressionSyntax expression = invocation.Expression;

            if (expression == null)
                return;

            if (invocation.SpanContainsDirectives())
                return;

            ArgumentListSyntax argumentList = invocation.ArgumentList;

            if (argumentList == null)
                return;

            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

            if (arguments.Count < 1 || arguments.Count > 3)
                return;

            if (arguments[0].Expression?.Kind() != SyntaxKind.FalseLiteralExpression)
                return;

            IMethodSymbol methodSymbol = context.SemanticModel.GetMethodSymbol(invocation, context.CancellationToken);

            if (!SymbolUtility.IsPublicStaticNonGeneric(methodSymbol, "Assert"))
                return;

            if (methodSymbol.ContainingType?.Equals(debugSymbol) != true)
                return;

            if (!methodSymbol.ReturnsVoid)
                return;

            ImmutableArray<IParameterSymbol> assertParameters = methodSymbol.Parameters;

            int length = assertParameters.Length;

            if (assertParameters[0].Type.SpecialType != SpecialType.System_Boolean)
                return;

            for (int i = 1; i < length; i++)
            {
                if (assertParameters[i].Type.SpecialType != SpecialType.System_String)
                    return;
            }

            if (!ContainsFailMethod())
                return;

            if (expression.Kind() != SyntaxKind.SimpleMemberAccessExpression
                && context.SemanticModel.GetSpeculativeMethodSymbol(invocation.SpanStart, GetNewInvocation(invocation))?
                    .ContainingType?
                    .Equals(debugSymbol) != true)
            {
                return;
            }

            if (expression.Kind() == SyntaxKind.SimpleMemberAccessExpression)
                expression = ((MemberAccessExpressionSyntax)expression).Name;

            Debug.Assert(expression.Kind() == SyntaxKind.IdentifierName, expression.Kind().ToString());

            context.ReportDiagnostic(DiagnosticDescriptors.CallDebugFailInsteadOfDebugAssert, expression);

            bool ContainsFailMethod()
            {
                foreach (ISymbol symbol in methodSymbol.ContainingType.GetMembers("Fail"))
                {
                    if (symbol is IMethodSymbol failMethodSymbol
                        && SymbolUtility.IsPublicStaticNonGeneric(failMethodSymbol)
                        && failMethodSymbol.ReturnsVoid)
                    {
                        ImmutableArray<IParameterSymbol> failParameters = failMethodSymbol.Parameters;

                        if (failParameters.Length == ((length == 1) ? 1 : length - 1)
                            && failParameters.All(f => f.Type.SpecialType == SpecialType.System_String))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        private static InvocationExpressionSyntax GetNewInvocation(InvocationExpressionSyntax invocation)
        {
            ArgumentListSyntax argumentList = invocation.ArgumentList;
            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

            if (arguments.Count == 1)
            {
                ArgumentSyntax argument = arguments[0];
                arguments = arguments.ReplaceAt(0, argument.WithExpression(StringLiteralExpression("").WithTriviaFrom(argument.Expression)));
            }
            else
            {
                arguments = arguments.RemoveAt(0);
            }

            return RefactoringUtility.ChangeInvokedMethodName(invocation, "Fail")
                .WithArgumentList(argumentList.WithArguments(arguments));
        }
    }
}
