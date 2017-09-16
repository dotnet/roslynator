// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class ReplaceSwitchWithIfElseRefactoring
    {
        public string SomeMethod()
        {
            var options = StringSplitOptions.None;

            switch (options)
            {
                case StringSplitOptions.None:
                    return "None";
                case StringSplitOptions.RemoveEmptyEntries:
                    return "RemoveEmptyEntries";
            }

            return string.Empty;
        }
    }
}
