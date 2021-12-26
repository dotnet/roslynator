// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class UnncessaryNullForgivingOperatorAnalyzer : BaseDiagnosticAnalyzer
    {
        private static readonly MetadataName System_Diagnostics_CodeAnalysis_MaybeNullWhenAttribute = MetadataName.Parse("System.Diagnostics.CodeAnalysis.MaybeNullWhenAttribute");
        private static readonly MetadataName System_Diagnostics_CodeAnalysis_NotNullIfNotNullAttribute = MetadataName.Parse("System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute");
        private static readonly MetadataName System_Diagnostics_CodeAnalysis_NotNullWhenAttribute = MetadataName.Parse("System.Diagnostics.CodeAnalysis.NotNullWhenAttribute");

        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                    Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.UnnecessaryNullForgivingOperator);

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeSuppressNullableWarningExpression(f), SyntaxKind.SuppressNullableWarningExpression);
        }

        private static void AnalyzeSuppressNullableWarningExpression(SyntaxNodeAnalysisContext context)
        {
            var suppressExpression = (PostfixUnaryExpressionSyntax)context.Node;

            SyntaxNode node = suppressExpression.WalkUpParentheses().Parent;

            if (node is ArgumentSyntax argument)
            {
                IParameterSymbol parameterSymbol = context.SemanticModel.DetermineParameter(
                    argument,
                    cancellationToken: context.CancellationToken);

                if (parameterSymbol?.Type.IsErrorType() == false
                    && parameterSymbol.Type.IsReferenceType
                    && parameterSymbol.Type.NullableAnnotation == NullableAnnotation.Annotated)
                {
                    foreach (AttributeData attribute in parameterSymbol.GetAttributes())
                    {
                        INamedTypeSymbol attributeClass = attribute.AttributeClass;

                        if (attributeClass.HasMetadataName(System_Diagnostics_CodeAnalysis_MaybeNullWhenAttribute)
                            || attributeClass.HasMetadataName(System_Diagnostics_CodeAnalysis_NotNullIfNotNullAttribute)
                            || attributeClass.HasMetadataName(System_Diagnostics_CodeAnalysis_NotNullWhenAttribute))
                        {
                            return;
                        }
                    }

                    context.ReportDiagnostic(DiagnosticRules.UnnecessaryNullForgivingOperator, suppressExpression.OperatorToken);
                }
            }
            else if (node.IsKind(SyntaxKind.EqualsValueClause))
            {
                if (suppressExpression.Operand.WalkDownParentheses().IsKind(
                    SyntaxKind.NullLiteralExpression,
                    SyntaxKind.DefaultLiteralExpression,
                    SyntaxKind.DefaultExpression))
                {
                    SyntaxNode parent = node.Parent;

                    if (parent.IsKind(SyntaxKind.PropertyDeclaration))
                    {
                        var property = (PropertyDeclarationSyntax)node.Parent;

                        if (IsNullableReferenceType(context, property.Type))
                            context.ReportDiagnostic(DiagnosticRules.UnnecessaryNullForgivingOperator, node);
                    }
                    else if (parent.IsKind(SyntaxKind.VariableDeclarator))
                    {
                        SyntaxDebug.Assert(
                            parent.IsParentKind(SyntaxKind.VariableDeclaration)
                                && parent.Parent.IsParentKind(SyntaxKind.FieldDeclaration, SyntaxKind.LocalDeclarationStatement),
                            parent);

                        if (parent.IsParentKind(SyntaxKind.VariableDeclaration)
                            && parent.Parent.IsParentKind(SyntaxKind.FieldDeclaration, SyntaxKind.LocalDeclarationStatement))
                        {
                            var variableDeclaration = (VariableDeclarationSyntax)parent.Parent;

                            if (IsNullableReferenceType(context, variableDeclaration.Type))
                            {
                                if (parent.Parent.IsParentKind(SyntaxKind.FieldDeclaration))
                                {
                                    context.ReportDiagnostic(DiagnosticRules.UnnecessaryNullForgivingOperator, node);
                                }
                                else
                                {
                                    context.ReportDiagnostic(DiagnosticRules.UnnecessaryNullForgivingOperator, suppressExpression.OperatorToken);
                                }
                            }
                        }
                    }
                }
            }

            static bool IsNullableReferenceType(SyntaxNodeAnalysisContext context, TypeSyntax type)
            {
                if (!type.IsKind(SyntaxKind.NullableType))
                    return false;

                ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(type, context.CancellationToken);

                return !typeSymbol.IsErrorType()
                    && typeSymbol.IsReferenceType;
            }
        }
    }
}
