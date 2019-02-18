// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class OptimizeLinqMethodCallAnalysis
    {
        public static void AnalyzeAny(SyntaxNodeAnalysisContext context, in SimpleMemberInvocationExpressionInfo invocationInfo)
        {
            bool isLogicalNot = false;

            InvocationExpressionSyntax invocationExpression = invocationInfo.InvocationExpression;

            ExpressionSyntax expression = invocationExpression.WalkUpParentheses();

            if (expression.IsParentKind(SyntaxKind.LogicalNotExpression))
            {
                isLogicalNot = true;
                expression = (ExpressionSyntax)expression.Parent;

                expression = expression.WalkUpParentheses();
            }

            if (!expression.IsParentKind(SyntaxKind.ConditionalExpression))
                return;

            if (expression.Parent.ContainsDiagnostics)
                return;

            var conditionalExpression = (ConditionalExpressionSyntax)expression.Parent;

            if (conditionalExpression.Condition != expression)
                return;

            ExpressionSyntax firstExpression = (isLogicalNot)
                ? conditionalExpression.WhenFalse?.WalkDownParentheses()
                : conditionalExpression.WhenTrue?.WalkDownParentheses();

            if (!firstExpression.IsKind(SyntaxKind.InvocationExpression))
                return;

            ExpressionSyntax secondExpression = (isLogicalNot)
                ? conditionalExpression.WhenTrue?.WalkDownParentheses()
                : conditionalExpression.WhenFalse?.WalkDownParentheses();

            if (secondExpression == null)
                return;

            SimpleMemberInvocationExpressionInfo invocationInfo2 = SyntaxInfo.SimpleMemberInvocationExpressionInfo((InvocationExpressionSyntax)firstExpression);

            if (invocationInfo2.NameText != "First")
                return;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            ExtensionMethodSymbolInfo extensionMethodSymbolInfo = semanticModel.GetExtensionMethodInfo(invocationExpression, cancellationToken);

            if (extensionMethodSymbolInfo.Symbol == null)
                return;

            if (!extensionMethodSymbolInfo.IsReduced)
                return;

            if (!SymbolUtility.IsLinqExtensionOfIEnumerableOfTWithoutParameters(extensionMethodSymbolInfo.Symbol, "Any"))
                return;

            IMethodSymbol methodSymbol2 = semanticModel.GetExtensionMethodInfo(invocationInfo2.InvocationExpression, cancellationToken).Symbol;

            if (methodSymbol2 == null)
                return;

            if (!SymbolUtility.IsLinqExtensionOfIEnumerableOfTWithoutParameters(methodSymbol2, "First"))
                return;

            if (!SyntaxFactory.AreEquivalent(invocationInfo.Expression, invocationInfo2.Expression, topLevel: false))
                return;

            if (!semanticModel.IsDefaultValue(extensionMethodSymbolInfo.ReducedSymbol.TypeArguments[0], secondExpression, cancellationToken))
                return;

            Report(context, conditionalExpression);
        }

        // items.Where(predicate).Any/Count/First/FirstOrDefault/Last/LastOrDefault/LongCount/Single/SingleOrDefault() >>> items.Any/Count/First/FirstOrDefault/Last/LastOrDefault/LongCount/Single/SingleOrDefault(predicate)
        public static void AnalyzeWhere(SyntaxNodeAnalysisContext context, in SimpleMemberInvocationExpressionInfo invocationInfo)
        {
            SimplifyLinqMethodChain(
                context,
                invocationInfo,
                "Where");
        }

        // items.Select(selector).Min/Max() >>> items.Min/Max(selector)
        public static void AnalyzeSelectAndMinOrMax(
            SyntaxNodeAnalysisContext context,
            in SimpleMemberInvocationExpressionInfo invocationInfo)
        {
            SimplifyLinqMethodChain(
                context,
                invocationInfo,
                "Select");
        }

        private static void SimplifyLinqMethodChain(
            SyntaxNodeAnalysisContext context,
            in SimpleMemberInvocationExpressionInfo invocationInfo,
            string methodName)
        {
            SimpleMemberInvocationExpressionInfo invocationInfo2 = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocationInfo.Expression);

            if (!invocationInfo2.Success)
                return;

            if (invocationInfo2.Arguments.Count != 1)
                return;

            if (invocationInfo2.NameText != methodName)
                return;

            InvocationExpressionSyntax invocation = invocationInfo.InvocationExpression;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            IMethodSymbol methodSymbol = semanticModel.GetExtensionMethodInfo(invocation, cancellationToken).Symbol;

            if (methodSymbol == null)
                return;

            if (!SymbolUtility.IsLinqExtensionOfIEnumerableOfTWithoutParameters(methodSymbol, invocationInfo.NameText))
                return;

            IMethodSymbol methodSymbol2 = semanticModel.GetExtensionMethodInfo(invocationInfo2.InvocationExpression, cancellationToken).Symbol;

            if (methodSymbol2 == null)
                return;

            switch (methodName)
            {
                case "Where":
                    {
                        if (!SymbolUtility.IsLinqWhere(methodSymbol2, allowImmutableArrayExtension: true))
                            return;

                        break;
                    }
                case "Select":
                    {
                        if (!SymbolUtility.IsLinqSelect(methodSymbol2, allowImmutableArrayExtension: true))
                            return;

                        break;
                    }
                default:
                    {
                        Debug.Fail(methodName);
                        return;
                    }
            }

            TextSpan span = TextSpan.FromBounds(invocationInfo2.Name.SpanStart, invocation.Span.End);

            Report(context, invocation, span, checkDirectives: true, property: new KeyValuePair<string, string>("Name", "SimplifyLinqMethodChain"));
        }

        public static void AnalyzeFirstOrDefault(SyntaxNodeAnalysisContext context, in SimpleMemberInvocationExpressionInfo invocationInfo)
        {
            InvocationExpressionSyntax invocation = invocationInfo.InvocationExpression;

            ExtensionMethodSymbolInfo extensionMethodSymbolInfo = context.SemanticModel.GetReducedExtensionMethodInfo(invocation, context.CancellationToken);

            IMethodSymbol methodSymbol = extensionMethodSymbolInfo.Symbol;

            if (methodSymbol == null)
                return;

            if (methodSymbol.DeclaredAccessibility != Accessibility.Public)
                return;

            INamedTypeSymbol containingType = methodSymbol.ContainingType;

            if (containingType == null)
                return;

            bool success = false;

            if (containingType.HasMetadataName(MetadataNames.System_Linq_Enumerable))
            {
                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                int parameterCount = parameters.Length;

                if (parameterCount == 2)
                {
                    if (parameters[0].Type.OriginalDefinition.IsIEnumerableOfT()
                        && SymbolUtility.IsPredicateFunc(parameters[1].Type, methodSymbol.TypeArguments[0]))
                    {
                        ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(invocationInfo.Expression, context.CancellationToken);

                        if (typeSymbol != null)
                        {
                            if (typeSymbol.Kind == SymbolKind.ArrayType)
                            {
                                if (((IArrayTypeSymbol)typeSymbol).Rank == 1
                                    && !invocationInfo.Expression.IsKind(SyntaxKind.MemberBindingExpression)
                                    && context.SemanticModel.Compilation.GetTypeByMetadataName("System.Array").GetMembers("Find").Any())
                                {
                                    Report(context, invocationInfo.Name);
                                    return;
                                }
                            }
                            else if (typeSymbol.OriginalDefinition.HasMetadataName(MetadataNames.System_Collections_Generic_List_T))
                            {
                                Report(context, invocationInfo.Name);
                                return;
                            }
                        }

                        success = true;
                    }
                }
                else if (parameterCount == 1)
                {
                    if (parameters[0].Type.OriginalDefinition.IsIEnumerableOfT())
                    {
                        success = true;
                    }
                }
            }
            else if (containingType.HasMetadataName(MetadataNames.System_Linq_ImmutableArrayExtensions))
            {
                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                int parameterCount = parameters.Length;

                if (parameterCount == 2)
                {
                    success = SymbolUtility.IsImmutableArrayOfT(parameters[0].Type)
                        && SymbolUtility.IsPredicateFunc(parameters[1].Type, methodSymbol.TypeArguments[0]);
                }
                else if (parameterCount == 1)
                {
                    success = SymbolUtility.IsImmutableArrayOfT(parameters[0].Type);
                }
            }

            if (!success)
                return;

            SyntaxNode parent = invocation.WalkUpParentheses().Parent;

            NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(parent, NullCheckStyles.ComparisonToNull | NullCheckStyles.IsNull);

            if (!nullCheck.Success)
                return;

            SyntaxNode node = nullCheck.NullCheckExpression;

            if (node.ContainsDirectives)
                return;

            if (!extensionMethodSymbolInfo
                .ReducedSymbol
                .ReturnType
                .IsReferenceTypeOrNullableType())
            {
                return;
            }

            Report(context, node);
        }

        public static void AnalyzeWhereAndAny(SyntaxNodeAnalysisContext context, in SimpleMemberInvocationExpressionInfo invocationInfo)
        {
            SimpleMemberInvocationExpressionInfo invocationInfo2 = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocationInfo.Expression);

            if (!invocationInfo2.Success)
                return;

            if (invocationInfo2.NameText != "Where")
                return;

            ArgumentSyntax argument2 = invocationInfo2.Arguments.SingleOrDefault(shouldThrow: false);

            if (argument2 == null)
                return;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            ExpressionSyntax invocationExpression = invocationInfo.InvocationExpression;

            IMethodSymbol methodSymbol = semanticModel.GetExtensionMethodInfo(invocationExpression, cancellationToken).Symbol;

            if (methodSymbol == null)
                return;

            if (!SymbolUtility.IsLinqExtensionOfIEnumerableOfTWithPredicate(methodSymbol, "Any"))
                return;

            IMethodSymbol methodSymbol2 = semanticModel.GetExtensionMethodInfo(invocationInfo2.InvocationExpression, cancellationToken).Symbol;

            if (methodSymbol2 == null)
                return;

            if (!SymbolUtility.IsLinqWhere(methodSymbol2, allowImmutableArrayExtension: true))
                return;

            SingleParameterLambdaExpressionInfo lambda = SyntaxInfo.SingleParameterLambdaExpressionInfo(invocationInfo.Arguments[0].Expression);

            if (!lambda.Success)
                return;

            if (!(lambda.Body is ExpressionSyntax))
                return;

            SingleParameterLambdaExpressionInfo lambda2 = SyntaxInfo.SingleParameterLambdaExpressionInfo(argument2.Expression);

            if (!lambda2.Success)
                return;

            if (!(lambda2.Body is ExpressionSyntax))
                return;

            if (!lambda.Parameter.Identifier.ValueText.Equals(lambda2.Parameter.Identifier.ValueText, StringComparison.Ordinal))
                return;

            Report(context, invocationExpression, TextSpan.FromBounds(invocationInfo2.Name.SpanStart, invocationExpression.Span.End));
        }

        public static void AnalyzeWhereAndCast(SyntaxNodeAnalysisContext context, in SimpleMemberInvocationExpressionInfo invocationInfo)
        {
            SimpleMemberInvocationExpressionInfo invocationInfo2 = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocationInfo.Expression);

            if (!invocationInfo2.Success)
                return;

            ArgumentSyntax argument = invocationInfo2.Arguments.SingleOrDefault(shouldThrow: false);

            if (argument == null)
                return;

            if (!string.Equals(invocationInfo2.NameText, "Where", StringComparison.Ordinal))
                return;

            InvocationExpressionSyntax invocationExpression = invocationInfo.InvocationExpression;
            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            IMethodSymbol methodSymbol = semanticModel.GetReducedExtensionMethodInfo(invocationExpression, cancellationToken).Symbol;

            if (methodSymbol == null)
                return;

            if (!SymbolUtility.IsLinqCast(methodSymbol))
                return;

            IMethodSymbol methodSymbol2 = semanticModel.GetReducedExtensionMethodInfo(invocationInfo2.InvocationExpression, cancellationToken).Symbol;

            if (methodSymbol2 == null)
                return;

            if (!SymbolUtility.IsLinqWhere(methodSymbol2))
                return;

            IsExpressionInfo isExpressionInfo = SyntaxInfo.IsExpressionInfo(GetLambdaExpression(argument.Expression));

            if (!isExpressionInfo.Success)
                return;

            TypeSyntax type2 = (invocationInfo.Name as GenericNameSyntax)?.TypeArgumentList?.Arguments.SingleOrDefault(shouldThrow: false);

            if (type2 == null)
                return;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(isExpressionInfo.Type, cancellationToken);

            if (typeSymbol == null)
                return;

            ITypeSymbol typeSymbol2 = semanticModel.GetTypeSymbol(type2, cancellationToken);

            if (!typeSymbol.Equals(typeSymbol2))
                return;

            TextSpan span = TextSpan.FromBounds(invocationInfo2.Name.SpanStart, invocationExpression.Span.End);

            Report(context, invocationExpression, span, checkDirectives: true);

            SyntaxNode GetLambdaExpression(ExpressionSyntax expression)
            {
                CSharpSyntaxNode body = (expression as LambdaExpressionSyntax)?.Body;

                if (body?.Kind() == SyntaxKind.Block)
                {
                    StatementSyntax statement = ((BlockSyntax)body).Statements.SingleOrDefault(shouldThrow: false);

                    if (statement?.Kind() == SyntaxKind.ReturnStatement)
                    {
                        var returnStatement = (ReturnStatementSyntax)statement;

                        return returnStatement.Expression;
                    }
                }

                return body;
            }
        }

        public static void AnalyzeOfType(
            SyntaxNodeAnalysisContext context,
            in SimpleMemberInvocationExpressionInfo invocationInfo)
        {
            TypeSyntax typeArgument = (invocationInfo.Name as GenericNameSyntax)?
                .TypeArgumentList
                .Arguments
                .SingleOrDefault(shouldThrow: false);

            if (typeArgument == null)
                return;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            ExtensionMethodSymbolInfo extensionMethodSymbolInfo = semanticModel.GetReducedExtensionMethodInfo(invocationInfo.InvocationExpression, cancellationToken);

            IMethodSymbol methodSymbol = extensionMethodSymbolInfo.Symbol;

            if (methodSymbol == null)
                return;

            if (!SymbolUtility.IsLinqOfType(methodSymbol))
                return;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(typeArgument, cancellationToken);

            if (typeSymbol?.IsErrorType() != false)
                return;

            ITypeSymbol typeSymbol2 = semanticModel.GetTypeSymbol(invocationInfo.Expression, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            if (!typeSymbol2.Implements((INamedTypeSymbol)extensionMethodSymbolInfo.ReducedSymbol.ReturnType, allInterfaces: true))
                return;

            ReportNameWithArgumentList(context, invocationInfo);
        }

        public static void AnalyzeFirst(
            SyntaxNodeAnalysisContext context,
            in SimpleMemberInvocationExpressionInfo invocationInfo)
        {
            ExtensionMethodSymbolInfo extensionMethodSymbolInfo = context.SemanticModel.GetReducedExtensionMethodInfo(invocationInfo.InvocationExpression, context.CancellationToken);

            IMethodSymbol methodSymbol = extensionMethodSymbolInfo.Symbol;

            if (methodSymbol == null)
                return;

            if (!SymbolUtility.IsLinqExtensionOfIEnumerableOfTWithoutParameters(methodSymbol, "First"))
                return;

            ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(invocationInfo.Expression, context.CancellationToken);

            if (typeSymbol?.IsErrorType() != false)
                return;

            if (!StringUtility.Equals(typeSymbol.MetadataName, "Queue`1", "Stack`1"))
                return;

            if (!typeSymbol.ContainingNamespace.HasMetadataName(MetadataNames.System_Collections_Generic))
                return;

            Report(context, invocationInfo.Name, property: new KeyValuePair<string, string>("MethodName", "Peek"));
        }

        public static void AnalyzeCount(SyntaxNodeAnalysisContext context, in SimpleMemberInvocationExpressionInfo invocationInfo)
        {
            InvocationExpressionSyntax invocationExpression = invocationInfo.InvocationExpression;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            IMethodSymbol methodSymbol = semanticModel.GetReducedExtensionMethodInfo(invocationExpression, cancellationToken).Symbol;

            if (methodSymbol == null)
                return;

            if (!SymbolUtility.IsLinqExtensionOfIEnumerableOfTWithoutParameters(methodSymbol, "Count"))
                return;

            ExpressionSyntax expression = invocationInfo.Expression;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken);

            if (typeSymbol == null)
                return;

            string propertyName = SymbolUtility.GetCountOrLengthPropertyName(typeSymbol, semanticModel, expression.SpanStart);

            if (propertyName != null)
            {
                if (CanBeReplacedWithMemberAccessExpression(invocationExpression)
                    && CheckInfiniteRecursion(typeSymbol, propertyName, invocationExpression.SpanStart, semanticModel, cancellationToken))
                {
                    ReportNameWithArgumentList(context, invocationInfo, property: new KeyValuePair<string, string>("PropertyName", propertyName), messageArgs: propertyName);
                }

                return;
            }

            SyntaxNode parent = invocationExpression.Parent;

            switch (parent.Kind())
            {
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                    {
                        var equalsExpression = (BinaryExpressionSyntax)parent;

                        if (equalsExpression.Left == invocationExpression)
                        {
                            if (equalsExpression.Right.IsNumericLiteralExpression("0"))
                                ReportNameWithArgumentList(context, invocationInfo);
                        }
                        else if (equalsExpression.Left.IsNumericLiteralExpression("0"))
                        {
                            ReportNameWithArgumentList(context, invocationInfo);
                        }

                        break;
                    }
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                    {
                        var binaryExpression = (BinaryExpressionSyntax)parent;

                        if (binaryExpression.Left == invocationExpression)
                        {
                            if (binaryExpression.Right.IsNumericLiteralExpression("0"))
                                ReportNameWithArgumentList(context, invocationInfo);
                        }
                        else if (binaryExpression.Left.IsNumericLiteralExpression("1"))
                        {
                            ReportNameWithArgumentList(context, invocationInfo);
                        }

                        break;
                    }
                case SyntaxKind.GreaterThanOrEqualExpression:
                case SyntaxKind.LessThanExpression:
                    {
                        var binaryExpression = (BinaryExpressionSyntax)parent;

                        if (binaryExpression.Left == invocationExpression)
                        {
                            if (binaryExpression.Right.IsNumericLiteralExpression("1"))
                                ReportNameWithArgumentList(context, invocationInfo);
                        }
                        else if (binaryExpression.Left.IsNumericLiteralExpression("0"))
                        {
                            ReportNameWithArgumentList(context, invocationInfo);
                        }

                        break;
                    }
            }

            bool CanBeReplacedWithMemberAccessExpression(ExpressionSyntax e)
            {
                SyntaxNode p = CSharpUtility.GetTopmostExpressionInCallChain(e).WalkUpParentheses().Parent;

                switch (p.Kind())
                {
                    case SyntaxKind.ExpressionStatement:
                        {
                            return false;
                        }
                    case SyntaxKind.SimpleLambdaExpression:
                    case SyntaxKind.ParenthesizedLambdaExpression:
                        {
                            return semanticModel.GetMethodSymbol((LambdaExpressionSyntax)p, cancellationToken)?.ReturnType.IsVoid() == false;
                        }
                }

                return true;
            }
        }

        private static bool CheckInfiniteRecursion(
            ITypeSymbol typeSymbol,
            string propertyName,
            int position,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ISymbol symbol = semanticModel.GetEnclosingSymbol(position, cancellationToken);

            if (symbol != null)
            {
                IPropertySymbol propertySymbol = null;

                if (symbol.Kind == SymbolKind.Property)
                {
                    propertySymbol = (IPropertySymbol)symbol;
                }
                else if (symbol.Kind == SymbolKind.Method)
                {
                    var methodSymbol = (IMethodSymbol)symbol;

                    if (methodSymbol.MethodKind.Is(MethodKind.PropertyGet, MethodKind.PropertySet))
                        propertySymbol = methodSymbol.AssociatedSymbol as IPropertySymbol;
                }

                if (propertySymbol?.IsIndexer == false
                    && propertySymbol.Name == propertyName
                    && propertySymbol.ContainingType == typeSymbol)
                {
                    return false;
                }
            }

            return true;
        }

        private static void ReportNameWithArgumentList(
            SyntaxNodeAnalysisContext context,
            in SimpleMemberInvocationExpressionInfo invocationInfo,
            bool checkDirectives = false,
            KeyValuePair<string, string>? property = null,
            params string[] messageArgs)
        {
            Report(
                context: context,
                node: invocationInfo.InvocationExpression,
                span: TextSpan.FromBounds(invocationInfo.Name.SpanStart, invocationInfo.ArgumentList.Span.End),
                checkDirectives: checkDirectives,
                property: property,
                messageArgs: messageArgs);
        }

        private static void Report(
            SyntaxNodeAnalysisContext context,
            SyntaxNode node,
            bool checkDirectives = false,
            KeyValuePair<string, string>? property = null,
            params string[] messageArgs)
        {
            Report(
                context: context,
                node: node,
                span: node.Span,
                checkDirectives: checkDirectives,
                property: property,
                messageArgs: messageArgs);
        }

        private static void Report(
            SyntaxNodeAnalysisContext context,
            SyntaxNode node,
            TextSpan span,
            bool checkDirectives = false,
            KeyValuePair<string, string>? property = null,
            params string[] messageArgs)
        {
            if (checkDirectives
                && node.ContainsDirectives(span))
            {
                return;
            }

            ImmutableDictionary<string, string> properties = (property != null)
                ? ImmutableDictionary.CreateRange(new KeyValuePair<string, string>[] { property.Value })
                : ImmutableDictionary<string, string>.Empty;

            DiagnosticHelpers.ReportDiagnostic(context,
                descriptor: DiagnosticDescriptors.OptimizeLinqMethodCall,
                location: Location.Create(node.SyntaxTree, span),
                properties: properties,
                messageArgs: messageArgs);
        }
    }
}
