// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text.RegularExpressions;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class CommentOutStatementRefactoring
    {
        public string GetValue()
        {
            string value = null;

            bool condition = false;

            if (condition)
            {
                return value;
            }

            if (condition)
            {
                return value;
            }
            else if (condition)
            {
                return value;
            }

            RegexOptions options = RegexOptions.None;

            switch (options)
            {
                case RegexOptions.CultureInvariant:
                    break;
                case RegexOptions.ECMAScript:
                    break;
                case RegexOptions.ExplicitCapture:
                    break;
                case RegexOptions.IgnoreCase:
                    break;
                case RegexOptions.IgnorePatternWhitespace:
                    break;
                case RegexOptions.Multiline:
                    break;
                case RegexOptions.None:
                    break;
                case RegexOptions.RightToLeft:
                    break;
                case RegexOptions.Singleline:
                    break;
                default:
                    break;
            }

            return null;
        }
    }
}
