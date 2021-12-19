// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator
{
    internal static class NameGenerators
    {
        public static AsyncMethodNameGenerator AsyncMethod { get; } = new();

        public static NumberSuffixNameGenerator NumberSuffix { get; } = new();

        public static UnderscoreSuffixNameGenerator UnderscoreSuffix { get; } = new();
    }
}
