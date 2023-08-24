// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CommandLine;

internal static class CommandLineHelpers
{
    public static bool IsGlobPatternForFileOrFolder(string pattern)
    {
        return !IsGlobPatternForProject(pattern)
            && !IsGlobPatternForSolution(pattern);
    }

    public static bool IsGlobPatternForProject(string pattern)
    {
        return pattern.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase)
            || pattern.EndsWith(".vbproj", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsGlobPatternForSolution(string pattern)
    {
        return pattern.EndsWith(".sln", StringComparison.OrdinalIgnoreCase);
    }

    public static void WaitForKeyPress(string message = null)
    {
        if (Console.IsInputRedirected)
            return;

        Console.Write(message ?? "Press any key to continue...");
        Console.ReadKey();
        Console.WriteLine();
    }
}
