// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings.AddOrRemoveParameterName
{
    internal class AddParameterNameRewriter : CSharpSyntaxRewriter
    {
        private static readonly SymbolDisplayFormat _symbolDisplayFormat = new SymbolDisplayFormat(
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers,
            parameterOptions: SymbolDisplayParameterOptions.IncludeName);

        private readonly ImmutableArray<ArgumentSyntax> _arguments;
        private readonly SemanticModel _semanticModel;

        public AddParameterNameRewriter(ImmutableArray<ArgumentSyntax> arguments, SemanticModel semanticModel)
        {
            _arguments = arguments;
            _semanticModel = semanticModel;
        }

        public override SyntaxNode VisitArgument(ArgumentSyntax node)
        {
            if (_arguments.Contains(node))
            {
                return AddParameterName(node, _semanticModel);
            }
            else
            {
                return base.VisitArgument(node);
            }
        }

        private static ArgumentSyntax AddParameterName(
            ArgumentSyntax argument,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (argument.NameColon == null || argument.NameColon.IsMissing)
            {
                IParameterSymbol parameterSymbol = semanticModel.DetermineParameter(
                    argument,
                    allowParams: false,
                    cancellationToken: cancellationToken);

                if (parameterSymbol != null)
                {
                    return argument
                        .WithNameColon(
                            NameColon(parameterSymbol.ToDisplayString(_symbolDisplayFormat))
                                .WithTrailingTrivia(Space))
                        .WithTriviaFrom(argument);
                }
            }

            return argument;
        }
    }
}
