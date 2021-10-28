// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator.Host.Mef
{
    internal class LanguageMetadata
    {
        public string Language { get; }

        public LanguageMetadata(IDictionary<string, object> data)
        {
            Language = (string)data.GetValueOrDefault("Language");
        }

        public LanguageMetadata(string language)
        {
            Language = language;
        }
    }
}
