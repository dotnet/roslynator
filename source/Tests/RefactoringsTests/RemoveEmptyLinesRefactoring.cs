// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings.Tests
{
    internal class RemoveEmptyLinesRefactoring
    {

        public bool GetValue()
        {

            object value = null;

            var array = value as string[];

            if (array == null)
            {

                return false;
            }

            return false;
        }

    }
}
