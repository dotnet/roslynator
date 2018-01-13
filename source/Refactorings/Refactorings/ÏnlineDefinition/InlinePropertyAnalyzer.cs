// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings.InlineDefinition
{
    internal class InlinePropertyAnalyzer : InlineAnalyzer<IdentifierNameSyntax, PropertyDeclarationSyntax, IPropertySymbol>
    {
        public static InlinePropertyAnalyzer Instance { get; } = new InlinePropertyAnalyzer();

        protected override bool CheckNode(IdentifierNameSyntax node, TextSpan span)
        {
            SyntaxNode parent = node.Parent;

            SyntaxKind kind = parent.Kind();

            if (kind == SyntaxKind.InvocationExpression)
                return false;

            if (kind == SyntaxKind.SimpleMemberAccessExpression)
            {
                var memberAccessExpression = (MemberAccessExpressionSyntax)parent;

                if (object.ReferenceEquals(node, memberAccessExpression.Name)
                    && memberAccessExpression.IsParentKind(SyntaxKind.InvocationExpression))
                {
                    return false;
                }
            }

            return true;
        }

        protected override IPropertySymbol GetMemberSymbol(
            IdentifierNameSyntax node,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var propertySymbol = semanticModel.GetSymbol(node, cancellationToken) as IPropertySymbol;

            if (propertySymbol == null)
                return null;

            if (propertySymbol.Language != LanguageNames.CSharp)
                return null;

            if (propertySymbol.IsStatic)
                return propertySymbol;

            INamedTypeSymbol enclosingType = semanticModel.GetEnclosingNamedType(node.SpanStart, cancellationToken);

            if (propertySymbol.ContainingType?.Equals(enclosingType) != true)
                return null;

            if (node.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                if (((MemberAccessExpressionSyntax)node.Parent).Expression.IsKind(SyntaxKind.ThisExpression))
                    return propertySymbol;
            }
            else
            {
                return propertySymbol;
            }

            return null;
        }

        protected override async Task<PropertyDeclarationSyntax> GetMemberDeclarationAsync(IPropertySymbol symbol, CancellationToken cancellationToken)
        {
            SyntaxReference syntaxReference = symbol.DeclaringSyntaxReferences.FirstOrDefault();

            if (syntaxReference == null)
                return null;

            return (PropertyDeclarationSyntax)await syntaxReference.GetSyntaxAsync(cancellationToken).ConfigureAwait(false);
        }

        protected override ImmutableArray<ParameterInfo> GetParameterInfos(IdentifierNameSyntax node, IPropertySymbol symbol)
        {
            return ImmutableArray<ParameterInfo>.Empty;
        }

        protected override ExpressionOrStatements GetExpressionOrStatements(PropertyDeclarationSyntax declaration)
        {
            ArrowExpressionClauseSyntax expressionBody = declaration.ExpressionBody;

            if (expressionBody != null)
                return new ExpressionOrStatements(expressionBody.Expression);

            AccessorDeclarationSyntax accessor = declaration.AccessorList?.Accessors.SingleOrDefault(shouldThrow: false);

            if (accessor?.IsKind(SyntaxKind.GetAccessorDeclaration) != true)
                return default(ExpressionOrStatements);

            switch (accessor.Body?.Statements.SingleOrDefault(shouldThrow: false))
            {
                case ReturnStatementSyntax returnStatement:
                    return new ExpressionOrStatements(returnStatement.Expression);
                case ExpressionStatementSyntax expressionStatement:
                    return new ExpressionOrStatements(expressionStatement.Expression);
            }

            return default(ExpressionOrStatements);
        }

        protected override InlineRefactoring<IdentifierNameSyntax, PropertyDeclarationSyntax, IPropertySymbol> CreateRefactoring(
            Document document,
            IdentifierNameSyntax node,
            INamedTypeSymbol nodeEnclosingType,
            IPropertySymbol symbol,
            PropertyDeclarationSyntax declaration,
            ImmutableArray<ParameterInfo> parameterInfos,
            SemanticModel nodeSemanticModel,
            SemanticModel declarationSemanticModel,
            CancellationToken cancellationToken)
        {
            return new InlinePropertyRefactoring(document, node, nodeEnclosingType, symbol, declaration, parameterInfos, nodeSemanticModel, declarationSemanticModel, cancellationToken);
        }
    }
}
