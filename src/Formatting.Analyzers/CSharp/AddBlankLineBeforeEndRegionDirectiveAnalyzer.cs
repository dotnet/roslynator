﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AddBlankLineBeforeEndRegionDirectiveAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.AddBlankLineBeforeEndRegionDirective);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeEndRegionDirectiveTrivia(f), SyntaxKind.EndRegionDirectiveTrivia);
    }

    private static void AnalyzeEndRegionDirectiveTrivia(SyntaxNodeAnalysisContext context)
    {
        var endRegionDirective = (EndRegionDirectiveTriviaSyntax)context.Node;

        if (!IsPrecededWithEmptyLineOrRegionDirective(endRegionDirective))
        {
            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticRules.AddBlankLineBeforeEndRegionDirective,
                Location.Create(endRegionDirective.SyntaxTree, endRegionDirective.Span.WithLength(0)));
        }

        static bool IsPrecededWithEmptyLineOrRegionDirective(EndRegionDirectiveTriviaSyntax endRegionDirective)
        {
            SyntaxTrivia parentTrivia = endRegionDirective.ParentTrivia;

            SyntaxTriviaList.Reversed.Enumerator en = parentTrivia.Token.LeadingTrivia.Reverse().GetEnumerator();

            while (en.MoveNext())
            {
                if (en.Current == parentTrivia)
                {
                    if (!en.MoveNext())
                        return false;

                    if (en.Current.IsWhitespaceTrivia()
                        && !en.MoveNext())
                    {
                        return false;
                    }

                    if (en.Current.IsKind(SyntaxKind.RegionDirectiveTrivia))
                        return true;

                    if (!en.Current.IsEndOfLineTrivia())
                        return false;

                    if (!en.MoveNext())
                        return true;

                    if (en.Current.IsWhitespaceTrivia()
                        && !en.MoveNext())
                    {
                        return false;
                    }

                    return en.Current.IsKind(
                        SyntaxKind.EndOfLineTrivia,
                        SyntaxKind.RegionDirectiveTrivia,
                        SyntaxKind.EndRegionDirectiveTrivia,
                        SyntaxKind.PragmaWarningDirectiveTrivia,
                        SyntaxKind.EndIfDirectiveTrivia);
                }
            }

            return false;
        }
    }
}
