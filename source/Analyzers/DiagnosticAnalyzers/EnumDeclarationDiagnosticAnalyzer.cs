// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EnumDeclarationDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.FormatEachEnumMemberOnSeparateLine,
                    DiagnosticDescriptors.RemoveEnumDefaultUnderlyingType);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeSyntaxNode(f), SyntaxKind.EnumDeclaration);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var enumDeclaration = (EnumDeclarationSyntax)context.Node;

            AnalyzeStatements(context, enumDeclaration);

            BaseTypeSyntax baseType = GetRedundantBaseType(context, enumDeclaration);

            if (baseType != null)
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.RemoveEnumDefaultUnderlyingType,
                    baseType.GetLocation());
            }
        }

        private static void AnalyzeStatements(SyntaxNodeAnalysisContext context, EnumDeclarationSyntax enumDeclaration)
        {
            SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;

            if (members.Count <= 1)
                return;

            int previousIndex = members[0].GetSpanStartLine();

            for (int i = 1; i < members.Count; i++)
            {
                if (members[i].GetSpanStartLine() == previousIndex)
                {
                    TextSpan span = TextSpan.FromBounds(
                        enumDeclaration.Members.First().Span.Start,
                        enumDeclaration.Members.Last().Span.End);

                    context.ReportDiagnostic(
                        DiagnosticDescriptors.FormatEachEnumMemberOnSeparateLine,
                        Location.Create(enumDeclaration.SyntaxTree, span));

                    return;
                }

                previousIndex = members[i].GetSpanEndLine();
            }
        }

        private static BaseTypeSyntax GetRedundantBaseType(SyntaxNodeAnalysisContext context, EnumDeclarationSyntax enumDeclaration)
        {
            if (enumDeclaration.BaseList != null)
            {
                foreach (BaseTypeSyntax baseType in enumDeclaration.BaseList.Types)
                {
                    if (baseType.IsKind(SyntaxKind.SimpleBaseType))
                    {
                        var simpleBaseType = (SimpleBaseTypeSyntax)baseType;

                        if (simpleBaseType.Type?.IsKind(SyntaxKind.PredefinedType) == true)
                        {
                            var symbol = context.SemanticModel.GetSymbolInfo(simpleBaseType.Type, context.CancellationToken).Symbol as INamedTypeSymbol;

                            if (symbol?.IsInt32() == true)
                                return baseType;
                        }
                    }
                }
            }

            return null;
        }
    }
}
