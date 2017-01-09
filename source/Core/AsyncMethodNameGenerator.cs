// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator
{
    internal class AsyncMethodNameGenerator : UniqueNameGenerator
    {
        private const string AsyncSuffix = "Async";

        private int _suffix = 1;

        public override string GetInitialName(string name)
        {
            return name + AsyncSuffix;
        }

        public override string GetNewName(string name)
        {
            _suffix++;
            return name + _suffix.ToString() + AsyncSuffix;
        }
    }
}
