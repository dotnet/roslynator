// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class NormalizeWhitespaceAtEndOfFileAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                {
                    Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.NormalizeWhitespaceAtEndOfFile);
                }

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeCompilationUnit(f), SyntaxKind.CompilationUnit);
        }

        private static void AnalyzeCompilationUnit(SyntaxNodeAnalysisContext context)
        {
            var compilationUnit = (CompilationUnitSyntax)context.Node;

            SyntaxToken endOfFile = compilationUnit.EndOfFileToken;
            SyntaxTriviaList.Reversed.Enumerator en = endOfFile.LeadingTrivia.Reverse().GetEnumerator();

            bool? preferNewLineAtEndOfFile = context.PreferNewLineAtEndOfFile();

            if (preferNewLineAtEndOfFile == null)
                return;

            if (preferNewLineAtEndOfFile == false)
            {
                if (en.MoveNext()
                    && (!en.Current.IsWhitespaceTrivia()
                        || en.MoveNext()))
                {
                    if (en.Current.IsEndOfLineTrivia())
                    {
                        ReportDiagnostic(context, endOfFile);
                    }
                    else if (SyntaxFacts.IsPreprocessorDirective(en.Current.Kind())
                        && en.Current.GetStructure() is DirectiveTriviaSyntax directiveTrivia
                        && directiveTrivia.GetTrailingTrivia().LastOrDefault().IsEndOfLineTrivia())
                    {
                        ReportDiagnostic(context, endOfFile);
                    }
                }
                else
                {
                    SyntaxTriviaList trailing = endOfFile.GetPreviousToken().TrailingTrivia;

                    if (trailing.Any())
                    {
                        Debug.Assert(endOfFile.FullSpan.Start == trailing.Span.End);

                        if (endOfFile.FullSpan.Start == trailing.Span.End
                            && trailing.LastOrDefault().IsEndOfLineTrivia())
                        {
                            ReportDiagnostic(context, endOfFile);
                        }
                    }
                }
            }
            else if (en.MoveNext())
            {
                if (CSharpFacts.IsCommentTrivia(en.Current.Kind())
                    || SyntaxFacts.IsPreprocessorDirective(en.Current.Kind()))
                {
                    ReportDiagnostic(context, endOfFile);
                }
                else if (en.Current.IsWhitespaceOrEndOfLineTrivia()
                    && endOfFile.LeadingTrivia.Span.Start == 0)
                {
                    while (en.MoveNext())
                    {
                        if (!en.Current.IsWhitespaceOrEndOfLineTrivia())
                            return;
                    }

                    ReportDiagnostic(context, endOfFile);
                }
            }
            else if (endOfFile.SpanStart > 0)
            {
                SyntaxTriviaList trailing = endOfFile.GetPreviousToken().TrailingTrivia;

                if (!trailing.Any())
                {
                    ReportDiagnostic(context, endOfFile);
                }
                else
                {
                    Debug.Assert(endOfFile.FullSpan.Start == trailing.Span.End);

                    if (endOfFile.FullSpan.Start == trailing.Span.End
                        && !trailing.Last().IsEndOfLineTrivia())
                    {
                        ReportDiagnostic(context, endOfFile);
                    }
                }
            }

            static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxToken eof)
            {
                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.NormalizeWhitespaceAtEndOfFile, eof);
            }
        }
    }
}
