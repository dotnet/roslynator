// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    public static class CSharpExtensions
    {
        public static bool IsExplicitConversion(this SemanticModel semanticModel, ExpressionSyntax expression, ITypeSymbol destinationType)
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            if (destinationType == null)
                throw new ArgumentNullException(nameof(destinationType));

            if (!destinationType.IsErrorType()
                && !destinationType.IsVoid())
            {
                Conversion conversion = semanticModel.ClassifyConversion(
                    expression,
                    destinationType,
                    isExplicitInSource: false);

                if (conversion.IsExplicit)
                    return true;
            }

            return false;
        }
    }
}
