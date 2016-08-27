// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class ConstructorDeclarationSyntaxExtensions
    {
        public static ConstructorDeclarationSyntax WithBody(
            this ConstructorDeclarationSyntax constructorDeclaration,
            IEnumerable<StatementSyntax> statements)
        {
            if (constructorDeclaration == null)
                throw new ArgumentNullException(nameof(constructorDeclaration));

            return constructorDeclaration.WithBody(Block(statements));
        }

        public static TextSpan HeaderSpan(this ConstructorDeclarationSyntax constructorDeclaration)
        {
            if (constructorDeclaration == null)
                throw new ArgumentNullException(nameof(constructorDeclaration));

            return TextSpan.FromBounds(
                constructorDeclaration.Span.Start,
                constructorDeclaration.ParameterList?.Span.End ?? constructorDeclaration.Identifier.Span.End);
        }
    }
}
