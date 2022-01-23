// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class UseBlockBodyOrExpressionBodyAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                {
                    Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.UseBlockBodyOrExpressionBody);
                }

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterCompilationStartAction(startContext =>
            {
                if (((CSharpCompilation)startContext.Compilation).LanguageVersion < LanguageVersion.CSharp6)
                    return;

                startContext.RegisterSyntaxNodeAction(f => AnalyzeMethodDeclaration(f), SyntaxKind.MethodDeclaration);
                startContext.RegisterSyntaxNodeAction(f => AnalyzePropertyDeclaration(f), SyntaxKind.PropertyDeclaration);
                startContext.RegisterSyntaxNodeAction(f => AnalyzeIndexerDeclaration(f), SyntaxKind.IndexerDeclaration);
                startContext.RegisterSyntaxNodeAction(f => AnalyzeOperatorDeclaration(f), SyntaxKind.OperatorDeclaration);
                startContext.RegisterSyntaxNodeAction(f => AnalyzeConversionOperatorDeclaration(f), SyntaxKind.ConversionOperatorDeclaration);
                startContext.RegisterSyntaxNodeAction(f => AnalyzeConstructorDeclaration(f), SyntaxKind.ConstructorDeclaration);
                startContext.RegisterSyntaxNodeAction(f => AnalyzeDestructorDeclaration(f), SyntaxKind.DestructorDeclaration);
                startContext.RegisterSyntaxNodeAction(f => AnalyzeLocalFunctionStatement(f), SyntaxKind.LocalFunctionStatement);

                startContext.RegisterSyntaxNodeAction(
                    f => AnalyzeAccessorDeclaration(f),
                    SyntaxKind.GetAccessorDeclaration,
                    SyntaxKind.SetAccessorDeclaration,
                    SyntaxKind.AddAccessorDeclaration,
                    SyntaxKind.RemoveAccessorDeclaration,
                    SyntaxKind.InitAccessorDeclaration);
            });
        }

        private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            BlockSyntax body = methodDeclaration.Body;
            if (body != null)
            {
                if (body.ContainsDirectives)
                    return;

                BodyStyle style = context.GetBodyStyle();

                if (style.IsDefault)
                    return;

                BlockExpressionAnalysis analysis = BlockExpressionAnalysis.Create(body);

                if (!analysis.Success)
                    return;

                if (!style.UseExpression)
                    return;

                if (style.UseBlockWhenDeclarationIsMultiLine == true
                    && methodDeclaration.SyntaxTree.IsMultiLineSpan(methodDeclaration.HeaderSpan()))
                {
                    return;
                }

                AnalyzeBlock(context, body, analysis, style);
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = methodDeclaration.ExpressionBody;

                if (expressionBody?.ContainsDirectives == false)
                {
                    BodyStyle style = context.GetBodyStyle();

                    if (style.IsDefault)
                        return;

                    if (style.UseBlock)
                    {
                        ReportDiagnostic(context, expressionBody);
                        return;
                    }

                    if (style.UseBlockWhenDeclarationIsMultiLine == true
                        && methodDeclaration.SyntaxTree.IsMultiLineSpan(methodDeclaration.HeaderSpan()))
                    {
                        ReportDiagnostic(context, expressionBody);
                        return;
                    }

                    if (style.UseBlockWhenExpressionIsMultiLine == true
                        && expressionBody.Expression?.IsMultiLine() == true)
                    {
                        ReportDiagnostic(context, expressionBody);
                    }
                }
            }
        }

        private static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

            ArrowExpressionClauseSyntax expressionBody = propertyDeclaration.ExpressionBody;

            if (expressionBody?.ContainsDirectives == false)
            {
                BodyStyle style = context.GetBodyStyle();

                if (style.IsDefault)
                    return;

                if (style.UseBlock)
                {
                    ReportDiagnostic(context, expressionBody);
                    return;
                }

                if (style.UseBlockWhenDeclarationIsMultiLine == true
                    && propertyDeclaration.SyntaxTree.IsMultiLineSpan(propertyDeclaration.HeaderSpan()))
                {
                    ReportDiagnostic(context, expressionBody);
                    return;
                }

                if (style.UseBlockWhenExpressionIsMultiLine == true
                    && expressionBody.Expression?.IsMultiLine() == true)
                {
                    ReportDiagnostic(context, expressionBody);
                }
            }
        }

        private static void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            ArrowExpressionClauseSyntax expressionBody = indexerDeclaration.ExpressionBody;

            if (expressionBody?.ContainsDirectives == false)
            {
                BodyStyle style = context.GetBodyStyle();

                if (style.IsDefault)
                    return;

                if (style.UseBlock)
                {
                    ReportDiagnostic(context, expressionBody);
                    return;
                }

                if (style.UseBlockWhenDeclarationIsMultiLine == true
                    && indexerDeclaration.SyntaxTree.IsMultiLineSpan(indexerDeclaration.HeaderSpan()))
                {
                    ReportDiagnostic(context, expressionBody);
                    return;
                }

                if (style.UseBlockWhenExpressionIsMultiLine == true
                    && expressionBody.Expression?.IsMultiLine() == true)
                {
                    ReportDiagnostic(context, expressionBody);
                }
            }
        }

        private static void AnalyzeOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var operatorDeclaration = (OperatorDeclarationSyntax)context.Node;

            BlockSyntax body = operatorDeclaration.Body;
            if (body != null)
            {
                if (body.ContainsDirectives)
                    return;

                BodyStyle style = context.GetBodyStyle();

                if (style.IsDefault)
                    return;

                BlockExpressionAnalysis analysis = BlockExpressionAnalysis.Create(body, allowExpressionStatement: false);

                if (!analysis.Success)
                    return;

                if (!style.UseExpression)
                    return;

                if (style.UseBlockWhenDeclarationIsMultiLine == true
                    && operatorDeclaration.SyntaxTree.IsMultiLineSpan(operatorDeclaration.HeaderSpan()))
                {
                    return;
                }

                AnalyzeBlock(context, body, analysis, style);
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = operatorDeclaration.ExpressionBody;

                if (expressionBody?.ContainsDirectives == false)
                {
                    BodyStyle style = context.GetBodyStyle();

                    if (style.IsDefault)
                        return;

                    if (style.UseBlock)
                    {
                        ReportDiagnostic(context, expressionBody);
                        return;
                    }

                    if (style.UseBlockWhenDeclarationIsMultiLine == true
                        && operatorDeclaration.SyntaxTree.IsMultiLineSpan(operatorDeclaration.HeaderSpan()))
                    {
                        ReportDiagnostic(context, expressionBody);
                        return;
                    }

                    if (style.UseBlockWhenExpressionIsMultiLine == true
                        && expressionBody.Expression?.IsMultiLine() == true)
                    {
                        ReportDiagnostic(context, expressionBody);
                    }
                }
            }
        }

        private static void AnalyzeConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var operatorDeclaration = (ConversionOperatorDeclarationSyntax)context.Node;

            BlockSyntax body = operatorDeclaration.Body;
            if (body != null)
            {
                if (body.ContainsDirectives)
                    return;

                BodyStyle style = context.GetBodyStyle();

                if (style.IsDefault)
                    return;

                BlockExpressionAnalysis analysis = BlockExpressionAnalysis.Create(body, allowExpressionStatement: false);

                if (!analysis.Success)
                    return;

                if (!style.UseExpression)
                    return;

                if (style.UseBlockWhenDeclarationIsMultiLine == true
                    && operatorDeclaration.SyntaxTree.IsMultiLineSpan(operatorDeclaration.HeaderSpan()))
                {
                    return;
                }

                AnalyzeBlock(context, body, analysis, style);
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = operatorDeclaration.ExpressionBody;

                if (expressionBody?.ContainsDirectives == false)
                {
                    BodyStyle style = context.GetBodyStyle();

                    if (style.IsDefault)
                        return;

                    if (style.UseBlock)
                    {
                        ReportDiagnostic(context, expressionBody);
                        return;
                    }

                    if (style.UseBlockWhenDeclarationIsMultiLine == true
                        && operatorDeclaration.SyntaxTree.IsMultiLineSpan(operatorDeclaration.HeaderSpan()))
                    {
                        ReportDiagnostic(context, expressionBody);
                        return;
                    }

                    if (style.UseBlockWhenExpressionIsMultiLine == true
                        && expressionBody.Expression?.IsMultiLine() == true)
                    {
                        ReportDiagnostic(context, expressionBody);
                    }
                }
            }
        }

        private static void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;

            BlockSyntax body = constructorDeclaration.Body;
            if (body != null)
            {
                if (body.ContainsDirectives)
                    return;

                BodyStyle style = context.GetBodyStyle();

                if (style.IsDefault)
                    return;

                BlockExpressionAnalysis analysis = BlockExpressionAnalysis.Create(body);

                if (!analysis.Success)
                    return;

                if (!style.UseExpression)
                    return;

                if (style.UseBlockWhenDeclarationIsMultiLine == true
                    && constructorDeclaration.SyntaxTree.IsMultiLineSpan(constructorDeclaration.HeaderSpan()))
                {
                    return;
                }

                AnalyzeBlock(context, body, analysis, style);
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = constructorDeclaration.ExpressionBody;

                if (expressionBody?.ContainsDirectives == false)
                {
                    BodyStyle style = context.GetBodyStyle();

                    if (style.IsDefault)
                        return;

                    if (style.UseBlock)
                    {
                        ReportDiagnostic(context, expressionBody);
                        return;
                    }

                    if (style.UseBlockWhenDeclarationIsMultiLine == true
                        && constructorDeclaration.SyntaxTree.IsMultiLineSpan(constructorDeclaration.HeaderSpan()))
                    {
                        ReportDiagnostic(context, expressionBody);
                        return;
                    }

                    if (style.UseBlockWhenExpressionIsMultiLine == true
                        && expressionBody.Expression?.IsMultiLine() == true)
                    {
                        ReportDiagnostic(context, expressionBody);
                    }
                }
            }
        }

        private static void AnalyzeDestructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var destructorDeclaration = (DestructorDeclarationSyntax)context.Node;

            BlockSyntax body = destructorDeclaration.Body;
            if (body != null)
            {
                if (body.ContainsDirectives)
                    return;

                BodyStyle style = context.GetBodyStyle();

                if (style.IsDefault)
                    return;

                BlockExpressionAnalysis analysis = BlockExpressionAnalysis.Create(body);

                if (!analysis.Success)
                    return;

                if (!style.UseExpression)
                    return;

                if (style.UseBlockWhenDeclarationIsMultiLine == true
                    && destructorDeclaration.SyntaxTree.IsMultiLineSpan(destructorDeclaration.HeaderSpan()))
                {
                    return;
                }

                AnalyzeBlock(context, body, analysis, style);
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = destructorDeclaration.ExpressionBody;

                if (expressionBody?.ContainsDirectives == false)
                {
                    BodyStyle style = context.GetBodyStyle();

                    if (style.IsDefault)
                        return;

                    if (style.UseBlock)
                    {
                        ReportDiagnostic(context, expressionBody);
                        return;
                    }

                    if (style.UseBlockWhenDeclarationIsMultiLine == true
                        && destructorDeclaration.SyntaxTree.IsMultiLineSpan(destructorDeclaration.HeaderSpan()))
                    {
                        ReportDiagnostic(context, expressionBody);
                        return;
                    }

                    if (style.UseBlockWhenExpressionIsMultiLine == true
                        && expressionBody.Expression?.IsMultiLine() == true)
                    {
                        ReportDiagnostic(context, expressionBody);
                    }
                }
            }
        }

        private static void AnalyzeLocalFunctionStatement(SyntaxNodeAnalysisContext context)
        {
            var localFunction = (LocalFunctionStatementSyntax)context.Node;

            BlockSyntax body = localFunction.Body;
            if (body != null)
            {
                if (body.ContainsDirectives)
                    return;

                BodyStyle style = context.GetBodyStyle();

                if (style.IsDefault)
                    return;

                BlockExpressionAnalysis analysis = BlockExpressionAnalysis.Create(body);

                if (!analysis.Success)
                    return;

                if (!style.UseExpression)
                    return;

                if (style.UseBlockWhenDeclarationIsMultiLine == true
                    && localFunction.SyntaxTree.IsMultiLineSpan(localFunction.HeaderSpan()))
                {
                    return;
                }

                AnalyzeBlock(context, body, analysis, style);
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = localFunction.ExpressionBody;

                if (expressionBody?.ContainsDirectives == false)
                {
                    BodyStyle style = context.GetBodyStyle();

                    if (style.IsDefault)
                        return;

                    if (style.UseBlock)
                    {
                        ReportDiagnostic(context, expressionBody);
                        return;
                    }

                    if (style.UseBlockWhenDeclarationIsMultiLine == true
                        && localFunction.SyntaxTree.IsMultiLineSpan(localFunction.HeaderSpan()))
                    {
                        ReportDiagnostic(context, expressionBody);
                        return;
                    }

                    if (style.UseBlockWhenExpressionIsMultiLine == true
                        && expressionBody.Expression?.IsMultiLine() == true)
                    {
                        ReportDiagnostic(context, expressionBody);
                    }
                }
            }
        }

        private static void AnalyzeAccessorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var accessor = (AccessorDeclarationSyntax)context.Node;

            BlockSyntax body = accessor.Body;

            if (body != null)
            {
                AnalyzeAccessorDeclarationBlock(context, accessor, body);
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = accessor.ExpressionBody;

                if (expressionBody?.ContainsDirectives == false)
                {
                    BodyStyle style = context.GetBodyStyle();

                    if (style.IsDefault)
                        return;

                    if (style.UseBlock)
                    {
                        ReportDiagnostic(context, expressionBody);
                        return;
                    }

                    if (style.UseBlockWhenExpressionIsMultiLine == true
                        && expressionBody.Expression?.IsMultiLine() == true)
                    {
                        ReportDiagnostic(context, expressionBody);
                    }
                }
            }
        }

        private static void AnalyzeAccessorDeclarationBlock(
            SyntaxNodeAnalysisContext context,
            AccessorDeclarationSyntax accessor,
            BlockSyntax body)
        {
            if (body.ContainsDirectives)
                return;

            if (accessor.AttributeLists.Any())
                return;

            BodyStyle style = context.GetBodyStyle();

            if (style.IsDefault)
                return;

            bool isGetter = accessor.IsKind(SyntaxKind.GetAccessorDeclaration);

            BlockExpressionAnalysis analysis = BlockExpressionAnalysis.Create(body, allowExpressionStatement: !isGetter);

            ExpressionSyntax expression = analysis.Expression;

            if (expression == null)
                return;

            if (!style.UseExpression)
                return;

            if (style.UseBlockWhenExpressionIsMultiLine == true
                && expression.IsMultiLine())
            {
                return;
            }

            if (isGetter
                && accessor.Parent is AccessorListSyntax accessorList
                && accessorList.Accessors.Count == 1)
            {
                if (!SyntaxTriviaAnalysis.IsExteriorTriviaEmptyOrWhitespace(accessorList.OpenBraceToken))
                    return;

                if (!SyntaxTriviaAnalysis.IsExteriorTriviaEmptyOrWhitespace(accessor.Keyword))
                    return;

                if (!SyntaxTriviaAnalysis.IsExteriorTriviaEmptyOrWhitespace(body.OpenBraceToken))
                    return;

                if (style.UseBlockWhenDeclarationIsMultiLine == true)
                {
                    switch (accessorList.Parent.Kind())
                    {
                        case SyntaxKind.PropertyDeclaration:
                            {
                                if (accessor.SyntaxTree.IsMultiLineSpan(((PropertyDeclarationSyntax)accessorList.Parent).HeaderSpan()))
                                    return;

                                break;
                            }
                        case SyntaxKind.IndexerDeclaration:
                            {
                                if (accessor.SyntaxTree.IsMultiLineSpan(((IndexerDeclarationSyntax)accessorList.Parent).HeaderSpan()))
                                    return;

                                break;
                            }
                        default:
                            {
                                SyntaxDebug.Fail(accessorList.Parent);
                                break;
                            }
                    }

                    return;
                }

                ReportDiagnostic(context, accessorList);
                return;
            }

            if (!accessor.Keyword.TrailingTrivia.IsEmptyOrWhitespace())
                return;

            if (!SyntaxTriviaAnalysis.IsExteriorTriviaEmptyOrWhitespace(body.OpenBraceToken))
                return;

            if (!accessor.Keyword.LeadingTrivia.IsEmptyOrWhitespace())
                return;

            ReportDiagnostic(context, body);
        }

        private static void AnalyzeBlock(SyntaxNodeAnalysisContext context, BlockSyntax block, BlockExpressionAnalysis analysis, BodyStyle style)
        {
            if (style.UseBlockWhenExpressionIsMultiLine == true
                && analysis.Expression.IsMultiLine())
            {
                return;
            }

            if (!style.UseExpression)
                return;

            if (!SyntaxTriviaAnalysis.IsExteriorTriviaEmptyOrWhitespace(block.OpenBraceToken))
                return;

            if (!analysis.ReturnOrThrowKeyword.LeadingTrivia.IsEmptyOrWhitespace())
                return;

            ReportDiagnostic(context, analysis.Block);
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, AccessorListSyntax accessorList)
        {
            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.UseBlockBodyOrExpressionBody, accessorList, "expression");
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, BlockSyntax block)
        {
            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.UseBlockBodyOrExpressionBody, block, "expression");
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, ArrowExpressionClauseSyntax expressionBody)
        {
            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.UseBlockBodyOrExpressionBody, expressionBody, "block");
        }
    }
}
