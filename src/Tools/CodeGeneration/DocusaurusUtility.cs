// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;

namespace Roslynator.CodeGeneration;

public static class DocusaurusUtility
{
    public static string CreateFrontMatter(int? position = null, string label = null)
    {
        if (position is not null
            || label is not null)
        {
            var sb = new StringBuilder();

            sb.AppendLine("---");

            if (position is not null)
                sb.AppendLine($"sidebar_position: {position}");

            if (label is not null)
                sb.AppendLine($"sidebar_label: {label}");

            sb.AppendLine("---");
            sb.AppendLine();

            return sb.ToString();
        }

        return null;
    }

}
