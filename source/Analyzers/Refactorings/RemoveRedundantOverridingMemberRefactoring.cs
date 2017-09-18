// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantOverridingMemberRefactoring
    {
        public static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            if (!methodDeclaration.ContainsDirectives
                && !methodDeclaration.ContainsDiagnostics)
            {
                SyntaxTokenList modifiers = methodDeclaration.Modifiers;

                if (modifiers.Contains(SyntaxKind.OverrideKeyword)
                    && !modifiers.ContainsAny(SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword)
                    && !methodDeclaration.AttributeLists.Any()
                    && !methodDeclaration.HasDocumentationComment()
                    && methodDeclaration.DescendantTrivia(methodDeclaration.Span).All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                {
                    ExpressionSyntax expression = GetMethodExpression(methodDeclaration);

                    MemberInvocationExpression memberInvocation;
                    if (MemberInvocationExpression.TryCreate(expression, out memberInvocation)
                        && memberInvocation.Expression.IsKind(SyntaxKind.BaseExpression))
                    {
                        IMethodSymbol methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

                        IMethodSymbol overriddenMethod = methodSymbol?.OverriddenMethod;

                        if (overriddenMethod != null)
                        {
                            ISymbol symbol = context.SemanticModel.GetSymbol(memberInvocation.Name, context.CancellationToken);

                            if (overriddenMethod.Equals(symbol)
                                && CheckParameters(methodDeclaration.ParameterList, memberInvocation.ArgumentList, context.SemanticModel, context.CancellationToken)
                                && CheckDefaultValues(methodSymbol.Parameters, overriddenMethod.Parameters))
                            {
                                context.ReportDiagnostic(
                                    DiagnosticDescriptors.RemoveRedundantOverridingMember,
                                    methodDeclaration,
                                    methodDeclaration.GetTitle());
                            }
                        }
                    }
                }
            }
        }

        private static bool CheckParameters(
            BaseParameterListSyntax parameterList,
            BaseArgumentListSyntax argumentList,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;
            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

            if (parameters.Count != arguments.Count)
                return false;

            for (int i = 0; i < parameters.Count; i++)
            {
                if (semanticModel
                    .GetDeclaredSymbol(parameters[i], cancellationToken)?
                    .Equals(GetParameterSymbol(arguments[i].Expression, semanticModel, cancellationToken)) != true)
                {
                    return false;
                }
            }

            return true;
        }

        private static IParameterSymbol GetParameterSymbol(ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (expression != null)
            {
                ISymbol symbol = semanticModel.GetSymbol(expression, cancellationToken);

                if (symbol?.IsParameter() == true)
                {
                    var parameterSymbol = (IParameterSymbol)symbol;

                    ISymbol containingSymbol = parameterSymbol.ContainingSymbol;

                    if (containingSymbol?.IsMethod() == true)
                    {
                        var methodSymbol = (IMethodSymbol)containingSymbol;

                        ISymbol associatedSymbol = methodSymbol.AssociatedSymbol;

                        if (associatedSymbol?.IsKind(SymbolKind.Property) == true)
                        {
                            var propertySymbol = (IPropertySymbol)associatedSymbol;

                            if (propertySymbol.IsIndexer)
                            {
                                ImmutableArray<IParameterSymbol> parameters = propertySymbol.Parameters;

                                if (parameters.Length > parameterSymbol.Ordinal)
                                    return propertySymbol.Parameters[parameterSymbol.Ordinal];
                            }
                        }
                    }

                    return parameterSymbol;
                }
            }

            return null;
        }

        private static bool CheckDefaultValues(ImmutableArray<IParameterSymbol> parameters, ImmutableArray<IParameterSymbol> baseParameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].HasExplicitDefaultValue)
                {
                    if (baseParameters[i].HasExplicitDefaultValue)
                    {
                        if (!Equals(parameters[i].ExplicitDefaultValue, baseParameters[i].ExplicitDefaultValue))
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (baseParameters[i].HasExplicitDefaultValue)
                {
                    return false;
                }
            }

            return true;
        }

        private static ExpressionSyntax GetMethodExpression(MethodDeclarationSyntax methodDeclaration)
        {
            BlockSyntax body = methodDeclaration.Body;

            if (body != null)
            {
                StatementSyntax statement = body.SingleStatementOrDefault();

                if (statement != null)
                {
                    if (methodDeclaration.ReturnsVoid())
                    {
                        if (statement.IsKind(SyntaxKind.ExpressionStatement))
                            return ((ExpressionStatementSyntax)statement).Expression;
                    }
                    else if (statement.IsKind(SyntaxKind.ReturnStatement))
                    {
                        return ((ReturnStatementSyntax)statement).Expression;
                    }
                }
            }
            else
            {
                return methodDeclaration.ExpressionBody?.Expression;
            }

            return null;
        }

        public static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

            if (!propertyDeclaration.ContainsDirectives
                && !propertyDeclaration.ContainsDiagnostics)
            {
                SyntaxTokenList modifiers = propertyDeclaration.Modifiers;

                if (modifiers.Contains(SyntaxKind.OverrideKeyword)
                    && !modifiers.Contains(SyntaxKind.SealedKeyword)
                    && !propertyDeclaration.AttributeLists.Any()
                    && !propertyDeclaration.HasDocumentationComment()
                    && propertyDeclaration.DescendantTrivia(propertyDeclaration.Span).All(f => f.IsWhitespaceOrEndOfLineTrivia())
                    && propertyDeclaration
                        .AccessorList?
                        .Accessors
                        .All(accessor => CanRefactor(propertyDeclaration, accessor, context.SemanticModel, context.CancellationToken)) == true)
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.RemoveRedundantOverridingMember,
                        propertyDeclaration,
                        propertyDeclaration.GetTitle());
                }
            }
        }

        internal static bool CanRefactor(
            PropertyDeclarationSyntax propertyDeclaration,
            AccessorDeclarationSyntax accessor,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            switch (accessor.Kind())
            {
                case SyntaxKind.GetAccessorDeclaration:
                    {
                        ExpressionSyntax expression = GetGetAccessorExpression(accessor);

                        if (expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                        {
                            var memberAccess = (MemberAccessExpressionSyntax)expression;

                            if (memberAccess.Expression?.IsKind(SyntaxKind.BaseExpression) == true)
                            {
                                SimpleNameSyntax simpleName = memberAccess.Name;

                                IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, cancellationToken);

                                if (propertySymbol != null)
                                {
                                    IPropertySymbol overriddenProperty = propertySymbol.OverriddenProperty;

                                    if (overriddenProperty != null)
                                    {
                                        ISymbol symbol = semanticModel.GetSymbol(simpleName, cancellationToken);

                                        if (overriddenProperty.Equals(symbol))
                                            return true;
                                    }
                                }
                            }
                        }

                        return false;
                    }
                case SyntaxKind.SetAccessorDeclaration:
                    {
                        ExpressionSyntax expression = GetSetAccessorExpression(accessor);

                        if (expression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
                        {
                            var assignment = (AssignmentExpressionSyntax)expression;

                            ExpressionSyntax left = assignment.Left;

                            if (left?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                            {
                                var memberAccess = (MemberAccessExpressionSyntax)left;

                                if (memberAccess.Expression?.IsKind(SyntaxKind.BaseExpression) == true)
                                {
                                    ExpressionSyntax right = assignment.Right;

                                    if (right?.IsKind(SyntaxKind.IdentifierName) == true)
                                    {
                                        var identifierName = (IdentifierNameSyntax)right;

                                        if (identifierName.Identifier.ValueText == "value")
                                        {
                                            SimpleNameSyntax simpleName = memberAccess.Name;

                                            if (simpleName != null)
                                            {
                                                IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, cancellationToken);

                                                if (propertySymbol != null)
                                                {
                                                    IPropertySymbol overriddenProperty = propertySymbol.OverriddenProperty;

                                                    if (overriddenProperty != null)
                                                    {
                                                        ISymbol symbol = semanticModel.GetSymbol(simpleName, cancellationToken);

                                                        if (overriddenProperty.Equals(symbol))
                                                            return true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        return false;
                    }
                case SyntaxKind.UnknownAccessorDeclaration:
                    {
                        return false;
                    }
                default:
                    {
                        Debug.Fail(accessor.Kind().ToString());
                        return false;
                    }
            }
        }

        public static void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            if (!indexerDeclaration.ContainsDirectives
                && !indexerDeclaration.ContainsDiagnostics)
            {
                SyntaxTokenList modifiers = indexerDeclaration.Modifiers;

                if (modifiers.Contains(SyntaxKind.OverrideKeyword)
                    && !modifiers.Contains(SyntaxKind.SealedKeyword)
                    && !indexerDeclaration.AttributeLists.Any()
                    && !indexerDeclaration.HasDocumentationComment()
                    && indexerDeclaration.DescendantTrivia(indexerDeclaration.Span).All(f => f.IsWhitespaceOrEndOfLineTrivia())
                    && indexerDeclaration
                        .AccessorList?
                        .Accessors
                        .All(accessor => CanRefactor(indexerDeclaration, accessor, context.SemanticModel, context.CancellationToken)) == true)
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.RemoveRedundantOverridingMember,
                        indexerDeclaration,
                        indexerDeclaration.GetTitle());
                }
            }
        }

        internal static bool CanRefactor(
            IndexerDeclarationSyntax indexerDeclaration,
            AccessorDeclarationSyntax accessor,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            switch (accessor.Kind())
            {
                case SyntaxKind.GetAccessorDeclaration:
                    {
                        ExpressionSyntax expression = GetGetAccessorExpression(accessor);

                        if (expression?.IsKind(SyntaxKind.ElementAccessExpression) == true)
                        {
                            var elementAccess = (ElementAccessExpressionSyntax)expression;

                            if (elementAccess.Expression?.IsKind(SyntaxKind.BaseExpression) == true
                                && elementAccess.ArgumentList != null)
                            {
                                IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(indexerDeclaration, cancellationToken);

                                if (propertySymbol != null)
                                {
                                    IPropertySymbol overriddenProperty = propertySymbol.OverriddenProperty;

                                    if (overriddenProperty != null)
                                    {
                                        ISymbol symbol = semanticModel.GetSymbol(elementAccess, cancellationToken);

                                        if (overriddenProperty.Equals(symbol)
                                            && CheckParameters(indexerDeclaration.ParameterList, elementAccess.ArgumentList, semanticModel, cancellationToken)
                                            && CheckDefaultValues(propertySymbol.Parameters, overriddenProperty.Parameters))
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }

                        return false;
                    }
                case SyntaxKind.SetAccessorDeclaration:
                    {
                        ExpressionSyntax expression = GetSetAccessorExpression(accessor);

                        if (expression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
                        {
                            var assignment = (AssignmentExpressionSyntax)expression;

                            ExpressionSyntax left = assignment.Left;

                            if (left?.IsKind(SyntaxKind.ElementAccessExpression) == true)
                            {
                                var elementAccess = (ElementAccessExpressionSyntax)left;

                                if (elementAccess.Expression?.IsKind(SyntaxKind.BaseExpression) == true
                                    && elementAccess.ArgumentList != null)
                                {
                                    ExpressionSyntax right = assignment.Right;

                                    if (right?.IsKind(SyntaxKind.IdentifierName) == true)
                                    {
                                        var identifierName = (IdentifierNameSyntax)right;

                                        if (identifierName.Identifier.ValueText == "value")
                                        {
                                            IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(indexerDeclaration, cancellationToken);

                                            if (propertySymbol != null)
                                            {
                                                IPropertySymbol overriddenProperty = propertySymbol.OverriddenProperty;

                                                if (overriddenProperty != null)
                                                {
                                                    ISymbol symbol = semanticModel.GetSymbol(elementAccess, cancellationToken);

                                                    if (overriddenProperty.Equals(symbol)
                                                        && CheckParameters(indexerDeclaration.ParameterList, elementAccess.ArgumentList, semanticModel, cancellationToken)
                                                        && CheckDefaultValues(propertySymbol.Parameters, overriddenProperty.Parameters))
                                                    {
                                                        return true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        return false;
                    }
                case SyntaxKind.UnknownAccessorDeclaration:
                    {
                        return false;
                    }
                default:
                    {
                        Debug.Fail(accessor.Kind().ToString());
                        return false;
                    }
            }
        }

        private static ExpressionSyntax GetGetAccessorExpression(AccessorDeclarationSyntax accessor)
        {
            BlockSyntax body = accessor.Body;

            if (body != null)
            {
                StatementSyntax statement = body.SingleStatementOrDefault();

                if (statement?.IsKind(SyntaxKind.ReturnStatement) == true)
                    return ((ReturnStatementSyntax)statement).Expression;
            }
            else
            {
                return accessor.ExpressionBody?.Expression;
            }

            return null;
        }

        private static ExpressionSyntax GetSetAccessorExpression(AccessorDeclarationSyntax accessor)
        {
            BlockSyntax body = accessor.Body;

            if (body != null)
            {
                StatementSyntax statement = body.SingleStatementOrDefault();

                if (statement?.IsKind(SyntaxKind.ExpressionStatement) == true)
                    return ((ExpressionStatementSyntax)statement).Expression;
            }
            else
            {
                return accessor.ExpressionBody?.Expression;
            }

            return null;
        }
    }
}
