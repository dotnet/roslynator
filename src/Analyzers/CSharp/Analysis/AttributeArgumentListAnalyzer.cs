// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AttributeArgumentListAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemoveArgumentListFromAttribute); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeAttributeArgumentList, SyntaxKind.AttributeArgumentList);
        }

        private static void AnalyzeAttributeArgumentList(SyntaxNodeAnalysisContext context)
        {
            var attributeArgumentList = (AttributeArgumentListSyntax)context.Node;

            if (!attributeArgumentList.Arguments.Any())
                context.ReportDiagnostic(DiagnosticDescriptors.RemoveArgumentListFromAttribute, attributeArgumentList);
        }
    }
}
