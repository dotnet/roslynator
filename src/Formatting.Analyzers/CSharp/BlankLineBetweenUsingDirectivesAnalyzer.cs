// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.Formatting.CSharp;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class BlankLineBetweenUsingDirectivesAnalyzer : BaseDiagnosticAnalyzer
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
                    DiagnosticRules.RemoveBlankLineBetweenUsingDirectivesWithSameRootNamespace,
                    DiagnosticRules.BlankLineBetweenUsingDirectives);
            }

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeCompilationUnit(f), SyntaxKind.CompilationUnit);
        context.RegisterSyntaxNodeAction(f => AnalyzeNamespaceDeclaration(f), SyntaxKind.NamespaceDeclaration);
    }

    private static void AnalyzeCompilationUnit(SyntaxNodeAnalysisContext context)
    {
        var compilationUnit = (CompilationUnitSyntax)context.Node;

        AnalyzeUsings(context, compilationUnit.Usings);
    }

    private static void AnalyzeNamespaceDeclaration(SyntaxNodeAnalysisContext context)
    {
        var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;

        AnalyzeUsings(context, namespaceDeclaration.Usings);
    }

    private static void AnalyzeUsings(SyntaxNodeAnalysisContext context, SyntaxList<UsingDirectiveSyntax> usings)
    {
        int count = usings.Count;

        if (count <= 1)
            return;

        UsingDirectiveSyntax usingDirective1 = usings[0];

        for (int i = 1; i < count; i++, usingDirective1 = usings[i - 1])
        {
            if (usingDirective1.StaticKeyword.IsKind(SyntaxKind.StaticKeyword))
                return;

            if (usingDirective1.Alias is not null)
                return;

            UsingDirectiveSyntax usingDirective2 = usings[i];

            if (usingDirective2.StaticKeyword.IsKind(SyntaxKind.StaticKeyword))
                return;

            if (usingDirective2.Alias is not null)
                return;

            IdentifierNameSyntax rootNamespace1 = usingDirective1.GetRootNamespace();

            if (rootNamespace1 is null)
                continue;

            IdentifierNameSyntax rootNamespace2 = usingDirective2.GetRootNamespace();

            if (rootNamespace2 is null)
                continue;

            TriviaBlockAnalysis analysis = TriviaBlockAnalysis.FromBetween(usingDirective1, usingDirective2);

            if (!analysis.Success)
                return;

            if (string.Equals(rootNamespace1.Identifier.ValueText, rootNamespace2.Identifier.ValueText, StringComparison.Ordinal))
            {
                if (analysis.Kind == TriviaBlockKind.BlankLine)
                {
                    if (DiagnosticRules.RemoveBlankLineBetweenUsingDirectivesWithSameRootNamespace.IsEffective(context))
                    {
                        DiagnosticHelpers.ReportDiagnostic(
                            context,
                            DiagnosticRules.RemoveBlankLineBetweenUsingDirectivesWithSameRootNamespace,
                            analysis.GetLocation());
                    }

                    if (DiagnosticRules.BlankLineBetweenUsingDirectives.IsEffective(context)
                        && context.GetBlankLineBetweenUsingDirectives() == UsingDirectiveBlankLineStyle.Never)
                    {
                        DiagnosticHelpers.ReportDiagnostic(
                            context,
                            DiagnosticRules.BlankLineBetweenUsingDirectives,
                            analysis.GetLocation(),
                            "Remove");
                    }
                }
            }
            else if (DiagnosticRules.BlankLineBetweenUsingDirectives.IsEffective(context))
            {
                UsingDirectiveBlankLineStyle style = context.GetBlankLineBetweenUsingDirectives();

                if (analysis.Kind == TriviaBlockKind.BlankLine)
                {
                    if (style == UsingDirectiveBlankLineStyle.Never)
                    {
                        DiagnosticHelpers.ReportDiagnostic(
                            context,
                            DiagnosticRules.BlankLineBetweenUsingDirectives,
                            analysis.GetLocation(),
                            "Remove");
                    }
                }
                else if (style == UsingDirectiveBlankLineStyle.SeparateGroups)
                {
                    DiagnosticHelpers.ReportDiagnostic(
                        context,
                        DiagnosticRules.BlankLineBetweenUsingDirectives,
                        analysis.GetLocation(),
                        "Add");
                }
            }
        }
    }
}
