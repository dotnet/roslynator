// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DefaultExpressionAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.SimplifyDefaultExpression); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterCompilationStartAction(startContext =>
            {
                if (((CSharpCompilation)startContext.Compilation).LanguageVersion >= LanguageVersion.CSharp7_1)
                    startContext.RegisterSyntaxNodeAction(AnalyzeDefaultExpression, SyntaxKind.DefaultExpression);
            });
        }

        public static void AnalyzeDefaultExpression(SyntaxNodeAnalysisContext context)
        {
            var defaultExpression = (DefaultExpressionSyntax)context.Node;

            ExpressionSyntax expression = defaultExpression.WalkUpParentheses();

            SyntaxNode parent = expression.Parent;

            switch (parent.Kind())
            {
                case SyntaxKind.EqualsValueClause:
                    {
                        parent = parent.Parent;

                        switch (parent.Kind())
                        {
                            case SyntaxKind.Parameter:
                                {
                                    ReportDiagnostic();
                                    return;
                                }
                            case SyntaxKind.VariableDeclarator:
                                {
                                    return;
                                }
                            default:
                                {
                                    Debug.WriteLine($"{parent.Kind()} {parent}");
                                    return;
                                }
                        }
                    }
                case SyntaxKind.ConditionalExpression:
                    {
                        var conditionalExpression = (ConditionalExpressionSyntax)parent;

                        ExpressionSyntax expression2 = (conditionalExpression.WhenTrue == expression)
                            ? conditionalExpression.WhenFalse
                            : conditionalExpression.WhenTrue;

                        if (expression2.IsKind(SyntaxKind.ThrowExpression))
                            return;

                        TypeInfo typeInfo = context.SemanticModel.GetTypeInfo(expression, context.CancellationToken);

                        ITypeSymbol type = typeInfo.Type;
                        ITypeSymbol convertedType = typeInfo.ConvertedType;

                        if (type != convertedType)
                            return;

                        ITypeSymbol type2 = context.SemanticModel.GetTypeSymbol(expression2, context.CancellationToken);

                        if (type != type2)
                            return;

                        ReportDiagnostic();
                        return;
                    }
                case SyntaxKind.ArrowExpressionClause:
                case SyntaxKind.CoalesceExpression:
                case SyntaxKind.ReturnStatement:
                case SyntaxKind.YieldReturnStatement:
                case SyntaxKind.SimpleAssignmentExpression:
                    {
                        if (parent.IsParentKind(SyntaxKind.ObjectInitializerExpression))
                            return;

                        TypeInfo typeInfo = context.SemanticModel.GetTypeInfo(expression, context.CancellationToken);

                        if (typeInfo.Type != typeInfo.ConvertedType)
                            return;

                        ReportDiagnostic();
                        return;
                    }
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                    {
                        ReportDiagnostic();
                        return;
                    }
                case SyntaxKind.Argument:
                case SyntaxKind.ConstantPattern:
                case SyntaxKind.CaseSwitchLabel:
                    {
                        return;
                    }
                default:
                    {
                        Debug.WriteLine($"{parent.Kind()} {parent}");
                        return;
                    }
            }

            void ReportDiagnostic()
            {
                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.SimplifyDefaultExpression, Location.Create(defaultExpression.SyntaxTree, defaultExpression.ParenthesesSpan()));
            }
        }
    }
}
