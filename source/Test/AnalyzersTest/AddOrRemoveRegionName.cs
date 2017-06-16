// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable RCS1016, RCS1163

namespace Roslynator.CSharp.Analyzers.Test
{
    internal static class AddOrRemoveRegionName
    {
        private class Foo
        {
            #region Methods
            public static void Method()
            {
            }
            #endregion

            #region Indexers
            public string this[int index]
            {
                get { return ""; }
            }
            #endregion //

            #region
            public string Property{ get; }
            #endregion Properties
        }
    }
}
