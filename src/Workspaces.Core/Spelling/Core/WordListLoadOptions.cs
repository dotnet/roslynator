// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Spelling
{
    [Flags]
    internal enum WordListLoadOptions
    {
        None = 0,
        IgnoreCase = 1,
    }
}
