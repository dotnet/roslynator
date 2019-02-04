// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp
{
    internal static class CSharpTypeFactory
    {
        private static TypeSyntax _boolType;
        private static TypeSyntax _intType;
        private static TypeSyntax _stringType;
        private static TypeSyntax _objectType;

        public static TypeSyntax BoolType()
        {
            return _boolType ?? (_boolType = ParseTypeName("global::System.Boolean").WithSimplifierAnnotation());
        }

        public static TypeSyntax IntType()
        {
            return _intType ?? (_intType = ParseTypeName("global::System.Int32").WithSimplifierAnnotation());
        }

        public static TypeSyntax StringType()
        {
            return _stringType ?? (_stringType = ParseTypeName("global::System.String").WithSimplifierAnnotation());
        }

        public static TypeSyntax ObjectType()
        {
            return _objectType ?? (_objectType = ParseTypeName("global::System.Object").WithSimplifierAnnotation());
        }
    }
}
