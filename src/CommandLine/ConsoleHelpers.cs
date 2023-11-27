// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;

namespace Roslynator.CommandLine;

internal static class ConsoleHelpers
{
    public static ImmutableArray<string> ReadRedirectedInputAsLines()
    {
        if (Console.IsInputRedirected)
        {
            ImmutableArray<string>.Builder lines = ImmutableArray.CreateBuilder<string>();

            using (Stream stream = Console.OpenStandardInput())
            using (var streamReader = new StreamReader(stream, Console.InputEncoding))
            {
                Task<string> readLineTask = streamReader.ReadLineAsync();

                // https://github.com/dotnet/runtime/issues/95079
                if (!readLineTask.Wait(TimeSpan.FromMilliseconds(500)))
                    return default;

                if (readLineTask.Result is null)
                    return ImmutableArray<string>.Empty;

                lines.Add(readLineTask.Result);

                string line;

                while ((line = streamReader.ReadLine()) is not null)
                    lines.Add(line);
            }

            return lines.ToImmutableArray();
        }

        return ImmutableArray<string>.Empty;
    }
}
