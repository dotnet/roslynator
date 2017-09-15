// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CodeGeneration.CSharp
{
    internal static class Extensions
    {
        public static FieldDeclarationSyntax AddObsoleteAttributeIf(this FieldDeclarationSyntax fieldDeclaration, bool condition, string message = "", bool error = false)
        {
            return (condition) ? AddObsoleteAttribute(fieldDeclaration, message, error) : fieldDeclaration;
        }

        public static FieldDeclarationSyntax AddObsoleteAttribute(this FieldDeclarationSyntax fieldDeclaration, string message = "", bool error = false)
        {
            return fieldDeclaration.AddAttributeLists(
                AttributeList(
                    Attribute(
                        IdentifierName("Obsolete"),
                        AttributeArgumentList(
                            AttributeArgument(StringLiteralExpression(message)),
                            AttributeArgument(NameColon("error"), BooleanLiteralExpression(error))))));
        }
    }
}
