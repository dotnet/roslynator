// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator
{
    public static class FileUtility
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
            return (language == LanguageNames.CSharp)
                ? DefaultCSharpFileName
                : DefaultVisualBasicFileName;
        }

        public static string CreateFileName(string fileName = TestFileName, int suffix = FileNumberingBase, string language = LanguageNames.CSharp)
        {
            string extension = ((language == LanguageNames.CSharp)
                ? CSharpFileExtension
                : VisualBasicFileExtension);

            return $"{fileName}{suffix}.{extension}";
        }
    }
}
