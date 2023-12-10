// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DeclareExplicitOrImplicitTypeAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.DeclareExplicitOrImplicitType);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeVariableDeclaration(f), SyntaxKind.VariableDeclaration);
        context.RegisterSyntaxNodeAction(f => AnalyzeDeclarationExpression(f), SyntaxKind.DeclarationExpression);
        context.RegisterSyntaxNodeAction(f => AnalyzeTupleExpression(f), SyntaxKind.TupleExpression);
        context.RegisterSyntaxNodeAction(f => AnalyzeForEachStatement(f), SyntaxKind.ForEachStatement);
    }

    private static void AnalyzeVariableDeclaration(SyntaxNodeAnalysisContext context)
    {
        var variableDeclaration = (VariableDeclarationSyntax)context.Node;

        TypeStyle style = context.GetTypeStyle();

        if (style == TypeStyle.Implicit)
        {
            if (CSharpTypeAnalysis.IsExplicitThatCanBeImplicit(variableDeclaration, context.SemanticModel, TypeAppearance.NotObvious, context.CancellationToken))
                ReportExplicitToImplicit(context, variableDeclaration.Type);
        }
        else if (style == TypeStyle.Explicit)
        {
            if (CSharpTypeAnalysis.IsImplicitThatCanBeExplicit(variableDeclaration, context.SemanticModel, TypeAppearance.Obvious, context.CancellationToken))
                ReportImplicitToExplicit(context, variableDeclaration.Type);
        }
        else if (style == TypeStyle.ImplicitWhenTypeIsObvious)
        {
            TypeAnalysis typeAnalysis = CSharpTypeAnalysis.AnalyzeType(variableDeclaration, context.SemanticModel, context.CancellationToken);

            if (typeAnalysis.IsExplicit)
            {
                if (typeAnalysis.IsTypeObvious
                    && typeAnalysis.SupportsImplicit)
                {
                    ReportExplicitToImplicit(context, variableDeclaration.Type);
                }
            }
            else if (typeAnalysis.IsImplicit)
            {
                if (!typeAnalysis.IsTypeObvious
                    && typeAnalysis.SupportsExplicit)
                {
                    ReportImplicitToExplicit(context, variableDeclaration.Type);
                }
            }
        }
    }

    private static void AnalyzeDeclarationExpression(SyntaxNodeAnalysisContext context)
    {
        var declarationExpression = (DeclarationExpressionSyntax)context.Node;

        TypeStyle style = context.GetTypeStyle();

        if (style == TypeStyle.Implicit)
        {
            if (!IsPartOfTupleExpression(declarationExpression)
                && CSharpTypeAnalysis.IsExplicitThatCanBeImplicit(declarationExpression, context.SemanticModel, context.CancellationToken))
            {
                ReportExplicitToImplicit(context, declarationExpression.Type);
            }
        }
        else if (style == TypeStyle.Explicit)
        {
            if (CSharpTypeAnalysis.IsImplicitThatCanBeExplicit(declarationExpression, context.SemanticModel, context.CancellationToken))
                ReportImplicitToExplicit(context, declarationExpression.Type);
        }
        else if (style == TypeStyle.ImplicitWhenTypeIsObvious)
        {
            if (declarationExpression.Parent is ForEachVariableStatementSyntax forEachStatement)
            {
                if (CSharpTypeAnalysis.IsImplicitThatCanBeExplicit(forEachStatement, context.SemanticModel))
                    ReportExplicitToImplicit(context, declarationExpression.Type);
            }
            else
            {
                if (!IsObvious(declarationExpression)
                    && !IsObviousTupleExpression(declarationExpression)
                    && CSharpTypeAnalysis.IsImplicitThatCanBeExplicit(declarationExpression, context.SemanticModel, TypeAppearance.Obvious, context.CancellationToken))
                {
                    ReportImplicitToExplicit(context, declarationExpression.Type);
                }

                static bool IsObviousTupleExpression(DeclarationExpressionSyntax declarationExpression)
                {
                    return IsPartOfTupleExpression(declarationExpression)
                        && declarationExpression.Parent.Parent.Parent is AssignmentExpressionSyntax assignmentExpression
                        && assignmentExpression.Right.IsKind(SyntaxKind.DefaultExpression);
                }

                static bool IsObvious(DeclarationExpressionSyntax declarationExpression)
                {
                    return declarationExpression.Parent is AssignmentExpressionSyntax assignmentExpression
                        && assignmentExpression.Right.IsKind(SyntaxKind.DefaultExpression);
                }
            }
        }
    }

    private static void AnalyzeTupleExpression(SyntaxNodeAnalysisContext context)
    {
        var tupleExpression = (TupleExpressionSyntax)context.Node;

        TypeStyle style = context.GetTypeStyle();

        if (style == TypeStyle.Implicit)
        {
            if (CSharpTypeAnalysis.IsExplicitThatCanBeImplicit(tupleExpression, context.SemanticModel, context.CancellationToken))
                ReportExplicitToImplicit(context, tupleExpression);
        }
        else if (style == TypeStyle.ImplicitWhenTypeIsObvious)
        {
            if (tupleExpression.Parent is AssignmentExpressionSyntax assignmentExpression
                && assignmentExpression.Right.IsKind(SyntaxKind.DefaultExpression)
                && CSharpTypeAnalysis.IsExplicitThatCanBeImplicit(tupleExpression, context.SemanticModel, context.CancellationToken))
            {
                ReportExplicitToImplicit(context, tupleExpression);
            }
        }
    }

    private static void AnalyzeForEachStatement(SyntaxNodeAnalysisContext context)
    {
        var forEachStatement = (ForEachStatementSyntax)context.Node;

        TypeStyle style = context.GetTypeStyle();

        if (style == TypeStyle.Implicit)
        {
            TypeAnalysis analysis = CSharpTypeAnalysis.AnalyzeType(forEachStatement, context.SemanticModel);

            if (analysis.IsExplicit
                && analysis.SupportsImplicit)
            {
                ReportExplicitToImplicit(context, forEachStatement.Type);
            }
        }
        else if (style == TypeStyle.Explicit
            || style == TypeStyle.ImplicitWhenTypeIsObvious)
        {
            if (CSharpTypeAnalysis.IsImplicitThatCanBeExplicit(forEachStatement, context.SemanticModel))
                ReportImplicitToExplicit(context, forEachStatement.Type);
        }
    }

    private static void ReportExplicitToImplicit(SyntaxNodeAnalysisContext context, SyntaxNode node)
    {
        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.DeclareExplicitOrImplicitType,
            node.GetLocation(),
            "implicit");
    }

    private static void ReportImplicitToExplicit(SyntaxNodeAnalysisContext context, SyntaxNode node)
    {
        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.DeclareExplicitOrImplicitType,
            node.GetLocation(),
            "explicit");
    }

    private static bool IsPartOfTupleExpression(DeclarationExpressionSyntax declarationExpression)
    {
        return declarationExpression.IsParentKind(SyntaxKind.Argument)
            && declarationExpression.Parent.IsParentKind(SyntaxKind.TupleExpression);
    }
}
