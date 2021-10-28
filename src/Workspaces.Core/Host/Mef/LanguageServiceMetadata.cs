// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator.Host.Mef
{
    internal class LanguageServiceMetadata : LanguageMetadata
    {
        public string ServiceType { get; }

        public IReadOnlyDictionary<string, object> Data { get; }

        public LanguageServiceMetadata(IDictionary<string, object> data) : base(data)
        {
            ServiceType = (string)data.GetValueOrDefault("ServiceType");

            Data = (IReadOnlyDictionary<string, object>)data;
        }
    }
}
