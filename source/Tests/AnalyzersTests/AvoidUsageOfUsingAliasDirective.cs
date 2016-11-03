// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using x = System;
using xx = System.Collections.Generic;
using xxx = System.String;

namespace Roslynator.CSharp.Analyzers.Tests
{
    public class AvoidUsageOfUsingAliasDirective
    {
        public void MethodName()
        {
            xxx ss = null;

            string s = xxx.Empty;
            string s2 = xxx.Empty;

            xx.List<string> t = new xx.List<string>();
            xx.List<string> t2 = new xx.List<string>();

            string u = x.String.Empty;
            string u2 = x.String.Empty;
        }
    }
}
