// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp
{
    public static class CSharpErrorCodes
    {
        private const string Prefix = "CS";

        public const string TypeUsedInUsingStatementMustBeImplicitlyConvertibleToIDisposable = Prefix + "1674";
        public const string CannotImplicitlyConvertType = Prefix + "0029";
    }
}
