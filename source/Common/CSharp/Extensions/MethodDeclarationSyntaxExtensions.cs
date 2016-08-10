// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class MethodDeclarationSyntaxExtensions
    {
        public static MethodDeclarationSyntax WithModifiers(
            this MethodDeclarationSyntax methodDeclaration,
            params SyntaxKind[] kinds)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return methodDeclaration.WithModifiers(CSharpFactory.TokenList(kinds));
        }

        public static MethodDeclarationSyntax WithBody(
            this MethodDeclarationSyntax methodDeclaration,
            IEnumerable<StatementSyntax> statements)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return methodDeclaration.WithBody(Block(statements));
        }

        public static MethodDeclarationSyntax WithBody(
            this MethodDeclarationSyntax methodDeclaration,
            params StatementSyntax[] statements)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return methodDeclaration.WithBody(Block(statements));
        }

        public static bool ReturnsVoid(this MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return methodDeclaration.ReturnType?.IsVoid() == true;
        }

        public static TextSpan HeaderSpan(this MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return TextSpan.FromBounds(
                methodDeclaration.Span.Start,
                methodDeclaration.ParameterList?.Span.End ?? methodDeclaration.Identifier.Span.End);
        }
    }
}
