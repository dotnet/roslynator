// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator
{
    public interface ICapture
    {
        string Value { get; }

        int Index { get; }

        int Length { get; }
    }
}
