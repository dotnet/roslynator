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
    public class RemoveRedundantAutoPropertyInitializationAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemoveRedundantAutoPropertyInitialization); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
        }

        public static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.ContainsDiagnostics)
                return;

            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

            EqualsValueClauseSyntax initializer = propertyDeclaration.Initializer;

            if (initializer == null)
                return;

            if (initializer.SpanOrLeadingTriviaContainsDirectives())
                return;

            ExpressionSyntax value = initializer.Value;

            if (value == null)
                return;

            AccessorListSyntax accessorList = propertyDeclaration.AccessorList;

            if (accessorList == null)
                return;

            if (accessorList.Accessors.Any(f => !f.IsAutoImplemented()))
                return;

            ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(propertyDeclaration.Type, context.CancellationToken);

            if (typeSymbol?.IsErrorType() != false)
                return;

            if (!context.SemanticModel.IsDefaultValue(typeSymbol, value, context.CancellationToken))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.RemoveRedundantAutoPropertyInitialization, value);
        }
    }
}
