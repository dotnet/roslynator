// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RemoveRedundantFieldInitializationAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemoveRedundantFieldInitialization); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeFieldDeclaration, SyntaxKind.FieldDeclaration);
        }

        internal static void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var fieldDeclaration = (FieldDeclarationSyntax)context.Node;

            if (fieldDeclaration.ContainsDiagnostics)
                return;

            if (fieldDeclaration.Modifiers.Contains(SyntaxKind.ConstKeyword))
                return;

            VariableDeclarationSyntax declaration = fieldDeclaration.Declaration;

            if (declaration == null)
                return;

            foreach (VariableDeclaratorSyntax declarator in declaration.Variables)
            {
                EqualsValueClauseSyntax initializer = declarator.Initializer;
                if (initializer?.ContainsDirectives == false)
                {
                    ExpressionSyntax value = initializer.Value;
                    if (value != null)
                    {
                        SemanticModel semanticModel = context.SemanticModel;
                        CancellationToken cancellationToken = context.CancellationToken;

                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(declaration.Type, cancellationToken);

                        if (typeSymbol?.IsErrorType() == false
                            && semanticModel.IsDefaultValue(typeSymbol, value, cancellationToken))
                        {
                            context.ReportDiagnostic(
                                DiagnosticDescriptors.RemoveRedundantFieldInitialization,
                                initializer);
                        }
                    }
                }
            }
        }
    }
}
