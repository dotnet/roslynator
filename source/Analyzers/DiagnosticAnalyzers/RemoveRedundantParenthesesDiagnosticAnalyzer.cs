// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.CSharp.Refactorings.RemoveRedundantParenthesesRefactoring;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RemoveRedundantParenthesesDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.RemoveRedundantParentheses,
                    DiagnosticDescriptors.RemoveRedundantParenthesesFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(f => Analyze(f, (ParenthesizedExpressionSyntax)f.Node), SyntaxKind.ParenthesizedExpression);

            context.RegisterSyntaxNodeAction(f => Analyze(f, (WhileStatementSyntax)f.Node), SyntaxKind.WhileStatement);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (DoStatementSyntax)f.Node), SyntaxKind.DoStatement);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (UsingStatementSyntax)f.Node), SyntaxKind.UsingStatement);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (LockStatementSyntax)f.Node), SyntaxKind.LockStatement);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (IfStatementSyntax)f.Node), SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (SwitchStatementSyntax)f.Node), SyntaxKind.SwitchStatement);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (ForEachStatementSyntax)f.Node), SyntaxKind.ForEachStatement);

            context.RegisterSyntaxNodeAction(f => Analyze(f, (ReturnStatementSyntax)f.Node), SyntaxKind.ReturnStatement);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (YieldStatementSyntax)f.Node), SyntaxKind.YieldReturnStatement);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (ExpressionStatementSyntax)f.Node), SyntaxKind.ExpressionStatement);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (ArgumentSyntax)f.Node), SyntaxKind.Argument);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (AttributeArgumentSyntax)f.Node), SyntaxKind.AttributeArgument);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (EqualsValueClauseSyntax)f.Node), SyntaxKind.EqualsValueClause);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (AwaitExpressionSyntax)f.Node), SyntaxKind.AwaitExpression);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (ArrowExpressionClauseSyntax)f.Node), SyntaxKind.ArrowExpressionClause);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (InterpolationSyntax)f.Node), SyntaxKind.Interpolation);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (InitializerExpressionSyntax)f.Node), SyntaxKind.ArrayInitializerExpression);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (InitializerExpressionSyntax)f.Node), SyntaxKind.CollectionInitializerExpression);

            context.RegisterSyntaxNodeAction(f => Analyze(f, (AssignmentExpressionSyntax)f.Node), SyntaxKind.SimpleAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (AssignmentExpressionSyntax)f.Node), SyntaxKind.AddAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (AssignmentExpressionSyntax)f.Node), SyntaxKind.SubtractAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (AssignmentExpressionSyntax)f.Node), SyntaxKind.MultiplyAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (AssignmentExpressionSyntax)f.Node), SyntaxKind.DivideAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (AssignmentExpressionSyntax)f.Node), SyntaxKind.ModuloAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (AssignmentExpressionSyntax)f.Node), SyntaxKind.AndAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (AssignmentExpressionSyntax)f.Node), SyntaxKind.ExclusiveOrAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (AssignmentExpressionSyntax)f.Node), SyntaxKind.OrAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (AssignmentExpressionSyntax)f.Node), SyntaxKind.LeftShiftAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (AssignmentExpressionSyntax)f.Node), SyntaxKind.RightShiftAssignmentExpression);
        }
    }
}
