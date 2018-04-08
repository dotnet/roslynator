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
    public class RemoveEmptyDestructorAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemoveEmptyDestructor); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeDestructorDeclaration, SyntaxKind.DestructorDeclaration);
        }

        public static void AnalyzeDestructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var destructor = (DestructorDeclarationSyntax)context.Node;

            if (destructor.ContainsDiagnostics)
                return;

            if (destructor.ContainsDirectives)
                return;

            if (destructor.AttributeLists.Any())
                return;

            if (destructor.Body?.Statements.Count != 0)
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.RemoveEmptyDestructor, destructor);
        }
    }
}
