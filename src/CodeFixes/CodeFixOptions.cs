// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator;

internal static class CodeFixOptions
{
    public static readonly CodeFixOption CS1591_MissingXmlCommentForPubliclyVisibleTypeOrMember_IgnoredTags = new("roslynator.CS1591.ignored_tags", "<COMMA_SEPARATED_LIST_OF_IGNORED_XML_TAGS>");
}
