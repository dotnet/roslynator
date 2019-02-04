// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Roslynator
{
    internal static class XmlTagMapper
    {
        private static readonly ImmutableDictionary<string, XmlTag> _map = CreateMap();

        private static ImmutableDictionary<string, XmlTag> CreateMap()
        {
            return ImmutableDictionary.CreateRange(
                StringComparer.OrdinalIgnoreCase,
                new KeyValuePair<string, XmlTag>[]
                {
                    new KeyValuePair<string, XmlTag>(WellKnownXmlTags.C, XmlTag.C),
                    new KeyValuePair<string, XmlTag>(WellKnownXmlTags.Code, XmlTag.Code),
                    new KeyValuePair<string, XmlTag>(WellKnownXmlTags.Content, XmlTag.Content),
                    new KeyValuePair<string, XmlTag>(WellKnownXmlTags.Example, XmlTag.Example),
                    new KeyValuePair<string, XmlTag>(WellKnownXmlTags.Exception, XmlTag.Exception),
                    new KeyValuePair<string, XmlTag>(WellKnownXmlTags.Exclude, XmlTag.Exclude),
                    new KeyValuePair<string, XmlTag>(WellKnownXmlTags.Include, XmlTag.Include),
                    new KeyValuePair<string, XmlTag>(WellKnownXmlTags.InheritDoc, XmlTag.InheritDoc),
                    new KeyValuePair<string, XmlTag>(WellKnownXmlTags.List, XmlTag.List),
                    new KeyValuePair<string, XmlTag>(WellKnownXmlTags.Para, XmlTag.Para),
                    new KeyValuePair<string, XmlTag>(WellKnownXmlTags.Param, XmlTag.Param),
                    new KeyValuePair<string, XmlTag>(WellKnownXmlTags.ParamRef, XmlTag.ParamRef),
                    new KeyValuePair<string, XmlTag>(WellKnownXmlTags.Permission, XmlTag.Permission),
                    new KeyValuePair<string, XmlTag>(WellKnownXmlTags.Remarks, XmlTag.Remarks),
                    new KeyValuePair<string, XmlTag>(WellKnownXmlTags.Returns, XmlTag.Returns),
                    new KeyValuePair<string, XmlTag>(WellKnownXmlTags.See, XmlTag.See),
                    new KeyValuePair<string, XmlTag>(WellKnownXmlTags.SeeAlso, XmlTag.SeeAlso),
                    new KeyValuePair<string, XmlTag>(WellKnownXmlTags.Summary, XmlTag.Summary),
                    new KeyValuePair<string, XmlTag>(WellKnownXmlTags.TypeParam, XmlTag.TypeParam),
                    new KeyValuePair<string, XmlTag>(WellKnownXmlTags.TypeParamRef, XmlTag.TypeParamRef),
                    new KeyValuePair<string, XmlTag>(WellKnownXmlTags.Value, XmlTag.Value),
                });
        }

        public static XmlTag GetTagOrDefault(string name)
        {
            if (_map.TryGetValue(name, out XmlTag kind))
                return kind;

            return XmlTag.None;
        }

        public static string GetName(XmlTag kind)
        {
            switch (kind)
            {
                case XmlTag.None:
                    return "";
                case XmlTag.C:
                    return WellKnownXmlTags.C;
                case XmlTag.Code:
                    return WellKnownXmlTags.Code;
                case XmlTag.Content:
                    return WellKnownXmlTags.Content;
                case XmlTag.Example:
                    return WellKnownXmlTags.Example;
                case XmlTag.Exception:
                    return WellKnownXmlTags.Exception;
                case XmlTag.Exclude:
                    return WellKnownXmlTags.Exclude;
                case XmlTag.Include:
                    return WellKnownXmlTags.Include;
                case XmlTag.InheritDoc:
                    return WellKnownXmlTags.InheritDoc;
                case XmlTag.List:
                    return WellKnownXmlTags.List;
                case XmlTag.Para:
                    return WellKnownXmlTags.Para;
                case XmlTag.Param:
                    return WellKnownXmlTags.Param;
                case XmlTag.ParamRef:
                    return WellKnownXmlTags.ParamRef;
                case XmlTag.Permission:
                    return WellKnownXmlTags.Permission;
                case XmlTag.Remarks:
                    return WellKnownXmlTags.Remarks;
                case XmlTag.Returns:
                    return WellKnownXmlTags.Returns;
                case XmlTag.See:
                    return WellKnownXmlTags.See;
                case XmlTag.SeeAlso:
                    return WellKnownXmlTags.SeeAlso;
                case XmlTag.Summary:
                    return WellKnownXmlTags.Summary;
                case XmlTag.TypeParam:
                    return WellKnownXmlTags.TypeParam;
                case XmlTag.TypeParamRef:
                    return WellKnownXmlTags.TypeParamRef;
                case XmlTag.Value:
                    return WellKnownXmlTags.Value;
                default:
                    throw new ArgumentException("", nameof(kind));
            }
        }
    }
}
