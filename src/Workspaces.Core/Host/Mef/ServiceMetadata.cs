// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator.Host.Mef
{
    internal class ServiceMetadata
    {
        public string ServiceType { get; }

        public IReadOnlyDictionary<string, object> Data { get; }

        public ServiceMetadata(IDictionary<string, object> data)
        {
            ServiceType = (string)data.GetValueOrDefault("ServiceType");

            Data = (IReadOnlyDictionary<string, object>)data;
        }
    }
}
