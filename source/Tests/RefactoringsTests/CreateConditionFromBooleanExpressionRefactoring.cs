// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class CreateConditionFromBooleanExpressionRefactoring
    {
        public bool GetValue()
        {
            bool boolValue = false;

            return boolValue;
        }

        public IEnumerable<bool> GetValues()
        {
            bool boolValue = false;

            yield return boolValue;
        }
    }
}
