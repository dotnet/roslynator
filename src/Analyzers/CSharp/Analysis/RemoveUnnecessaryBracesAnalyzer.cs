// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RemoveUnnecessaryBracesAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.RemoveUnnecessaryBraces);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(
            c => AnalyzeRecordDeclaration(c),
#if ROSLYN_4_0
            SyntaxKind.RecordStructDeclaration,
#endif
            SyntaxKind.RecordDeclaration);

        context.RegisterCompilationStartAction(startContext =>
        {
            if ((int)((CSharpCompilation)startContext.Compilation).LanguageVersion >= 1200)
            {
                startContext.RegisterSyntaxNodeAction(
                    c => AnalyzeTypeDeclaration(c),
                    SyntaxKind.ClassDeclaration,
                    SyntaxKind.StructDeclaration,
                    SyntaxKind.InterfaceDeclaration);
            }
        });
    }

    private static void AnalyzeRecordDeclaration(SyntaxNodeAnalysisContext context)
    {
        var recordDeclaration = (RecordDeclarationSyntax)context.Node;

        if (!recordDeclaration.Members.Any()
            && recordDeclaration.ParameterList is not null)
        {
            Analyze(context, recordDeclaration);
        }
    }

    private static void Analyze(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDeclaration)
    {
        SyntaxToken openBrace = typeDeclaration.OpenBraceToken;

        if (!openBrace.IsKind(SyntaxKind.None))
        {
            SyntaxToken closeBrace = typeDeclaration.CloseBraceToken;

            if (!closeBrace.IsKind(SyntaxKind.None)
                && openBrace.TrailingTrivia.IsEmptyOrWhitespace()
                && closeBrace.LeadingTrivia.IsEmptyOrWhitespace()
#if ROSLYN_4_7
                && typeDeclaration.ParameterList?.CloseParenToken.TrailingTrivia.IsEmptyOrWhitespace() != false
#endif
                )
            {
                DiagnosticHelpers.ReportDiagnostic(
                    context,
                    DiagnosticRules.RemoveUnnecessaryBraces,
                    openBrace.GetLocation(),
                    additionalLocations: new Location[] { closeBrace.GetLocation() });
            }
        }
    }

    private static void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context)
    {
        var typeDeclaration = (TypeDeclarationSyntax)context.Node;

        if (!typeDeclaration.Members.Any()
#if ROSLYN_4_7
            && typeDeclaration.ParameterList is null
#endif
            )
        {
            Analyze(context, typeDeclaration);
        }
    }
}
