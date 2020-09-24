// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConvertBlockBodyToExpressionBodyOrViceVersaAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.ConvertBlockBodyToExpressionBodyOrViceVersa,
                    DiagnosticDescriptors.ConvertBlockBodyToExpressionBodyOrViceVersaFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterCompilationStartAction(startContext =>
            {
                if (startContext.IsAnalyzerSuppressed(DiagnosticDescriptors.ConvertBlockBodyToExpressionBodyOrViceVersa))
                    return;

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
                startContext.RegisterSyntaxNodeAction(f => AnalyzeAccessorDeclaration(f), SyntaxKind.GetAccessorDeclaration);
                startContext.RegisterSyntaxNodeAction(f => AnalyzeAccessorDeclaration(f), SyntaxKind.SetAccessorDeclaration);
                startContext.RegisterSyntaxNodeAction(f => AnalyzeAccessorDeclaration(f), SyntaxKind.AddAccessorDeclaration);
                startContext.RegisterSyntaxNodeAction(f => AnalyzeAccessorDeclaration(f), SyntaxKind.RemoveAccessorDeclaration);
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

                BlockExpressionAnalysis analysis = BlockExpressionAnalysis.Create(body);

                if (!analysis.Success)
                    return;

                if (!context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine)
                    && methodDeclaration.SyntaxTree.IsMultiLineSpan(methodDeclaration.HeaderSpan()))
                {
                    return;
                }

                AnalyzeBlock(context, body, analysis);
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = methodDeclaration.ExpressionBody;

                if (expressionBody?.ContainsDirectives == false)
                {
                    if (!context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine)
                        && methodDeclaration.SyntaxTree.IsMultiLineSpan(methodDeclaration.HeaderSpan()))
                    {
                        DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.ReportOnly.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine, expressionBody);
                        return;
                    }

                    if (!context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine)
                        && expressionBody.Expression?.IsMultiLine() == true)
                    {
                        DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.ReportOnly.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine, expressionBody);
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
                if (!context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine)
                    && propertyDeclaration.SyntaxTree.IsMultiLineSpan(propertyDeclaration.HeaderSpan()))
                {
                    DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.ReportOnly.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine, expressionBody);
                    return;
                }

                if (!context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine)
                    && expressionBody.Expression?.IsMultiLine() == true)
                {
                    DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.ReportOnly.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine, expressionBody);
                }
            }
        }

        private static void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            ArrowExpressionClauseSyntax expressionBody = indexerDeclaration.ExpressionBody;

            if (expressionBody?.ContainsDirectives == false)
            {
                if (!context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine)
                    && indexerDeclaration.SyntaxTree.IsMultiLineSpan(indexerDeclaration.HeaderSpan()))
                {
                    DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.ReportOnly.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine, expressionBody);
                    return;
                }

                if (!context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine)
                    && expressionBody.Expression?.IsMultiLine() == true)
                {
                    DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.ReportOnly.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine, expressionBody);
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

                BlockExpressionAnalysis analysis = BlockExpressionAnalysis.Create(body, allowExpressionStatement: false);

                if (!analysis.Success)
                    return;

                if (!context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine)
                    && operatorDeclaration.SyntaxTree.IsMultiLineSpan(operatorDeclaration.HeaderSpan()))
                {
                    return;
                }

                AnalyzeBlock(context, body, analysis);
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = operatorDeclaration.ExpressionBody;

                if (expressionBody?.ContainsDirectives == false)
                {
                    if (!context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine)
                        && operatorDeclaration.SyntaxTree.IsMultiLineSpan(operatorDeclaration.HeaderSpan()))
                    {
                        DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.ReportOnly.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine, expressionBody);
                        return;
                    }

                    if (!context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine)
                        && expressionBody.Expression?.IsMultiLine() == true)
                    {
                        DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.ReportOnly.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine, expressionBody);
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

                BlockExpressionAnalysis analysis = BlockExpressionAnalysis.Create(body, allowExpressionStatement: false);

                if (!analysis.Success)
                    return;

                if (!context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine)
                    && operatorDeclaration.SyntaxTree.IsMultiLineSpan(operatorDeclaration.HeaderSpan()))
                {
                    return;
                }

                AnalyzeBlock(context, body, analysis);
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = operatorDeclaration.ExpressionBody;

                if (expressionBody?.ContainsDirectives == false)
                {
                    if (!context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine)
                        && operatorDeclaration.SyntaxTree.IsMultiLineSpan(operatorDeclaration.HeaderSpan()))
                    {
                        DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.ReportOnly.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine, expressionBody);
                        return;
                    }

                    if (!context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine)
                        && expressionBody.Expression?.IsMultiLine() == true)
                    {
                        DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.ReportOnly.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine, expressionBody);
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

                BlockExpressionAnalysis analysis = BlockExpressionAnalysis.Create(body);

                if (!analysis.Success)
                    return;

                if (!context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine)
                    && constructorDeclaration.SyntaxTree.IsMultiLineSpan(constructorDeclaration.HeaderSpan()))
                {
                    return;
                }

                AnalyzeBlock(context, body, analysis);
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = constructorDeclaration.ExpressionBody;

                if (expressionBody?.ContainsDirectives == false)
                {
                    if (!context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine)
                        && constructorDeclaration.SyntaxTree.IsMultiLineSpan(constructorDeclaration.HeaderSpan()))
                    {
                        DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.ReportOnly.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine, expressionBody);
                        return;
                    }

                    if (!context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine)
                        && expressionBody.Expression?.IsMultiLine() == true)
                    {
                        DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.ReportOnly.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine, expressionBody);
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

                BlockExpressionAnalysis analysis = BlockExpressionAnalysis.Create(body);

                if (!analysis.Success)
                    return;

                if (!context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine)
                    && destructorDeclaration.SyntaxTree.IsMultiLineSpan(destructorDeclaration.HeaderSpan()))
                {
                    return;
                }

                AnalyzeBlock(context, body, analysis);
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = destructorDeclaration.ExpressionBody;

                if (expressionBody?.ContainsDirectives == false)
                {
                    if (!context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine)
                        && destructorDeclaration.SyntaxTree.IsMultiLineSpan(destructorDeclaration.HeaderSpan()))
                    {
                        DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.ReportOnly.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine, expressionBody);
                        return;
                    }

                    if (!context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine)
                        && expressionBody.Expression?.IsMultiLine() == true)
                    {
                        DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.ReportOnly.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine, expressionBody);
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

                BlockExpressionAnalysis analysis = BlockExpressionAnalysis.Create(body);

                if (!analysis.Success)
                    return;

                if (!context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine)
                    && localFunction.SyntaxTree.IsMultiLineSpan(localFunction.HeaderSpan()))
                {
                    return;
                }

                AnalyzeBlock(context, body, analysis);
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = localFunction.ExpressionBody;

                if (expressionBody?.ContainsDirectives == false)
                {
                    if (!context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine)
                        && localFunction.SyntaxTree.IsMultiLineSpan(localFunction.HeaderSpan()))
                    {
                        DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.ReportOnly.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine, expressionBody);
                        return;
                    }

                    if (!context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine)
                        && expressionBody.Expression?.IsMultiLine() == true)
                    {
                        DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.ReportOnly.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine, expressionBody);
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

                if (expressionBody?.ContainsDirectives == false
                    && !context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine)
                    && expressionBody.Expression?.IsMultiLine() == true)
                {
                    DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.ReportOnly.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine, expressionBody);
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

            bool isGetter = accessor.IsKind(SyntaxKind.GetAccessorDeclaration);

            BlockExpressionAnalysis analysis = BlockExpressionAnalysis.Create(body, allowExpressionStatement: !isGetter);

            ExpressionSyntax expression = analysis.Expression;

            if (expression == null)
                return;

            if (!context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine)
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

                if (!context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine))
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
                                Debug.Fail(accessorList.Parent.Kind().ToString());
                                break;
                            }
                    }

                    return;
                }

                ReportDiagnostic(context, accessorList, expression);
                ReportFadeOut(context, accessor.Keyword, body);
                return;
            }

            if (!accessor.Keyword.TrailingTrivia.IsEmptyOrWhitespace())
                return;

            if (!SyntaxTriviaAnalysis.IsExteriorTriviaEmptyOrWhitespace(body.OpenBraceToken))
                return;

            if (!accessor.Keyword.LeadingTrivia.IsEmptyOrWhitespace())
                return;

            ReportDiagnostic(context, body, expression);
        }

        private static void AnalyzeBlock(SyntaxNodeAnalysisContext context, BlockSyntax block, BlockExpressionAnalysis analysis)
        {
            if (!context.IsAnalyzerSuppressed(AnalyzerOptions.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine)
                && analysis.Expression.IsMultiLine())
            {
                return;
            }

            if (!SyntaxTriviaAnalysis.IsExteriorTriviaEmptyOrWhitespace(block.OpenBraceToken))
                return;

            if (!analysis.ReturnOrThrowKeyword.LeadingTrivia.IsEmptyOrWhitespace())
                return;

            ReportDiagnostic(context, analysis.Block, analysis.Expression);
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxNode blockOrAccessorList, ExpressionSyntax expression)
        {
            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.ConvertBlockBodyToExpressionBodyOrViceVersa, blockOrAccessorList);

            ReportFadeOut(context, (expression.Parent as ReturnStatementSyntax)?.ReturnKeyword ?? default, blockOrAccessorList);
        }

        private static void ReportFadeOut(SyntaxNodeAnalysisContext context, SyntaxToken token, SyntaxNode blockOrAccessorList)
        {
            DiagnosticDescriptor fadeOutAnalyzer = DiagnosticDescriptors.ConvertBlockBodyToExpressionBodyOrViceVersaFadeOut;

            if (token != default)
                DiagnosticHelpers.ReportToken(context, fadeOutAnalyzer, token);

            switch (blockOrAccessorList)
            {
                case BlockSyntax block:
                    {
                        CSharpDiagnosticHelpers.ReportBraces(context, fadeOutAnalyzer, block);
                        break;
                    }
                case AccessorListSyntax accessorList:
                    {
                        CSharpDiagnosticHelpers.ReportBraces(context, fadeOutAnalyzer, accessorList);
                        break;
                    }
                default:
                    {
                        Debug.Fail(blockOrAccessorList.Kind().ToString());
                        break;
                    }
            }
        }
    }
}
