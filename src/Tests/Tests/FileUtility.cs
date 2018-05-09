// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class FileUtility
    {
        public const string TestFileName = "Test";
        public const string TestProjectName = "TestProject";
        public const string CSharpFileExtension = "cs";
        public const string VisualBasicFileExtension = "vb";
        public const string DefaultCSharpFileName = TestFileName + "0." + CSharpFileExtension;
        public const string DefaultVisualBasicFileName = TestFileName + "0." + VisualBasicFileExtension;
        internal const int FileNumberingBase = 0;

        public static string CreateDefaultFileName(string language)
        {
            if (language == LanguageNames.CSharp)
                return DefaultCSharpFileName;

            if (language == LanguageNames.VisualBasic)
                return DefaultVisualBasicFileName;

            throw new NotSupportedException();
        }

        public static string CreateFileName(string language, string fileName = TestFileName, int suffix = FileNumberingBase)
        {
            return $"{fileName}{suffix}.{GetExtension(language)}";
        }

        private static string GetExtension(string language)
        {
            if (language == LanguageNames.CSharp)
                return CSharpFileExtension;

            if (language == LanguageNames.VisualBasic)
                return VisualBasicFileExtension;

            throw new NotSupportedException();
        }
    }
}
