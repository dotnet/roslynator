// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Xml.Linq;

namespace Roslynator.Metadata
{
    internal static class XmlExtensions
    {
        public static bool AttributeValueAsBoolean(this XElement element, string attributeName)
        {
            return bool.Parse(element.Attribute(attributeName).Value);
        }

        public static bool AttributeValueAsBooleanOrDefault(this XElement element, string attributeName, bool defaultValue = false)
        {
            XAttribute attribute = element.Attribute(attributeName);

            if (attribute == null)
                return defaultValue;

            return bool.Parse(attribute.Value);
        }
    }
}
