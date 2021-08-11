// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;

namespace Roslynator.Text
{
    internal static class EncodingHelpers
    {
        public static Encoding UTF8NoBom { get; } = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
    }
}
