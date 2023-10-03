// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Roslynator;

internal sealed class FileSystemFilter
{
#if DEBUG
    private static readonly object _lock = new();
#endif

    private FileSystemFilter(Matcher matcher)
    {
        Matcher = matcher;
    }

    public Matcher Matcher { get; }

    public string RootDirectoryPath { get; set; }

    public static FileSystemFilter CreateOrDefault(
        IEnumerable<string> include,
        IEnumerable<string> exclude)
    {
        if (!include.Any()
            && !exclude.Any())
        {
            return null;
        }

        var matcher = new Matcher((FileSystemHelpers.IsCaseSensitive) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);

        if (include.Any())
        {
            matcher.AddIncludePatterns(include);
        }
        else
        {
            matcher.AddInclude("**");
        }

        if (exclude.Any())
            matcher.AddExcludePatterns(exclude);

        return new FileSystemFilter(matcher);
    }

    public bool IsMatch(ISymbol symbol)
    {
        bool isMatch = Matcher.IsMatch(symbol, RootDirectoryPath);
#if DEBUG
        if (!isMatch
            && Logger.ShouldWrite(Verbosity.Diagnostic))
        {
            lock (_lock)
                Logger.WriteLine($"Excluding symbol '{symbol.ToDisplayString(SymbolDisplayFormats.FullName)}'", ConsoleColors.DarkGray, Verbosity.Diagnostic);
        }
#endif
        return isMatch;
    }

    public bool IsMatch(string filePath)
    {
        PatternMatchingResult result = (RootDirectoryPath is not null)
            ? Matcher.Match(RootDirectoryPath, filePath)
            : Matcher.Match(filePath);

#if DEBUG
        Debug.Assert(result.Files.Count() <= 1, result.Files.Count().ToString());

        if (!result.HasMatches
            && Logger.ShouldWrite(Verbosity.Diagnostic))
        {
            lock (_lock)
                Logger.WriteLine($"Excluding file '{filePath}'", ConsoleColors.DarkGray, Verbosity.Diagnostic);
        }
#endif
        return result.HasMatches;
    }
}
