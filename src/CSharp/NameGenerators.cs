// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator
{
    internal static class NameGenerators
    {
        public static AsyncMethodNameGenerator AsyncMethod { get; } = new AsyncMethodNameGenerator();

        public static NumberSuffixNameGenerator NumberSuffix { get; } = new NumberSuffixNameGenerator();

        public static UnderscoreSuffixNameGenerator UnderscoreSuffix { get; } = new UnderscoreSuffixNameGenerator();
    }
}
