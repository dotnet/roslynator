// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#region usings
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator;
#endregion usings

#pragma warning disable RCS1018, RCS1213, CA1822

namespace Roslynator
{
    public class Class1<T> // RCS1102 is reported on Class1
    {
        public sealed class Class2 : Class1<string>
        {
        }
    }
}
