// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal sealed class OptionValueProviderAttribute : Attribute
    {
        public OptionValueProviderAttribute(string propertyName, string providerName)
        {
            PropertyName = propertyName;
            ProviderName = providerName;
        }

        public string PropertyName { get; }

        public string ProviderName { get; }
    }
}
