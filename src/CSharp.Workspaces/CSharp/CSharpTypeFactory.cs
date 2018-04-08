// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp
{
    internal static class CSharpTypeFactory
    {
        private static TypeSyntax _boolType;
        private static TypeSyntax _byteType;
        private static TypeSyntax _sbyteType;
        private static TypeSyntax _intType;
        private static TypeSyntax _uintType;
        private static TypeSyntax _shortType;
        private static TypeSyntax _ushortType;
        private static TypeSyntax _longType;
        private static TypeSyntax _ulongType;
        private static TypeSyntax _floatType;
        private static TypeSyntax _doubleType;
        private static TypeSyntax _decimalType;
        private static TypeSyntax _stringType;
        private static TypeSyntax _charType;
        private static TypeSyntax _objectType;

        public static TypeSyntax BoolType()
        {
            return _boolType ?? (_boolType = ParseTypeName("global::System.Boolean").WithSimplifierAnnotation());
        }

        public static TypeSyntax ByteType()
        {
            return _byteType ?? (_byteType = ParseTypeName("global::System.Byte").WithSimplifierAnnotation());
        }

        public static TypeSyntax SByteType()
        {
            return _sbyteType ?? (_sbyteType = ParseTypeName("global::System.SByte").WithSimplifierAnnotation());
        }

        public static TypeSyntax IntType()
        {
            return _intType ?? (_intType = ParseTypeName("global::System.Int32").WithSimplifierAnnotation());
        }

        public static TypeSyntax UIntType()
        {
            return _uintType ?? (_uintType = ParseTypeName("global::System.UInt32").WithSimplifierAnnotation());
        }

        public static TypeSyntax ShortType()
        {
            return _shortType ?? (_shortType = ParseTypeName("global::System.Int16").WithSimplifierAnnotation());
        }

        public static TypeSyntax UShortType()
        {
            return _ushortType ?? (_ushortType = ParseTypeName("global::System.UInt16").WithSimplifierAnnotation());
        }

        public static TypeSyntax LongType()
        {
            return _longType ?? (_longType = ParseTypeName("global::System.Int64").WithSimplifierAnnotation());
        }

        public static TypeSyntax ULongType()
        {
            return _ulongType ?? (_ulongType = ParseTypeName("global::System.UInt64").WithSimplifierAnnotation());
        }

        public static TypeSyntax FloatType()
        {
            return _floatType ?? (_floatType = ParseTypeName("global::System.Single").WithSimplifierAnnotation());
        }

        public static TypeSyntax DoubleType()
        {
            return _doubleType ?? (_doubleType = ParseTypeName("global::System.Double").WithSimplifierAnnotation());
        }

        public static TypeSyntax DecimalType()
        {
            return _decimalType ?? (_decimalType = ParseTypeName("global::System.Decimal").WithSimplifierAnnotation());
        }

        public static TypeSyntax StringType()
        {
            return _stringType ?? (_stringType = ParseTypeName("global::System.String").WithSimplifierAnnotation());
        }

        public static TypeSyntax CharType()
        {
            return _charType ?? (_charType = ParseTypeName("global::System.Char").WithSimplifierAnnotation());
        }

        public static TypeSyntax ObjectType()
        {
            return _objectType ?? (_objectType = ParseTypeName("global::System.Object").WithSimplifierAnnotation());
        }
    }
}
