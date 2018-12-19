// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Globalization;

namespace Roslynator
{
    internal static class PathHelpers
    {
        public static string AppendNumberToFileName(string fileName, int number)
        {
            int index = fileName.LastIndexOf(".");

            return fileName.Insert(index, (number).ToString(CultureInfo.InvariantCulture));
        }
    }
}
