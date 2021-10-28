// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.CodeFixes
{
    internal readonly struct CodeFixRegistrationResult
    {
        public CodeFixRegistrationResult(bool success)
        {
            Success = success;
        }

        public bool Success { get; }

        public CodeFixRegistrationResult CombineWith(in CodeFixRegistrationResult other)
        {
            return Combine(this, other);
        }

        public static CodeFixRegistrationResult Combine(in CodeFixRegistrationResult result1, in CodeFixRegistrationResult result2)
        {
            return new CodeFixRegistrationResult(result1.Success || result2.Success);
        }
    }
}
