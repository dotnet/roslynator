// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

#pragma warning disable RCS1004, RCS1007, RCS1073

namespace Roslynator.CSharp.Analyzers.Test
{
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
}
