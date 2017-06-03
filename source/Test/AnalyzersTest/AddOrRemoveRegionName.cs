// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Test
{
    internal static class AddOrRemoveRegionName
    {
        #region Methods
        public static void Method()
        {
        }
        #endregion

        #region
        public static string Property { get; }
        #endregion Properties
    }
}
