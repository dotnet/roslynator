// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Tests
{
#pragma warning disable RCS1004, RCS1007, RCS1073
    internal static class AvoidEmbeddedStatementInIfElse
    {
        public static bool Foo()
        {
            bool f = false;

            if (f)
                return true;

            if (f)
                return true;
            else
                return false;
        }
    }
#pragma warning restore RCS1004, RCS1007, RCS1073
}
