// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class PlaceNewLineAfterOrBeforeEqualsTokenAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                {
                    Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.PlaceNewLineAfterOrBeforeEqualsToken);
                }

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeNameEquals(f), SyntaxKind.NameEquals);
            context.RegisterSyntaxNodeAction(f => AnalyzeEqualsValueClause(f), SyntaxKind.EqualsValueClause);
            context.RegisterSyntaxNodeAction(f => AnalyzeAssignmentExpression(f), SyntaxKind.AddAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeAssignmentExpression(f), SyntaxKind.AndAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeAssignmentExpression(f), SyntaxKind.CoalesceAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeAssignmentExpression(f), SyntaxKind.DivideAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeAssignmentExpression(f), SyntaxKind.ExclusiveOrAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeAssignmentExpression(f), SyntaxKind.LeftShiftAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeAssignmentExpression(f), SyntaxKind.ModuloAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeAssignmentExpression(f), SyntaxKind.MultiplyAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeAssignmentExpression(f), SyntaxKind.OrAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeAssignmentExpression(f), SyntaxKind.RightShiftAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeAssignmentExpression(f), SyntaxKind.SimpleAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeAssignmentExpression(f), SyntaxKind.SubtractAssignmentExpression);
        }

        private static void AnalyzeNameEquals(SyntaxNodeAnalysisContext context)
        {
            var node = (NameEqualsSyntax)context.Node;

            switch (node.Parent.Kind())
            {
                case SyntaxKind.AttributeArgument:
                    {
                        var attributeArgument = (AttributeArgumentSyntax)node.Parent;

                        Analyze(context, attributeArgument.NameEquals.EqualsToken, attributeArgument.Expression);
                        break;
                    }
                case SyntaxKind.AnonymousObjectMemberDeclarator:
                    {
                        var declarator = (AnonymousObjectMemberDeclaratorSyntax)node.Parent;

                        Analyze(context, declarator.NameEquals.EqualsToken, declarator.Expression);
                        break;
                    }
                case SyntaxKind.UsingDirective:
                    {
                        var usingDirective = (UsingDirectiveSyntax)node.Parent;

                        Analyze(context, usingDirective.Alias.EqualsToken, usingDirective.Name);
                        break;
                    }
                default:
                    {
                        SyntaxDebug.Fail(node.Parent);
                        break;
                    }
            }
        }

        private static void AnalyzeEqualsValueClause(SyntaxNodeAnalysisContext context = default)
        {
            var node = (EqualsValueClauseSyntax)context.Node;

            Analyze(context, node.EqualsToken, node.Value);
        }

        private static void AnalyzeAssignmentExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (AssignmentExpressionSyntax)context.Node;

            Analyze(context, node.OperatorToken, node.Right);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, SyntaxToken token, ExpressionSyntax expression)
        {
            NewLinePosition newLinePosition = context.GetEqualsSignNewLinePosition();

            if (newLinePosition == NewLinePosition.None)
                return;

            FormattingSuggestion suggestion = FormattingAnalysis.AnalyzeNewLineBeforeOrAfter(token, expression, newLinePosition);

            if (suggestion == FormattingSuggestion.AddNewLineBefore)
            {
                DiagnosticHelpers.ReportDiagnostic(
                    context,
                    DiagnosticRules.PlaceNewLineAfterOrBeforeEqualsToken,
                    token.GetLocation(),
                    "before");
            }
            else if (suggestion == FormattingSuggestion.AddNewLineAfter)
            {
                DiagnosticHelpers.ReportDiagnostic(
                    context,
                    DiagnosticRules.PlaceNewLineAfterOrBeforeEqualsToken,
                    token.GetLocation(),
                    properties: DiagnosticProperties.AnalyzerOption_Invert,
                    "after");
            }
        }
    }
}
