// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class AddEmptyLineBeforeUsingDirectiveListAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddEmptyLineBeforeUsingDirectiveList); }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeCompilationUnit, SyntaxKind.CompilationUnit);
        }

        private static void AnalyzeCompilationUnit(SyntaxNodeAnalysisContext context)
        {
            var compilationUnit = (CompilationUnitSyntax)context.Node;

            UsingDirectiveSyntax usingDirective = compilationUnit.Usings.FirstOrDefault();

            if (usingDirective == null)
                return;

            SyntaxTriviaList.Reversed.Enumerator en = usingDirective.GetLeadingTrivia().Reverse().GetEnumerator();

            if (en.MoveNext())
            {
                if (en.Current.IsWhitespaceTrivia()
                    && !en.MoveNext())
                {
                    if (IsPrecededWithExternAliasDirective())
                        ReportDiagnostic(usingDirective.SpanStart);
                }
                else
                {
                    switch (en.Current.Kind())
                    {
                        case SyntaxKind.EndOfLineTrivia:
                            {
                                if (en.MoveNext()
                                    && en.Current.IsKind(SyntaxKind.SingleLineCommentTrivia))
                                {
                                    ReportDiagnostic(usingDirective.SpanStart);
                                }

                                break;
                            }
                        case SyntaxKind.RegionDirectiveTrivia:
                            {
                                SyntaxTrivia regionDirective = en.Current;

                                if (en.MoveNext())
                                {
                                    if (en.Current.IsWhitespaceTrivia()
                                        && !en.MoveNext())
                                    {
                                        if (IsPrecededWithExternAliasDirective())
                                            ReportDiagnostic(regionDirective.SpanStart);
                                    }
                                    else if (en.Current.IsEndOfLineTrivia()
                                        && en.MoveNext()
                                        && en.Current.IsKind(SyntaxKind.SingleLineCommentTrivia))
                                    {
                                        ReportDiagnostic(regionDirective.SpanStart);
                                    }
                                }
                                else if (IsPrecededWithExternAliasDirective())
                                {
                                    ReportDiagnostic(regionDirective.SpanStart);
                                }

                                break;
                            }
                        default:
                            {
                                if (en.Current.IsDirective)
                                    ReportDiagnostic(usingDirective.SpanStart);

                                break;
                            }
                    }
                }
            }
            else if (IsPrecededWithExternAliasDirective())
            {
                ReportDiagnostic(usingDirective.SpanStart);
            }

            bool IsPrecededWithExternAliasDirective()
            {
                ExternAliasDirectiveSyntax externAliasDirective = compilationUnit.Externs.LastOrDefault();

                return externAliasDirective?.FullSpan.End == usingDirective.FullSpan.Start
                    && SyntaxTriviaAnalysis.IsOptionalWhitespaceThenOptionalSingleLineCommentThenEndOfLineTrivia(externAliasDirective.GetTrailingTrivia());
            }

            void ReportDiagnostic(int position)
            {
                context.ReportDiagnostic(
                   DiagnosticDescriptors.AddEmptyLineBeforeUsingDirectiveList,
                   Location.Create(compilationUnit.SyntaxTree, new TextSpan(position, 0)));
            }
        }
    }
}
