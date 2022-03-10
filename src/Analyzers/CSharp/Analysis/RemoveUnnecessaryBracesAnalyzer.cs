// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
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

            context.RegisterSyntaxNodeAction(f => AnalyzeRecordDeclaration(f), SyntaxKind.RecordDeclaration, SyntaxKind.RecordStructDeclaration);
        }

        private static void AnalyzeRecordDeclaration(SyntaxNodeAnalysisContext context)
        {
            var recordDeclaration = (RecordDeclarationSyntax)context.Node;

            if (!recordDeclaration.Members.Any()
                && recordDeclaration.ParameterList != null)
            {
                SyntaxToken openBrace = recordDeclaration.OpenBraceToken;

                if (!openBrace.IsKind(SyntaxKind.None))
                {
                    SyntaxToken closeBrace = recordDeclaration.CloseBraceToken;

                    if (!closeBrace.IsKind(SyntaxKind.None)
                        && openBrace.TrailingTrivia.IsEmptyOrWhitespace()
                        && closeBrace.LeadingTrivia.IsEmptyOrWhitespace()
                        && recordDeclaration.ParameterList?.CloseParenToken.TrailingTrivia.IsEmptyOrWhitespace() != false)
                    {
                        DiagnosticHelpers.ReportDiagnostic(
                            context,
                            DiagnosticRules.RemoveUnnecessaryBraces,
                            openBrace.GetLocation(),
                            additionalLocations: new Location[] { closeBrace.GetLocation() });
                    }
                }
            }
        }
    }
}
