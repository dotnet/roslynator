// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.UnusedSyntax
{
    internal class UnusedLocalFunctionParameterRefactoring : UnusedSyntaxRefactoring<LocalFunctionStatementSyntax, ParameterListSyntax, ParameterSyntax, IParameterSymbol>
    {
        private UnusedLocalFunctionParameterRefactoring()
        {
        }

        public static UnusedLocalFunctionParameterRefactoring Instance { get; } = new UnusedLocalFunctionParameterRefactoring();

        protected override ImmutableArray<ParameterSyntax> FindUnusedSyntax(
            LocalFunctionStatementSyntax node,
            ParameterListSyntax list,
            SeparatedSyntaxList<ParameterSyntax> separatedList,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (GetBody(node) != null)
            {
                return base.FindUnusedSyntax(node, list, separatedList, semanticModel, cancellationToken);
            }
            else
            {
                return ImmutableArray<ParameterSyntax>.Empty;
            }
        }

        protected override CSharpSyntaxNode GetBody(LocalFunctionStatementSyntax node)
        {
            return node.BodyOrExpressionBody();
        }

        protected override string GetIdentifier(ParameterSyntax syntax)
        {
            return syntax.Identifier.ValueText;
        }

        protected override ParameterListSyntax GetList(LocalFunctionStatementSyntax node)
        {
            return node.ParameterList;
        }

        protected override SyntaxTokenList GetModifiers(LocalFunctionStatementSyntax node)
        {
            return node.Modifiers;
        }

        protected override SeparatedSyntaxList<ParameterSyntax> GetSeparatedList(ParameterListSyntax list)
        {
            return list.Parameters;
        }
    }
}
