// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Spelling
{
    [Flags]
    internal enum SplitMode
    {
        None = 0,
        Case = 1,
        Hyphen = 1 << 1,
        CaseAndHyphen = Case | Hyphen,
    }
}
