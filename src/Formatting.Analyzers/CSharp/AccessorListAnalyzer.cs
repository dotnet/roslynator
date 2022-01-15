// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class AccessorListAnalyzer : BaseDiagnosticAnalyzer
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
                        DiagnosticRules.PutAutoAccessorsOnSingleLine,
                        DiagnosticRules.PutFullAccessorOnItsOwnLine);
                }

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeAccessorList(f), SyntaxKind.AccessorList);
        }

        private static void AnalyzeAccessorList(SyntaxNodeAnalysisContext context)
        {
            var accessorList = (AccessorListSyntax)context.Node;

            SyntaxList<AccessorDeclarationSyntax> accessors = accessorList.Accessors;

            if (accessors.Any(f => f.BodyOrExpressionBody() != null))
            {
                if (DiagnosticRules.PutFullAccessorOnItsOwnLine.IsEffective(context))
                {
                    SyntaxToken token = accessorList.OpenBraceToken;

                    foreach (AccessorDeclarationSyntax accessor in accessors)
                    {
                        if (accessor.BodyOrExpressionBody() != null
                            && accessor.SyntaxTree.IsSingleLineSpan(TextSpan.FromBounds(token.Span.End, accessor.SpanStart)))
                        {
                            DiagnosticHelpers.ReportDiagnostic(
                                context,
                                DiagnosticRules.PutFullAccessorOnItsOwnLine,
                                Location.Create(accessor.SyntaxTree, new TextSpan(accessor.SpanStart, 0)));

                            break;
                        }

                        token = accessor.Body?.CloseBraceToken ?? accessor.SemicolonToken;

                        if (!token.Equals(accessor.GetLastToken()))
                            break;
                    }
                }
            }
            else if (DiagnosticRules.PutAutoAccessorsOnSingleLine.IsEffective(context))
            {
                SyntaxNode parent = accessorList.Parent;

                switch (parent?.Kind())
                {
                    case SyntaxKind.PropertyDeclaration:
                        {
                            if (accessors.All(f => !f.AttributeLists.Any())
                                && !accessorList.IsSingleLine(includeExteriorTrivia: false))
                            {
                                var propertyDeclaration = (PropertyDeclarationSyntax)parent;
                                SyntaxToken identifier = propertyDeclaration.Identifier;

                                if (!identifier.IsMissing)
                                {
                                    SyntaxToken closeBrace = accessorList.CloseBraceToken;

                                    if (!closeBrace.IsMissing)
                                    {
                                        TextSpan span = TextSpan.FromBounds(identifier.Span.End, closeBrace.SpanStart);

                                        if (propertyDeclaration
                                            .DescendantTrivia(span)
                                            .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                                        {
                                            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.PutAutoAccessorsOnSingleLine, accessorList);
                                        }
                                    }
                                }
                            }

                            break;
                        }
                    case SyntaxKind.IndexerDeclaration:
                        {
                            if (accessors.All(f => !f.AttributeLists.Any())
                                && !accessorList.IsSingleLine(includeExteriorTrivia: false))
                            {
                                var indexerDeclaration = (IndexerDeclarationSyntax)parent;

                                BracketedParameterListSyntax parameterList = indexerDeclaration.ParameterList;

                                if (parameterList != null)
                                {
                                    SyntaxToken closeBracket = parameterList.CloseBracketToken;

                                    if (!closeBracket.IsMissing)
                                    {
                                        SyntaxToken closeBrace = accessorList.CloseBraceToken;

                                        if (!closeBrace.IsMissing)
                                        {
                                            TextSpan span = TextSpan.FromBounds(closeBracket.Span.End, closeBrace.SpanStart);

                                            if (indexerDeclaration
                                                .DescendantTrivia(span)
                                                .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                                            {
                                                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.PutAutoAccessorsOnSingleLine, accessorList);
                                            }
                                        }
                                    }
                                }
                            }

                            break;
                        }
                }
            }
        }
    }
}
