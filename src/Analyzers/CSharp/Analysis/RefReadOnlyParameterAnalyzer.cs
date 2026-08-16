// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.SyntaxWalkers;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RefReadOnlyParameterAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
            {
                Immutable.InterlockedInitialize(
                    ref _supportedDiagnostics,
                    DiagnosticRules.MakeParameterRefReadOnly,
                    DiagnosticRules.DoNotPassNonReadOnlyStructByReadOnlyReference);
            }

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterCompilationStartAction(startContext =>
        {
            if (((CSharpCompilation)startContext.Compilation).LanguageVersion <= LanguageVersion.CSharp7_1)
                return;

            //TODO: AnalyzeIndexerDeclaration
            startContext.RegisterSyntaxNodeAction(f => AnalyzeMethodDeclaration(f), SyntaxKind.MethodDeclaration);
            startContext.RegisterSyntaxNodeAction(f => AnalyzeConstructorDeclaration(f), SyntaxKind.ConstructorDeclaration);
            startContext.RegisterSyntaxNodeAction(f => AnalyzeOperatorDeclaration(f), SyntaxKind.OperatorDeclaration);
            startContext.RegisterSyntaxNodeAction(f => AnalyzeConversionOperatorDeclaration(f), SyntaxKind.ConversionOperatorDeclaration);
            startContext.RegisterSyntaxNodeAction(f => AnalyzeLocalFunction(f), SyntaxKind.LocalFunctionStatement);
        });
    }

    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;

        if (methodDeclaration.Modifiers.ContainsAny(SyntaxKind.AsyncKeyword, SyntaxKind.OverrideKeyword))
            return;

        Analyze(context, methodDeclaration, methodDeclaration.ParameterList, methodDeclaration.BodyOrExpressionBody());
    }

    private static void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
    {
        var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;

        Analyze(context, constructorDeclaration, constructorDeclaration.ParameterList, constructorDeclaration.BodyOrExpressionBody());
    }

    private static void AnalyzeOperatorDeclaration(SyntaxNodeAnalysisContext context)
    {
        var operatorDeclaration = (OperatorDeclarationSyntax)context.Node;

        Analyze(context, operatorDeclaration, operatorDeclaration.ParameterList, operatorDeclaration.BodyOrExpressionBody());
    }

    private static void AnalyzeConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
    {
        var operatorDeclaration = (ConversionOperatorDeclarationSyntax)context.Node;

        Analyze(context, operatorDeclaration, operatorDeclaration.ParameterList, operatorDeclaration.BodyOrExpressionBody());
    }

    private static void AnalyzeLocalFunction(SyntaxNodeAnalysisContext context)
    {
        var localFunction = (LocalFunctionStatementSyntax)context.Node;

        if (localFunction.Modifiers.Contains(SyntaxKind.AsyncKeyword))
            return;

        Analyze(context, localFunction, localFunction.ParameterList, localFunction.BodyOrExpressionBody());
    }

    private static void Analyze(
        SyntaxNodeAnalysisContext context,
        SyntaxNode declaration,
        ParameterListSyntax parameterList,
        CSharpSyntaxNode bodyOrExpressionBody)
    {
        if (parameterList is null)
            return;

        if (bodyOrExpressionBody is null)
            return;

        if (!parameterList.Parameters.Any())
            return;

        SemanticModel semanticModel = context.SemanticModel;
        CancellationToken cancellationToken = context.CancellationToken;

        var methodSymbol = (IMethodSymbol)semanticModel.GetDeclaredSymbol(declaration, cancellationToken);

        var walker = new RefReadOnlyParameterWalker(semanticModel, cancellationToken);

        var isFirstCandidate = true;

        foreach (IParameterSymbol parameter in methodSymbol.Parameters)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ITypeSymbol type = parameter.Type;

            if (type.Kind == SymbolKind.ErrorType)
                continue;

            if (CSharpFacts.IsSimpleType(type.SpecialType))
                continue;

            if (!type.IsReadOnlyStruct())
            {
                if (parameter.RefKind == RefKind.In
                    && type.TypeKind == TypeKind.Struct)
                {
                    var parameterSyntax = (ParameterSyntax)parameter.GetSyntax(cancellationToken);

                    Debug.Assert(parameterSyntax.Modifiers.Contains(SyntaxKind.InKeyword), "");

                    DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.DoNotPassNonReadOnlyStructByReadOnlyReference, parameterSyntax.Identifier);
                }

                continue;
            }

            if (type.IsRefLikeType)
                continue;

            if (parameter.RefKind != RefKind.None)
                continue;

            if (parameter.IsParams)
                continue;

            if (type.HasMetadataName(in MetadataNames.System_Threading_CancellationToken)
                && methodSymbol.ReturnType.IsWellKnownTaskType())
            {
                continue;
            }

            if (isFirstCandidate)
            {
                if (methodSymbol.ImplementsInterfaceMember(allInterfaces: true))
                    break;

                isFirstCandidate = false;
            }
            else if (walker.Parameters.ContainsKey(parameter.Name))
            {
                walker.Parameters.Clear();
                break;
            }

            walker.Parameters.Add(parameter.Name, parameter);
        }

        if (walker.Parameters.Count > 0)
        {
            if (bodyOrExpressionBody.IsKind(SyntaxKind.Block))
            {
                walker.VisitBlock((BlockSyntax)bodyOrExpressionBody);
            }
            else
            {
                walker.VisitArrowExpressionClause((ArrowExpressionClauseSyntax)bodyOrExpressionBody);
            }

            if (walker.Parameters.Count > 0)
            {
                DataFlowAnalysis analysis = (bodyOrExpressionBody.IsKind(SyntaxKind.Block))
                    ? semanticModel.AnalyzeDataFlow((BlockSyntax)bodyOrExpressionBody)
                    : semanticModel.AnalyzeDataFlow(((ArrowExpressionClauseSyntax)bodyOrExpressionBody).Expression);

                bool? isReferencedAsMethodGroup = null;

                foreach (KeyValuePair<string, IParameterSymbol> kvp in walker.Parameters)
                {
                    var isAssigned = false;

                    foreach (ISymbol assignedSymbol in analysis.AlwaysAssigned)
                    {
                        if (SymbolEqualityComparer.Default.Equals(assignedSymbol, kvp.Value))
                        {
                            isAssigned = true;
                            break;
                        }
                    }

                    if (isAssigned)
                        continue;

                    if (isReferencedAsMethodGroup ??= IsReferencedAsMethodGroup())
                        break;

                    if (kvp.Value.GetSyntaxOrDefault(cancellationToken) is ParameterSyntax parameter)
                    {
                        DiagnosticHelpers.ReportDiagnostic(
                            context,
                            DiagnosticRules.MakeParameterRefReadOnly,
                            parameter.Identifier);
                    }
                }
            }
        }

        bool IsReferencedAsMethodGroup()
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return MethodReferencedAsMethodGroupWalker.IsReferencedAsMethodGroup(declaration.Parent, methodSymbol, semanticModel, cancellationToken);
                case SyntaxKind.LocalFunctionStatement:
                    return MethodReferencedAsMethodGroupWalker.IsReferencedAsMethodGroup(declaration.FirstAncestor<MemberDeclarationSyntax>(), methodSymbol, semanticModel, cancellationToken);
                default:
                    return false;
            }
        }
    }

    private class RefReadOnlyParameterWalker : BaseCSharpSyntaxWalker
    {
        private int _localFunctionDepth;
        private int _anonymousFunctionDepth;

        public RefReadOnlyParameterWalker(SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            SemanticModel = semanticModel;
            CancellationToken = cancellationToken;
        }

        public Dictionary<string, IParameterSymbol> Parameters { get; } = [];

        public SemanticModel SemanticModel { get; }

        public CancellationToken CancellationToken { get; }

        protected override bool ShouldVisit => Parameters.Count > 0;

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            CancellationToken.ThrowIfCancellationRequested();

            string name = node.Identifier.ValueText;

            if (Parameters.TryGetValue(name, out IParameterSymbol parameterSymbol)
                && SymbolEqualityComparer.Default.Equals(parameterSymbol, SemanticModel.GetSymbol(node, CancellationToken)))
            {
                if (_localFunctionDepth > 0
                    || _anonymousFunctionDepth > 0
                    || node.IsInExpressionTree(SemanticModel, CancellationToken))
                {
                    Parameters.Remove(name);
                }
            }

            base.VisitIdentifierName(node);
        }

        public override void VisitYieldStatement(YieldStatementSyntax node)
        {
            if (_localFunctionDepth == 0)
            {
                Parameters.Clear();
            }
            else
            {
                base.VisitYieldStatement(node);
            }
        }

        public override void VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
        {
            _anonymousFunctionDepth++;
            base.VisitAnonymousMethodExpression(node);
            _anonymousFunctionDepth--;
        }

        public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            _anonymousFunctionDepth++;
            base.VisitSimpleLambdaExpression(node);
            _anonymousFunctionDepth--;
        }

        public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
            _anonymousFunctionDepth++;
            base.VisitParenthesizedLambdaExpression(node);
            _anonymousFunctionDepth--;
        }

        public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
        {
            _localFunctionDepth++;
            base.VisitLocalFunctionStatement(node);
            _localFunctionDepth--;
        }
    }
}
