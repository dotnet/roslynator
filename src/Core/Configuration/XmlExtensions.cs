// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Xml.Linq;

namespace Roslynator.Configuration
{
    internal static class XmlExtensions
    {
        public static bool HasName(this XElement element, string name, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return string.Equals(element.Name.LocalName, name, comparison);
        }

        public static bool HasName(this XAttribute attribute, string name, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return string.Equals(attribute.Name.LocalName, name, comparison);
        }

        public static bool? GetValueAsBoolean(this XAttribute attribute)
        {
            if (bool.TryParse(attribute.Value, out bool result))
            {
                return result;
            }

            return null;
        }
    }
}
