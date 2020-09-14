// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeFieldDeclaration(f), SyntaxKind.FieldDeclaration);
        }

        private static void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
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
                    ExpressionSyntax value = initializer.Value?.WalkDownParentheses();
                    if (value?.IsKind(SyntaxKind.SuppressNullableWarningExpression) == false)
                    {
                        SemanticModel semanticModel = context.SemanticModel;
                        CancellationToken cancellationToken = context.CancellationToken;

                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(declaration.Type, cancellationToken);

                        if (typeSymbol != null)
                        {
                            if (CSharpFacts.IsNumericType(typeSymbol.SpecialType)
                                && value.IsNumericLiteralExpression("0"))
                            {
                                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.RemoveRedundantFieldInitialization, initializer);
                            }
                            else if (semanticModel.IsDefaultValue(typeSymbol, value, cancellationToken))
                            {
                                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.RemoveRedundantFieldInitialization, initializer);
                            }
                        }
                    }
                }
            }
        }
    }
}
