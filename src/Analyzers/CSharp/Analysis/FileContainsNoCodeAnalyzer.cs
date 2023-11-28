// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
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

        if (compilationUnit.Span == token.Span
            && !token.HasTrailingTrivia
            && !token.LeadingTrivia.Any(trivia =>
            {
                switch (trivia.Kind())
                {
                    case SyntaxKind.IfDirectiveTrivia:
                    case SyntaxKind.ElseDirectiveTrivia:
                    case SyntaxKind.ElifDirectiveTrivia:
                    case SyntaxKind.EndIfDirectiveTrivia:
                        {
                            return true;
                        }
                    case SyntaxKind.PragmaWarningDirectiveTrivia:
                        {
                            var pragma = (PragmaWarningDirectiveTriviaSyntax)trivia.GetStructure();

                            foreach (ExpressionSyntax errorCode in pragma.ErrorCodes)
                            {
                                if (errorCode is IdentifierNameSyntax identifierName
                                    && string.Equals(identifierName.Identifier.ValueText, DiagnosticRules.FileContainsNoCode.Id, StringComparison.OrdinalIgnoreCase))
                                {
                                    return true;
                                }
                            }

                            break;
                        }
                }

                return false;
            }))
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
}
