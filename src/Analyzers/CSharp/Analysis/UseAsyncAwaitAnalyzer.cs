// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;
using Roslynator.CSharp.SyntaxWalkers;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class UseAsyncAwaitAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                    Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.UseAsyncAwait);

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeMethodDeclaration(f), SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeLocalFunctionStatement(f), SyntaxKind.LocalFunctionStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeSimpleLambdaExpression(f), SyntaxKind.SimpleLambdaExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeParenthesizedLambdaExpression(f), SyntaxKind.ParenthesizedLambdaExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeAnonymousMethodExpression(f), SyntaxKind.AnonymousMethodExpression);
        }

        private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            if (methodDeclaration.Modifiers.Contains(SyntaxKind.AsyncKeyword))
                return;

            BlockSyntax body = methodDeclaration.Body;

            if (body == null)
                return;

            if (!body.Statements.Any())
                return;

            if (!methodDeclaration.Identifier.ValueText.EndsWith("Async", StringComparison.Ordinal))
            {
                IMethodSymbol methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

                if (!IsTaskLike(methodSymbol.ReturnType))
                    return;
            }

            if (IsFixable(body, context))
                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.UseAsyncAwait, methodDeclaration.Identifier);
        }

        private static void AnalyzeLocalFunctionStatement(SyntaxNodeAnalysisContext context)
        {
            var localFunction = (LocalFunctionStatementSyntax)context.Node;

            if (localFunction.Modifiers.Contains(SyntaxKind.AsyncKeyword))
                return;

            BlockSyntax body = localFunction.Body;

            if (body == null)
                return;

            if (!body.Statements.Any())
                return;

            if (!localFunction.Identifier.ValueText.EndsWith("Async", StringComparison.Ordinal))
            {
                IMethodSymbol methodSymbol = context.SemanticModel.GetDeclaredSymbol(localFunction, context.CancellationToken);

                if (!IsTaskLike(methodSymbol.ReturnType))
                    return;
            }

            if (IsFixable(body, context))
                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.UseAsyncAwait, localFunction.Identifier);
        }

        private static void AnalyzeSimpleLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var simpleLambda = (SimpleLambdaExpressionSyntax)context.Node;

            if (simpleLambda.AsyncKeyword.IsKind(SyntaxKind.AsyncKeyword))
                return;

            if (simpleLambda.Body is not BlockSyntax body)
                return;

            if (context.SemanticModel.GetSymbol(simpleLambda, context.CancellationToken) is not IMethodSymbol methodSymbol)
                return;

            if (!IsTaskLike(methodSymbol.ReturnType))
                return;

            if (IsFixable(body, context))
                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.UseAsyncAwait, simpleLambda);
        }

        private static void AnalyzeParenthesizedLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var parenthesizedLambda = (ParenthesizedLambdaExpressionSyntax)context.Node;

            if (parenthesizedLambda.AsyncKeyword.IsKind(SyntaxKind.AsyncKeyword))
                return;

            if (parenthesizedLambda.Body is not BlockSyntax body)
                return;

            if (context.SemanticModel.GetSymbol(parenthesizedLambda, context.CancellationToken) is not IMethodSymbol methodSymbol)
                return;

            if (!IsTaskLike(methodSymbol.ReturnType))
                return;

            if (IsFixable(body, context))
                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.UseAsyncAwait, parenthesizedLambda);
        }

        private static void AnalyzeAnonymousMethodExpression(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;

            if (anonymousMethod.AsyncKeyword.IsKind(SyntaxKind.AsyncKeyword))
                return;

            BlockSyntax body = anonymousMethod.Block;

            if (body == null)
                return;

            if (context.SemanticModel.GetSymbol(anonymousMethod, context.CancellationToken) is not IMethodSymbol methodSymbol)
                return;

            if (!IsTaskLike(methodSymbol.ReturnType))
                return;

            if (IsFixable(body, context))
                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.UseAsyncAwait, anonymousMethod);
        }

        private static bool IsFixable(BlockSyntax body, SyntaxNodeAnalysisContext context)
        {
            UseAsyncAwaitWalker walker = null;

            try
            {
                walker = UseAsyncAwaitWalker.GetInstance(context.SemanticModel, context.CancellationToken);

                walker.VisitBlock(body);

                return walker.ReturnStatement != null;
            }
            finally
            {
                if (walker != null)
                    UseAsyncAwaitWalker.Free(walker);
            }
        }

        private static bool IsTaskLike(ITypeSymbol typeSymbol)
        {
            if (typeSymbol?.IsErrorType() == false
                && typeSymbol.SpecialType == SpecialType.None)
            {
                ITypeSymbol t = typeSymbol.OriginalDefinition;

                if (t.Name == "ValueTask`1"
                    && t.ContainingNamespace.HasMetadataName(MetadataNames.System_Threading_Tasks))
                {
                    return true;
                }

                do
                {
                    if ((t.Name == "Task" || t.Name == "Task`1")
                        && t.ContainingNamespace.HasMetadataName(MetadataNames.System_Threading_Tasks))
                    {
                        return true;
                    }

                    t = t.BaseType;

                } while (t?.SpecialType == SpecialType.None);
            }

            return false;
        }

        private class UseAsyncAwaitWalker : StatementWalker
        {
            [ThreadStatic]
            private static UseAsyncAwaitWalker _cachedInstance;

            private int _usingOrTryStatementDepth;
            private bool _shouldVisit = true;
            private readonly List<int> _usingDeclarations = new();

            public override bool ShouldVisit => _shouldVisit;

            public ReturnStatementSyntax ReturnStatement { get; private set; }

            public SemanticModel SemanticModel { get; private set; }

            public CancellationToken CancellationToken { get; private set; }

            public override void VisitUsingStatement(UsingStatementSyntax node)
            {
                _usingOrTryStatementDepth++;
                base.VisitUsingStatement(node);
                _usingOrTryStatementDepth--;
            }

            public override void VisitTryStatement(TryStatementSyntax node)
            {
                BlockSyntax block = node.Block;

                if (block != null)
                {
                    _usingOrTryStatementDepth++;
                    VisitBlock(block);
                    _usingOrTryStatementDepth--;
                }

                foreach (CatchClauseSyntax catchClause in node.Catches)
                    VisitCatchClause(catchClause);

                FinallyClauseSyntax finallyClause = node.Finally;

                if (finallyClause != null)
                    VisitFinallyClause(finallyClause);
            }

            public override void VisitBlock(BlockSyntax node)
            {
                _usingDeclarations.Add(0);
                base.VisitBlock(node);
                _usingDeclarations.RemoveAt(_usingDeclarations.Count - 1);
            }

            public override void VisitSwitchSection(SwitchSectionSyntax node)
            {
                _usingDeclarations.Add(0);
                base.VisitSwitchSection(node);
                _usingDeclarations.RemoveAt(_usingDeclarations.Count - 1);
            }

            public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
            {
                if (node.UsingKeyword.IsKind(SyntaxKind.UsingKeyword))
                    _usingDeclarations[_usingDeclarations.Count - 1]++;

                base.VisitLocalDeclarationStatement(node);
            }

            public override void VisitReturnStatement(ReturnStatementSyntax node)
            {
                bool isInsideUsingOrTry = _usingOrTryStatementDepth > 0;

                if (!isInsideUsingOrTry)
                {
                    foreach (int count in _usingDeclarations)
                    {
                        if (count > 0)
                        {
                            isInsideUsingOrTry = true;
                            break;
                        }
                    }
                }

                if (isInsideUsingOrTry)
                {
                    ExpressionSyntax expression = node.Expression;

                    if (expression?.IsKind(SyntaxKind.AwaitExpression) == false
                        && !IsCompletedTask(expression))
                    {
                        ReturnStatement = node;
                        _shouldVisit = false;
                    }
                }

                base.VisitReturnStatement(node);
            }

            private bool IsCompletedTask(ExpressionSyntax expression)
            {
                if (expression.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                {
                    var simpleMemberAccess = (MemberAccessExpressionSyntax)expression;

                    return string.Equals(simpleMemberAccess.Name.Identifier.ValueText, "CompletedTask", StringComparison.Ordinal)
                        && SemanticModel.GetSymbol(expression, CancellationToken) is IPropertySymbol propertySymbol
                        && IsTaskOrTaskOrT(propertySymbol.ContainingType);
                }
                else
                {
                    SimpleMemberInvocationExpressionInfo memberInvocation = SyntaxInfo.SimpleMemberInvocationExpressionInfo(expression);

                    if (memberInvocation.Success)
                    {
                        switch (memberInvocation.NameText)
                        {
                            case "FromCanceled":
                            case "FromException":
                            case "FromResult":
                                {
                                    if (SemanticModel.GetSymbol(expression, CancellationToken) is IMethodSymbol methodSymbol
                                        && (methodSymbol.Arity == 0 || methodSymbol.Arity == 1)
                                        && methodSymbol.Parameters.Length == 1
                                        && IsTaskOrTaskOrT(methodSymbol.ContainingType))
                                    {
                                        return true;
                                    }

                                    break;
                                }
                        }
                    }
                }

                return false;
            }

            private static bool IsTaskOrTaskOrT(INamedTypeSymbol typeSymbol)
            {
                return typeSymbol.HasMetadataName(MetadataNames.System_Threading_Tasks_Task)
                    || typeSymbol.HasMetadataName(MetadataNames.System_Threading_Tasks_Task_T);
            }

            public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
            {
            }

            public override void VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
            {
            }

            public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
            {
            }

            public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
            {
            }

            public static UseAsyncAwaitWalker GetInstance(SemanticModel semanticModel, CancellationToken cancellationToken)
            {
                UseAsyncAwaitWalker walker = _cachedInstance;

                if (walker != null)
                {
                    _cachedInstance = null;
                }
                else
                {
                    walker = new UseAsyncAwaitWalker();
                }

                walker.SemanticModel = semanticModel;
                walker.CancellationToken = cancellationToken;

                return walker;
            }

            public static void Free(UseAsyncAwaitWalker walker)
            {
                walker._shouldVisit = true;
                walker._usingDeclarations.Clear();
                walker.ReturnStatement = null;
                walker.SemanticModel = null;
                walker.CancellationToken = default;

                _cachedInstance = walker;
            }
        }
    }
}
