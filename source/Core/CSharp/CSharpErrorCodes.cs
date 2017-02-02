// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp
{
    public static class CSharpErrorCodes
    {
        private const string Prefix = "CS";

        public const string OperatorCannotBeAppliedToOperands = Prefix + "0019";
        public const string CannotImplicitlyConvertType = Prefix + "0029";
        public const string NotAllCodePathsReturnValue = Prefix + "0161";
        public const string UnreachableCodeDetected = Prefix + "0162";
        public const string CannotImplicitlyConvertTypeExplicitConversionExists = Prefix + "0266";
        public const string MemberTypeMustMatchOverridenMemberType = Prefix + "0508";
        public const string MissingXmlComment = Prefix + "1591";
        public const string CannotReturnValueFromIterator = Prefix + "1622";
        public const string TypeUsedInUsingStatementMustBeImplicitlyConvertibleToIDisposable = Prefix + "1674";
    }
}
