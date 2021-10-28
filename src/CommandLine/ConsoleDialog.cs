// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CommandLine
{
    internal class ConsoleDialog : IUserDialog
    {
        public ConsoleDialog(ConsoleDialogDefinition definition, string indent)
        {
            Definition = definition;
            Indent = indent;
        }

        public ConsoleDialogDefinition Definition { get; }

        public string Indent { get; }

        public DialogResult ShowDialog(string text)
        {
            while (true)
            {
                Console.Write($"{Indent}{text} ({Definition.ShortHelp}): ");

                string s = Console.ReadLine()?.Trim();

                if (s != null)
                {
                    if (s.Length == 0)
                        return DialogResult.None;

                    if (Definition.TryGetValue(s, out DialogResult dialogResult))
                    {
                        if (dialogResult == DialogResult.Cancel)
                            throw new OperationCanceledException();

                        return dialogResult;
                    }
                }
                else
                {
                    Console.WriteLine();
                    return DialogResult.None;
                }

                Console.WriteLine($"{Indent}Value '{s}' is invalid. Allowed values are: {Definition.LongHelp}");
            }
        }
    }
}
