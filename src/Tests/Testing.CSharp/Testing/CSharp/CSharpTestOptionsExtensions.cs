// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace Roslynator.Testing.CSharp;

public static class CSharpTestOptionsExtensions
{
    /// <summary>
    /// Sets config option to the test options.
    /// </summary>
    /// <param name="testOptions"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static CSharpTestOptions SetConfigOption(this CSharpTestOptions testOptions, string key, bool value)
    {
        return testOptions.SetConfigOption(key, (value) ? "true" : "false");
    }

    /// <summary>
    /// Sets config option to the test options.
    /// </summary>
    /// <param name="testOptions"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static CSharpTestOptions SetConfigOption(this CSharpTestOptions testOptions, string key, string value)
    {
        return testOptions.WithConfigOptions(testOptions.ConfigOptions.SetItem(key, value));
    }

    /// <summary>
    /// Sets config options to the test options.
    /// </summary>
    /// <param name="testOptions"></param>
    /// <param name="options"></param>
    public static CSharpTestOptions SetConfigOptions(this CSharpTestOptions testOptions, params (string Key, string Value)[] options)
    {
        return testOptions.SetConfigOptions(options.Select(f => new KeyValuePair<string, string>(f.Key, f.Value)));
    }

    /// <summary>
    /// Sets config options to the test options.
    /// </summary>
    /// <param name="testOptions"></param>
    /// <param name="options"></param>
    internal static CSharpTestOptions SetConfigOptions(this CSharpTestOptions testOptions, IEnumerable<KeyValuePair<string, string>> options)
    {
        return testOptions.WithConfigOptions(testOptions.ConfigOptions.SetItems(options));
    }

    /// <summary>
    /// Adds config option to the test options.
    /// </summary>
    /// <param name="testOptions"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static CSharpTestOptions AddConfigOption(this CSharpTestOptions testOptions, string key, bool value)
    {
        return testOptions.AddConfigOption(key, (value) ? "true" : "false");
    }

    /// <summary>
    /// Adds config option to the test options.
    /// </summary>
    /// <param name="testOptions"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static CSharpTestOptions AddConfigOption(this CSharpTestOptions testOptions, string key, string value)
    {
        return testOptions.WithConfigOptions(testOptions.ConfigOptions.Add(key, value));
    }

    /// <summary>
    /// Adds config options to the test options.
    /// </summary>
    /// <param name="testOptions"></param>
    /// <param name="options"></param>
    public static CSharpTestOptions AddConfigOptions(this CSharpTestOptions testOptions, params (string Key, string Value)[] options)
    {
        return testOptions.AddConfigOptions(options.Select(f => new KeyValuePair<string, string>(f.Key, f.Value)));
    }

    /// <summary>
    /// Adds config options to the test options.
    /// </summary>
    /// <param name="testOptions"></param>
    /// <param name="options"></param>
    internal static CSharpTestOptions AddConfigOptions(this CSharpTestOptions testOptions, IEnumerable<KeyValuePair<string, string>> options)
    {
        return testOptions.WithConfigOptions(testOptions.ConfigOptions.AddRange(options));
    }

    internal static CSharpTestOptions EnableConfigOption(this CSharpTestOptions testOptions, string key)
    {
        return testOptions.AddConfigOption(key, true);
    }
}
