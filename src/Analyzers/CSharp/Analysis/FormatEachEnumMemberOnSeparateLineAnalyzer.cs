// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FormatEachEnumMemberOnSeparateLineAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.FormatEachEnumMemberOnSeparateLine); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeEnumDeclaration, SyntaxKind.EnumDeclaration);
        }

        public static void AnalyzeEnumDeclaration(SyntaxNodeAnalysisContext context)
        {
            var enumDeclaration = (EnumDeclarationSyntax)context.Node;

            SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;

            if (members.Count <= 1)
                return;

            int previousIndex = members[0].GetSpanStartLine();

            for (int i = 1; i < members.Count; i++)
            {
                if (members[i].GetSpanStartLine() == previousIndex)
                {
                    TextSpan span = TextSpan.FromBounds(
                        members.First().SpanStart,
                        members.Last().Span.End);

                    context.ReportDiagnostic(
                        DiagnosticDescriptors.FormatEachEnumMemberOnSeparateLine,
                        Location.Create(enumDeclaration.SyntaxTree, span));

                    return;
                }

                previousIndex = members[i].GetSpanEndLine();
            }
        }
    }
}
