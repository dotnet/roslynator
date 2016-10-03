// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class SimpleNameSyntaxExtensions
    {
        public static bool IsQualified(this SimpleNameSyntax identifierName)
        {
            if (identifierName == null)
                throw new ArgumentNullException(nameof(identifierName));

            return identifierName.Parent?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true;
        }

        public static bool IsQualifiedWithThis(this SimpleNameSyntax identifierName)
        {
            if (identifierName == null)
                throw new ArgumentNullException(nameof(identifierName));

            if (IsQualified(identifierName))
            {
                var memberAccess = (MemberAccessExpressionSyntax)identifierName.Parent;

                return memberAccess.Expression?.IsKind(SyntaxKind.ThisExpression) == true;
            }

            return false;
        }
    }
}
