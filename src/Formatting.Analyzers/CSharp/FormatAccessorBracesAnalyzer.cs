// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class FormatAccessorBracesAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                {
                    Immutable.InterlockedInitialize(
                        ref _supportedDiagnostics,
                        DiagnosticRules.FormatAccessorBraces,
                        DiagnosticRules.FormatAccessorBracesOnSingleLineWhenExpressionIsOnSingleLine);
                }

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(
                f => AnalyzeAccessorDeclaration(f),
                SyntaxKind.GetAccessorDeclaration,
                SyntaxKind.SetAccessorDeclaration,
                SyntaxKind.InitAccessorDeclaration,
                SyntaxKind.AddAccessorDeclaration,
                SyntaxKind.RemoveAccessorDeclaration);
        }

        private static void AnalyzeAccessorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var accessor = (AccessorDeclarationSyntax)context.Node;

            BlockSyntax block = accessor.Body;

            if (block == null)
                return;

            SyntaxToken openBrace = block.OpenBraceToken;

            if (openBrace.IsMissing)
                return;

            if (DiagnosticRules.FormatAccessorBracesOnSingleLineWhenExpressionIsOnSingleLine.IsEffective(context)
                && accessor.SyntaxTree.IsMultiLineSpan(TextSpan.FromBounds(accessor.Keyword.SpanStart, accessor.Span.End))
                && CanBeMadeSingleLine(accessor))
            {
                DiagnosticHelpers.ReportDiagnostic(
                    context,
                    DiagnosticRules.FormatAccessorBracesOnSingleLineWhenExpressionIsOnSingleLine,
                    accessor);
            }

            AccessorBracesStyle style = context.GetAccessorBracesStyle();

            if (style == AccessorBracesStyle.None)
                return;

            if (accessor.SyntaxTree.IsSingleLineSpan(TextSpan.FromBounds(accessor.Keyword.SpanStart, accessor.Span.End)))
            {
                if (style == AccessorBracesStyle.MultiLine
                    || !CanBeMadeSingleLine(accessor))
                {
                    DiagnosticHelpers.ReportDiagnostic(
                        context,
                        DiagnosticRules.FormatAccessorBraces,
                        block.OpenBraceToken,
                        "multiple lines");
                }
            }
            else if (style == AccessorBracesStyle.SingleLineWhenExpressionIsOnSingleLine
                && CanBeMadeSingleLine(accessor))
            {
                DiagnosticHelpers.ReportDiagnostic(
                    context,
                    DiagnosticRules.FormatAccessorBraces,
                    block.OpenBraceToken,
                    "a single line");
            }
        }

        private static bool CanBeMadeSingleLine(AccessorDeclarationSyntax accessor)
        {
            SyntaxList<StatementSyntax> statements = accessor.Body.Statements;

            if (statements.Count <= 1
                && (!statements.Any() || statements[0].IsSingleLine()))
            {
                return accessor
                    .DescendantTrivia(TextSpan.FromBounds(accessor.Keyword.SpanStart, accessor.Span.End), descendIntoTrivia: true)
                    .All(f => f.IsWhitespaceOrEndOfLineTrivia());
            }

            return false;
        }
    }
}
