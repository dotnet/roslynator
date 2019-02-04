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
        private static TypeSyntax _notImplementedException;
        private static TypeSyntax _notSupportedException;

        public static TypeSyntax BoolType()
        {
            return _boolType ?? (_boolType = Parse("System.Boolean"));
        }

        public static TypeSyntax IntType()
        {
            return _intType ?? (_intType = Parse("System.Int32"));
        }

        public static TypeSyntax StringType()
        {
            return _stringType ?? (_stringType = Parse("System.String"));
        }

        public static TypeSyntax ObjectType()
        {
            return _objectType ?? (_objectType = Parse("System.Object"));
        }

        public static TypeSyntax NotImplementedException()
        {
            return _notImplementedException ?? (_notImplementedException = Parse("System.NotImplementedException"));
        }

        public static TypeSyntax NotSupportedException()
        {
            return _notSupportedException ?? (_notSupportedException = Parse("System.NotSupportedException"));
        }

        private static TypeSyntax Parse(string typeName)
        {
            return ParseTypeName($"global::{typeName}").WithSimplifierAnnotation();
        }
    }
}
