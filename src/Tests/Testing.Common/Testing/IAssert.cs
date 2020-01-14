// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Testing
{
    public interface IAssert
    {
        public abstract void Equal(string expected, string actual);

        public abstract void True(bool condition, string userMessage);
    }
}
