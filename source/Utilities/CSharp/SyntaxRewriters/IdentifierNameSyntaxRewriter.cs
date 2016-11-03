// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.SyntaxRewriters
{
    public class IdentifierNameSyntaxRewriter : CSharpSyntaxRewriter
    {
        public IdentifierNameSyntaxRewriter(ImmutableArray<IdentifierNameSyntax> identifierNames, SyntaxToken newIdentifier)
        {
            if (identifierNames == null)
                throw new ArgumentNullException(nameof(identifierNames));

            IdentifierNames = identifierNames;
            NewIdentifier = newIdentifier;
        }

        public ImmutableArray<IdentifierNameSyntax> IdentifierNames { get; }

        public SyntaxToken NewIdentifier { get; }

        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            if (IdentifierNames.Contains(node))
            {
                return node
                    .WithIdentifier(NewIdentifier)
                    .WithTriviaFrom(node);
            }

            return base.VisitIdentifierName(node);
        }
    }
}
