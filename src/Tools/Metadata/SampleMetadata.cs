// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;

namespace Roslynator.Metadata;

public readonly record struct SampleMetadata(string Before, string After, ImmutableArray<(string Key, string Value)> ConfigOptions);
