// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MergeLocalDeclarationWithAssignmentAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.MergeLocalDeclarationWithAssignment,
                    DiagnosticDescriptors.MergeLocalDeclarationWithAssignmentFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeLocalDeclarationStatement, SyntaxKind.LocalDeclarationStatement);
        }

        public static void AnalyzeLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
        {
            var localDeclaration = (LocalDeclarationStatementSyntax)context.Node;

            if (localDeclaration.ContainsDiagnostics)
                return;

            if (localDeclaration.SpanOrTrailingTriviaContainsDirectives())
                return;

            if (localDeclaration.IsConst)
                return;

            SingleLocalDeclarationStatementInfo localInfo = SyntaxInfo.SingleLocalDeclarationStatementInfo(localDeclaration);

            if (!localInfo.Success)
                return;

            SimpleAssignmentStatementInfo assignmentInfo = SyntaxInfo.SimpleAssignmentStatementInfo(localDeclaration.NextStatement());

            if (!assignmentInfo.Success)
                return;

            if (assignmentInfo.Statement.ContainsDiagnostics)
                return;

            if (assignmentInfo.Statement.SpanOrLeadingTriviaContainsDirectives())
                return;

            if (!(assignmentInfo.Left is IdentifierNameSyntax identifierName))
                return;

            string name = identifierName.Identifier.ValueText;

            if (!string.Equals(localInfo.IdentifierText, name, StringComparison.Ordinal))
                return;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            var localSymbol = semanticModel.GetSymbol(identifierName, cancellationToken) as ILocalSymbol;

            if (localSymbol == null)
                return;

            if (!localSymbol.Equals(semanticModel.GetDeclaredSymbol(localInfo.Declarator, cancellationToken)))
                return;

            ExpressionSyntax value = localInfo.Value;

            if (value != null)
            {
                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(localInfo.Type, cancellationToken);

                if (typeSymbol == null)
                    return;

                if (!semanticModel.IsDefaultValue(typeSymbol, value, cancellationToken))
                    return;

                if (IsReferenced(localSymbol, assignmentInfo.Right, semanticModel, cancellationToken))
                    return;
            }

            context.ReportDiagnostic(DiagnosticDescriptors.MergeLocalDeclarationWithAssignment, localInfo.Identifier);

            if (value != null)
            {
                context.ReportNode(DiagnosticDescriptors.MergeLocalDeclarationWithAssignmentFadeOut, localInfo.Initializer);
                context.ReportToken(DiagnosticDescriptors.MergeLocalDeclarationWithAssignmentFadeOut, assignmentInfo.OperatorToken);
            }

            context.ReportToken(DiagnosticDescriptors.MergeLocalDeclarationWithAssignmentFadeOut, localDeclaration.SemicolonToken);
            context.ReportNode(DiagnosticDescriptors.MergeLocalDeclarationWithAssignmentFadeOut, assignmentInfo.Left);
        }

        private static bool IsReferenced(
            ILocalSymbol localSymbol,
            SyntaxNode node,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            foreach (SyntaxNode descendantOrSelf in node.DescendantNodesAndSelf())
            {
                if (descendantOrSelf.IsKind(SyntaxKind.IdentifierName)
                    && semanticModel
                        .GetSymbol((IdentifierNameSyntax)descendantOrSelf, cancellationToken)?
                        .Equals(localSymbol) == true)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
