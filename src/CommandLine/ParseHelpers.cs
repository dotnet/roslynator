﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using static Roslynator.Logger;

namespace Roslynator.CommandLine;

internal static class ParseHelpers
{
    private static readonly Regex _lowerLetterUpperLetterRegex = new(@"\p{Ll}\p{Lu}");

    public static bool TryParseCodeExpression<T>(
        string expression,
        string filePath,
        string optionName1,
        string optionName2,
        string returnTypeName,
        Type returnType,
        string parameterTypeName,
        Type parameterType,
        string parameterName,
        out Func<ISymbol, T> func)
    {
        func = null;

        if (string.IsNullOrEmpty(expression))
        {
            if (string.IsNullOrEmpty(filePath))
            {
                WriteLine($"It is required to specify either '{optionName1}' or '{optionName2}'.", Verbosity.Quiet);
                return false;
            }

            if (!TryReadAllText(filePath, out filePath, ex => WriteCriticalError(ex)))
                return false;

            return DelegateFactory.TryCreateFromSourceText(filePath, returnType, parameterType, out func);
        }
        else if (!string.IsNullOrEmpty(filePath))
        {
            WriteLine($"It is not allowed to specify both '{optionName1}' and '{optionName2}'.", Verbosity.Quiet);
            return false;
        }

        return DelegateFactory.TryCreateFromExpression(
            expression,
            "C",
            "M",
            returnTypeName,
            returnType,
            parameterTypeName,
            parameterType,
            parameterName,
            out func);
    }

    public static bool TryParseMSBuildProperties(IEnumerable<string> values, out Dictionary<string, string> properties)
    {
        properties = null;

        foreach (string property in values)
        {
            int index = property.IndexOf("=");

            if (index == -1)
            {
                WriteLine($"Unable to parse property '{property}'", Verbosity.Quiet);
                return false;
            }

            string key = property.Substring(0, index);

            if (properties is null)
                properties = new Dictionary<string, string>();

            properties[key] = property.Substring(index + 1);
        }

        if (properties?.Count > 0)
        {
            int maxLength = properties.Max(f => f.Key.Length);

            foreach (KeyValuePair<string, string> kvp in properties)
                WriteLine($"Add MSBuild property {kvp.Key.PadRight(maxLength)} = {kvp.Value}", ConsoleColors.DarkGray, Verbosity.Detailed);
        }

        return true;
    }

    public static bool TryParseKeyValuePairs(IEnumerable<string> values, out List<KeyValuePair<string, string>> properties)
    {
        properties = null;

        foreach (string property in values)
        {
            int index = property.IndexOf("=");

            if (index == -1)
            {
                WriteLine($"Unable to parse key/value pair '{property}'", Verbosity.Quiet);
                return false;
            }

            string key = property.Substring(0, index);
            string value = property.Substring(index + 1);

            (properties ??= new List<KeyValuePair<string, string>>()).Add(new KeyValuePair<string, string>(key, value));
        }

        return true;
    }

    public static bool TryParseOptionValueAsEnumFlags<TEnum>(
        IEnumerable<string> values,
        string optionName,
        out TEnum result,
        TEnum? defaultValue = null) where TEnum : struct
    {
        result = (TEnum)(object)0;

        if (values?.Any() != true)
        {
            if (defaultValue is not null)
            {
                result = (TEnum)(object)defaultValue;
            }

            return true;
        }

        int flags = 0;

        foreach (string value in values)
        {
            if (!TryParseOptionValueAsEnum(value, optionName, out TEnum result2))
                return false;

            flags |= (int)(object)result2;
        }

        result = (TEnum)(object)flags;

        return true;
    }

    public static bool TryParseOptionValueAsEnumValues<TEnum>(
        IEnumerable<string> values,
        string optionName,
        out ImmutableArray<TEnum> result,
        ImmutableArray<TEnum> defaultValue = default) where TEnum : struct
    {
        if (values?.Any() != true)
        {
            result = (defaultValue.IsDefault) ? ImmutableArray<TEnum>.Empty : defaultValue;

            return true;
        }

        ImmutableArray<TEnum>.Builder builder = ImmutableArray.CreateBuilder<TEnum>();

        foreach (string value in values)
        {
            if (!TryParseOptionValueAsEnum(value, optionName, out TEnum result2))
            {
                result = default;
                return false;
            }

            builder.Add(result2);
        }

        result = builder.ToImmutableArray();

        return true;
    }

    public static bool TryParseOptionValueAsEnum<TEnum>(string value, string optionName, out TEnum result, TEnum? defaultValue = null) where TEnum : struct
    {
        if (value is null
            && defaultValue is not null)
        {
            result = defaultValue.Value;
            return true;
        }

        if (!Enum.TryParse(value?.Replace("-", ""), ignoreCase: true, out result))
        {
            IEnumerable<string> values = Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(f => _lowerLetterUpperLetterRegex.Replace(f.ToString(), e => e.Value.Insert(1, "-")).ToLowerInvariant());

            WriteLine($"Option '--{optionName}' has unknown value '{value}'. Known values: {string.Join(", ", values)}.", Verbosity.Quiet);
            return false;
        }

        return true;
    }

    public static bool TryParseVerbosity(string value, out Verbosity verbosity)
    {
        switch (value)
        {
            case "q":
            {
                verbosity = Verbosity.Quiet;
                return true;
            }
            case "m":
            {
                verbosity = Verbosity.Minimal;
                return true;
            }
            case "n":
            {
                verbosity = Verbosity.Normal;
                return true;
            }
            case "d":
            {
                verbosity = Verbosity.Detailed;
                return true;
            }
            case "diag":
            {
                verbosity = Verbosity.Diagnostic;
                return true;
            }
        }

        if (Enum.TryParse(value, ignoreCase: true, out verbosity))
            return true;

        WriteLine($"Unknown verbosity '{value}'.", Verbosity.Quiet);
        return false;
    }

    public static bool TryParseLanguage(string value, out string language)
    {
        switch (value)
        {
            case "cs":
            case "csharp":
            {
                language = LanguageNames.CSharp;
                return true;
            }
            case "vb":
            case "visual-basic":
            {
                language = LanguageNames.VisualBasic;
                return true;
            }
        }

        WriteLine($"Unknown language '{value}'.", Verbosity.Quiet);

        language = null;
        return false;
    }

    public static bool TryParseMetadataNames(IEnumerable<string> values, out ImmutableArray<MetadataName> metadataNames)
    {
        ImmutableArray<MetadataName>.Builder builder = null;

        foreach (string value in values)
        {
            if (!MetadataName.TryParse(value, out MetadataName metadataName))
            {
                WriteLine($"Unable to parse metadata name '{value}'.", Verbosity.Quiet);
                metadataNames = default;
                return false;
            }

            (builder ??= ImmutableArray.CreateBuilder<MetadataName>()).Add(metadataName);
        }

        metadataNames = builder?.ToImmutableArray() ?? ImmutableArray<MetadataName>.Empty;

        return true;
    }

    public static bool TryParseVersion(string value, out Version version)
    {
        if (!Version.TryParse(value, out version))
        {
            WriteLine($"Could not parse '{value}' as version.", Verbosity.Quiet);
            return false;
        }

        return true;
    }

    public static bool TryEnsureFullPath(IEnumerable<string> paths, out ImmutableArray<string> fullPaths)
    {
        ImmutableArray<string>.Builder builder = ImmutableArray.CreateBuilder<string>();

        foreach (string path in paths)
        {
            if (!TryEnsureFullPath(path, out string fullPath))
            {
                fullPaths = default;
                return false;
            }

            builder.Add(fullPath);
        }

        fullPaths = builder.ToImmutableArray();
        return true;
    }

    public static bool TryEnsureFullPath(string path, out string result)
    {
        try
        {
            result = Path.GetFullPath(path);
            return true;
        }
        catch (ArgumentException ex)
        {
            WriteLine($"Path '{path}' is invalid: {ex.Message}.", Verbosity.Quiet);
            result = null;
            return false;
        }
    }

    public static bool TryReadAllText(
        string path,
        out string content,
        Action<Exception> exceptionHandler = null)
    {
        try
        {
            content = File.ReadAllText(path);
            return true;
        }
        catch (Exception ex) when (ex is ArgumentException
            || ex is IOException
            || ex is UnauthorizedAccessException)
        {
            exceptionHandler?.Invoke(ex);
            content = null;
            return false;
        }
    }

    public static bool TryParseOutputFormat(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            // Default to XML if no value is provided
            return true;
        }

        bool valid = value.Trim().ToLowerInvariant() switch
        {
            "gitlab" => true,
            "xml" => true,
            _ => false
        };

        if (!valid)
        {
            WriteLine($"Unknown output format '{value}'.", Verbosity.Quiet);
        }

        return valid;
    }
}
