// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Roslynator.Testing.CSharp;

namespace Roslynator
{
    internal static class CSharpTestExtensions
    {
        public static CSharpTestOptions AddConfigOption(this CSharpTestOptions options, string key, string value)
        {
            return options.WithConfigOptions(options.ConfigOptions.SetItem(key, value));
        }

        public static CSharpTestOptions AddConfigOption(this CSharpTestOptions options, string key, bool value)
        {
            return AddConfigOption(options, key, (value) ? "true" : "false");
        }

        public static CSharpTestOptions EnableConfigOption(this CSharpTestOptions options, string key)
        {
            return AddConfigOption(options, key, "true");
        }
    }
}
