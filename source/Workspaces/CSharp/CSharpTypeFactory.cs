// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp
{
    internal static class CSharpTypeFactory
    {
        public static TypeSyntax BoolType()
        {
            return ParseTypeName("global::System.Boolean").WithSimplifierAnnotation();
        }

        public static TypeSyntax ByteType()
        {
            return ParseTypeName("global::System.Byte").WithSimplifierAnnotation();
        }

        public static TypeSyntax SByteType()
        {
            return ParseTypeName("global::System.SByte").WithSimplifierAnnotation();
        }

        public static TypeSyntax IntType()
        {
            return ParseTypeName("global::System.Int32").WithSimplifierAnnotation();
        }

        public static TypeSyntax UIntType()
        {
            return ParseTypeName("global::System.UInt32").WithSimplifierAnnotation();
        }

        public static TypeSyntax ShortType()
        {
            return ParseTypeName("global::System.Int16").WithSimplifierAnnotation();
        }

        public static TypeSyntax UShortType()
        {
            return ParseTypeName("global::System.UInt16").WithSimplifierAnnotation();
        }

        public static TypeSyntax LongType()
        {
            return ParseTypeName("global::System.Int64").WithSimplifierAnnotation();
        }

        public static TypeSyntax ULongType()
        {
            return ParseTypeName("global::System.UInt64").WithSimplifierAnnotation();
        }

        public static TypeSyntax FloatType()
        {
            return ParseTypeName("global::System.Single").WithSimplifierAnnotation();
        }

        public static TypeSyntax DoubleType()
        {
            return ParseTypeName("global::System.Double").WithSimplifierAnnotation();
        }

        public static TypeSyntax DecimalType()
        {
            return ParseTypeName("global::System.Decimal").WithSimplifierAnnotation();
        }

        public static TypeSyntax StringType()
        {
            return ParseTypeName("global::System.String").WithSimplifierAnnotation();
        }

        public static TypeSyntax CharType()
        {
            return ParseTypeName("global::System.Char").WithSimplifierAnnotation();
        }

        public static TypeSyntax ObjectType()
        {
            return ParseTypeName("global::System.Object").WithSimplifierAnnotation();
        }
    }
}
