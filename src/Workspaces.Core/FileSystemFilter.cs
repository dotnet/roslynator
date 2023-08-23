// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Roslynator;

internal sealed class FileSystemFilter
{
    private static readonly object _lock = new();

    public FileSystemFilter(Matcher matcher, bool isDefault, bool isDefaultInclude)
    {
        Matcher = matcher;
        IsDefault = isDefault;
        IsDefaultInclude = isDefaultInclude;
    }

    public Matcher Matcher { get; }

    public bool IsDefault { get; }

    public bool IsDefaultInclude { get; }

    public static FileSystemFilter Create(
        IEnumerable<string> include,
        IEnumerable<string> exclude)
    {
        var isDefault = false;
        var isDefaultInclude = false;

        var matcher = new Matcher((FileSystemHelpers.IsCaseSensitive) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);

        if (include.Any())
        {
            matcher.AddIncludePatterns(include);
        }
        else
        {
            matcher.AddInclude("**");
            isDefaultInclude = true;
            isDefault = true;
        }

        if (exclude.Any())
        {
            matcher.AddExcludePatterns(exclude);
            isDefault = false;
        }

        return new FileSystemFilter(matcher, isDefault, isDefaultInclude);
    }

    public bool IsMatch(ISymbol symbol)
    {
        if (IsDefault)
            return true;

        bool isMatch = Matcher.IsMatch(symbol);

        if (!isMatch
            && Logger.ShouldWrite(Verbosity.Diagnostic))
        {
            lock (_lock)
                Logger.WriteLine($"Excluding symbol '{symbol.ToDisplayString(SymbolDisplayFormats.FullName)}'", ConsoleColors.DarkGray, Verbosity.Diagnostic);
        }

        return isMatch;
    }

    public bool IsMatch(string filePath)
    {
        if (IsDefault)
            return true;

        PatternMatchingResult result = Matcher.Match(filePath);

        Debug.Assert(result.Files.Count() <= 1, result.Files.Count().ToString());

        if (!result.HasMatches
            && Logger.ShouldWrite(Verbosity.Diagnostic))
        {
            lock (_lock)
                Logger.WriteLine($"Excluding file '{filePath}'", ConsoleColors.DarkGray, Verbosity.Diagnostic);
        }

        return result.HasMatches;
    }
}
