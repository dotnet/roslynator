// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.MarkLocalVariableAsConst
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class LocalDeclarationStatementAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                    Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.MarkLocalVariableAsConst);

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeLocalDeclarationStatement(f), SyntaxKind.LocalDeclarationStatement);
        }

        private static void AnalyzeLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.ContainsDiagnostics)
                return;

            var localDeclaration = (LocalDeclarationStatementSyntax)context.Node;

            if (localDeclaration.IsConst)
                return;

            StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(localDeclaration);

            if (!statementsInfo.Success)
                return;

            SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

            if (statements.Count <= 1)
                return;

            int index = statements.IndexOf(localDeclaration);

            if (index == statements.Count - 1)
                return;

            LocalDeclarationStatementInfo localInfo = SyntaxInfo.LocalDeclarationStatementInfo(localDeclaration);

            if (!localInfo.Success)
                return;

            ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(localInfo.Type, context.CancellationToken);

            if (typeSymbol?.SupportsConstantValue() != true)
                return;

            if (localInfo.Type.IsVar && !typeSymbol.SupportsExplicitDeclaration())
                return;

            foreach (VariableDeclaratorSyntax declarator in localInfo.Variables)
            {
                ExpressionSyntax value = declarator.Initializer?.Value?.WalkDownParentheses();

                if (value?.IsMissing != false)
                    return;

                if (!HasConstantValue(value, typeSymbol, context.SemanticModel, context.CancellationToken))
                    return;
            }

            if (!CanBeMarkedAsConst(localInfo.Variables, statements, index + 1))
                return;

            if (((CSharpParseOptions)context.Node.SyntaxTree.Options).LanguageVersion <= LanguageVersion.CSharp9
                && ContainsInterpolatedString(localInfo.Variables))
            {
                return;
            }

            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.MarkLocalVariableAsConst, localInfo.Type);
        }

        private static bool CanBeMarkedAsConst(
            SeparatedSyntaxList<VariableDeclaratorSyntax> variables,
            SyntaxList<StatementSyntax> statements,
            int startIndex)
        {
            MarkLocalVariableAsConstWalker walker = MarkLocalVariableAsConstWalker.GetInstance();

            foreach (VariableDeclaratorSyntax variable in variables)
                walker.Identifiers.Add(variable.Identifier.ValueText);

            var result = true;

            for (int i = startIndex; i < statements.Count; i++)
            {
                walker.Visit(statements[i]);

                if (walker.Result)
                {
                    result = false;
                    break;
                }
            }

            MarkLocalVariableAsConstWalker.Free(walker);

            return result;
        }

        private static bool HasConstantValue(
            ExpressionSyntax expression,
            ITypeSymbol typeSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            switch (typeSymbol.SpecialType)
            {
                case SpecialType.System_Boolean:
                    {
                        if (CSharpFacts.IsBooleanLiteralExpression(expression.Kind()))
                            return true;

                        break;
                    }
                case SpecialType.System_Char:
                    {
                        if (expression.IsKind(SyntaxKind.CharacterLiteralExpression))
                            return true;

                        break;
                    }
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Decimal:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                    {
                        if (expression.IsKind(SyntaxKind.NumericLiteralExpression))
                            return true;

                        break;
                    }
                case SpecialType.System_String:
                    {
                        switch (expression.Kind())
                        {
                            case SyntaxKind.StringLiteralExpression:
                                return true;
                            case SyntaxKind.InterpolatedStringExpression:
                                return false;
                        }

                        break;
                    }
            }

            return semanticModel.HasConstantValue(expression, cancellationToken);
        }

        private static bool ContainsInterpolatedString(SeparatedSyntaxList<VariableDeclaratorSyntax> variables)
        {
            foreach (VariableDeclaratorSyntax declarator in variables)
            {
                ExpressionSyntax value = declarator.Initializer.Value.WalkDownParentheses();

                if (value is not LiteralExpressionSyntax)
                {
                    foreach (SyntaxNode node in value.DescendantNodes())
                    {
                        if (node.IsKind(SyntaxKind.InterpolatedStringExpression))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
