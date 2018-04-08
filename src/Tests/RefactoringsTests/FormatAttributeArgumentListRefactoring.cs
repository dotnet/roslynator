// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal static class FormatAttributeArgumentListRefactoring
    {
        [MyAttribute(null, null, null)]
        private class MyAttribute : Attribute
        {
            public MyAttribute(object parameter1, object parameter2, object parameter3)
            {
            }
        }
    }
}
