// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class OperatorDeclarationSyntaxExtensions
    {
        public static TextSpan HeaderSpan(this OperatorDeclarationSyntax operatorDeclaration)
        {
            if (operatorDeclaration == null)
                throw new ArgumentNullException(nameof(operatorDeclaration));

            return TextSpan.FromBounds(
                operatorDeclaration.Span.Start,
                operatorDeclaration.ParameterList?.Span.End ?? operatorDeclaration.OperatorToken.Span.End);
        }
    }
}
