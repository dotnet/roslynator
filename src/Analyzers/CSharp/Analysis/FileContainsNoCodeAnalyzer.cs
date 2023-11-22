// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FileContainsNoCodeAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.FileContainsNoCode);

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

        SyntaxToken token = compilationUnit.EndOfFileToken;

        if (!compilationUnit.AttributeLists.Any()
            && !compilationUnit.Externs.Any())
        {
            if (!compilationUnit.Members.Any()
                && compilationUnit.Span == token.Span
                && !token.HasTrailingTrivia
                && token.LeadingTrivia.All(f => !f.IsDirective))
            {
                ReportDiagnostic(context, compilationUnit);
            }
            else if ((!compilationUnit.Members.Any()
                || (compilationUnit.Members.Count == 1
                    && compilationUnit.Members[0] is FileScopedNamespaceDeclarationSyntax namespaceDeclaration
                    && !namespaceDeclaration.Members.Any()))
                && !compilationUnit.DescendantTrivia().Any(f =>
                {
                    switch (f.Kind())
                    {
                        case SyntaxKind.IfDirectiveTrivia:
                        case SyntaxKind.ElseDirectiveTrivia:
                        case SyntaxKind.ElifDirectiveTrivia:
                        case SyntaxKind.EndIfDirectiveTrivia:
                            return true;
                        default:
                            return false;
                    }
                }))
            {
                ReportDiagnostic(context, compilationUnit);
            }
        }
    }

    private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, CompilationUnitSyntax compilationUnit)
    {
        SyntaxTree syntaxTree = compilationUnit.SyntaxTree;

        if (!GeneratedCodeUtility.IsGeneratedCodeFile(syntaxTree.FilePath))
        {
            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticRules.FileContainsNoCode,
                Location.Create(syntaxTree, default(TextSpan)));
        }
    }
}
