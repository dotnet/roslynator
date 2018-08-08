// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.SyntaxWalkers;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseAsyncAwaitAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.UseAsyncAwait); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeLocalFunctionStatement, SyntaxKind.LocalFunctionStatement);
            context.RegisterSyntaxNodeAction(AnalyzeSimpleLambdaExpression, SyntaxKind.SimpleLambdaExpression);
            context.RegisterSyntaxNodeAction(AnalyzeParenthesizedLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression);
            context.RegisterSyntaxNodeAction(AnalyzeAnonymousMethodExpression, SyntaxKind.AnonymousMethodExpression);
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

            if (IsFixable(body))
                context.ReportDiagnostic(DiagnosticDescriptors.UseAsyncAwait, methodDeclaration.Identifier);
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

            if (IsFixable(body))
                context.ReportDiagnostic(DiagnosticDescriptors.UseAsyncAwait, localFunction.Identifier);
        }

        private static void AnalyzeSimpleLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var simpleLambda = (SimpleLambdaExpressionSyntax)context.Node;

            if (simpleLambda.AsyncKeyword.IsKind(SyntaxKind.AsyncKeyword))
                return;

            if (!(simpleLambda.Body is BlockSyntax body))
                return;

            if (!(context.SemanticModel.GetSymbol(simpleLambda, context.CancellationToken) is IMethodSymbol methodSymbol))
                return;

            if (!IsTaskLike(methodSymbol.ReturnType))
                return;

            if (IsFixable(body))
                context.ReportDiagnostic(DiagnosticDescriptors.UseAsyncAwait, simpleLambda);
        }

        private static void AnalyzeParenthesizedLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var parenthesizedLambda = (ParenthesizedLambdaExpressionSyntax)context.Node;

            if (parenthesizedLambda.AsyncKeyword.IsKind(SyntaxKind.AsyncKeyword))
                return;

            if (!(parenthesizedLambda.Body is BlockSyntax body))
                return;

            if (!(context.SemanticModel.GetSymbol(parenthesizedLambda, context.CancellationToken) is IMethodSymbol methodSymbol))
                return;

            if (!IsTaskLike(methodSymbol.ReturnType))
                return;

            if (IsFixable(body))
                context.ReportDiagnostic(DiagnosticDescriptors.UseAsyncAwait, parenthesizedLambda);
        }

        private static void AnalyzeAnonymousMethodExpression(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;

            if (anonymousMethod.AsyncKeyword.IsKind(SyntaxKind.AsyncKeyword))
                return;

            BlockSyntax body = anonymousMethod.Block;

            if (body == null)
                return;

            if (!(context.SemanticModel.GetSymbol(anonymousMethod, context.CancellationToken) is IMethodSymbol methodSymbol))
                return;

            if (!IsTaskLike(methodSymbol.ReturnType))
                return;

            if (IsFixable(body))
                context.ReportDiagnostic(DiagnosticDescriptors.UseAsyncAwait, anonymousMethod);
        }

        private static bool IsFixable(BlockSyntax body)
        {
            UseAsyncAwaitWalker walker = UseAsyncAwaitWalker.GetInstance();

            walker.VisitBlock(body);

            ReturnStatementSyntax returnStatement = walker.ReturnStatement;

            UseAsyncAwaitWalker.Free(walker);

            return returnStatement != null;
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
                }
                while (t?.SpecialType == SpecialType.None);
            }

            return false;
        }

        private class UseAsyncAwaitWalker : StatementWalker
        {
            [ThreadStatic]
            private static UseAsyncAwaitWalker _cachedInstance;

            private int _usingStatementDepth;
            private bool _shouldVisit = true;

            public override bool ShouldVisit => _shouldVisit;

            public ReturnStatementSyntax ReturnStatement { get; private set; }

            public void Reset()
            {
                _shouldVisit = true;
                ReturnStatement = null;
            }

            public override void VisitUsingStatement(UsingStatementSyntax node)
            {
                _usingStatementDepth++;
                base.VisitUsingStatement(node);
                _usingStatementDepth--;
            }

            public override void VisitReturnStatement(ReturnStatementSyntax node)
            {
                if (_usingStatementDepth > 0)
                {
                    ExpressionSyntax expression = node.Expression;

                    if (expression?.IsKind(SyntaxKind.AwaitExpression) == false)
                    {
                        ReturnStatement = node;
                    }

                    _shouldVisit = false;
                }

                base.VisitReturnStatement(node);
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

            public static UseAsyncAwaitWalker GetInstance()
            {
                UseAsyncAwaitWalker walker = _cachedInstance;

                if (walker != null)
                {
                    _cachedInstance = null;
                    return walker;
                }

                return new UseAsyncAwaitWalker();
            }

            public static void Free(UseAsyncAwaitWalker walker)
            {
                walker.Reset();
                _cachedInstance = walker;
            }
        }
    }
}
