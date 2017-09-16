// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable RCS1078, RCS1056, RCS1176

using System;
using System.Collections.Generic;
using s = System.String;

namespace Roslynator.CSharp.Analyzers.Tests
{
    /// <summary>
    /// <see cref="String"/>
    /// <see cref="System.String"/>
    /// <see cref="global::System.String"/>
    /// <see cref="s"/>
    /// </summary>
    internal static class UsePredefinedType
    {
        public static void MethodName(String x)
        {
            x = default(String);
            x = default(System.String);
            x = default(global::System.String);

            x = String.Empty;
            x = System.String.Empty;
            x = global::System.String.Empty;

            x = nameof(String.Empty);
            x = nameof(List<String>);
            x = nameof(Dictionary<System.String, global::System.String>);

            // n

            s value = s.Empty;

            x = nameof(String);
            x = nameof(System.String);
            x = nameof(global::System.String);
        }
    }
}
