// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class MethodDeclarationSyntaxExtensions
    {
        public static bool IsIterator(this MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return methodDeclaration
                    .DescendantNodes(node => !node.IsKind(
                        SyntaxKind.SimpleLambdaExpression,
                        SyntaxKind.ParenthesizedLambdaExpression,
                        SyntaxKind.AnonymousMethodExpression))
                    .Any(f => f.IsYieldStatement());
        }

        public static MethodDeclarationSyntax WithModifiers(
            this MethodDeclarationSyntax methodDeclaration,
            params SyntaxKind[] kinds)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return methodDeclaration.WithModifiers(CSharpFactory.TokenList(kinds));
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
