// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class RemoveRedundantFieldInitializationAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                    Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.RemoveRedundantFieldInitialization);

                return _supportedDiagnostics;
            }
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

            if (fieldDeclaration.IsParentKind(SyntaxKind.StructDeclaration, SyntaxKind.RecordStructDeclaration))
            {
                var structSymbol = context.SemanticModel.GetDeclaredSymbol(fieldDeclaration.Parent, context.CancellationToken) as INamedTypeSymbol;

                if (structSymbol?.InstanceConstructors.Length > 1)
                    return;
            }

            VariableDeclarationSyntax declaration = fieldDeclaration.Declaration;

            if (declaration == null)
                return;

            foreach (VariableDeclaratorSyntax declarator in declaration.Variables)
            {
                EqualsValueClauseSyntax initializer = declarator.Initializer;
                if (initializer?.ContainsDirectives == false)
                {
                    ExpressionSyntax value = initializer.Value?.WalkDownParentheses();

                    if (value is CastExpressionSyntax castExpression)
                        value = castExpression.Expression.WalkDownParentheses();

                    if (CanBeConstantValue(value))
                    {
                        SemanticModel semanticModel = context.SemanticModel;
                        CancellationToken cancellationToken = context.CancellationToken;

                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(declaration.Type, cancellationToken);

                        if (typeSymbol != null)
                        {
                            if (CSharpFacts.IsNumericType(typeSymbol.SpecialType)
                                && value.IsNumericLiteralExpression("0"))
                            {
                                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.RemoveRedundantFieldInitialization, initializer);
                            }
                            else if (semanticModel.IsDefaultValue(typeSymbol, value, cancellationToken))
                            {
                                DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.RemoveRedundantFieldInitialization, initializer);
                            }
                        }
                    }
                }
            }
        }

        private static bool CanBeConstantValue(ExpressionSyntax value)
        {
            switch (value.Kind())
            {
                case SyntaxKind.NullLiteralExpression:
                case SyntaxKind.NumericLiteralExpression:
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                case SyntaxKind.CharacterLiteralExpression:
                case SyntaxKind.DefaultLiteralExpression:
                case SyntaxKind.DefaultExpression:
                    return true;
                default:
                    return false;
            }
        }
    }
}
