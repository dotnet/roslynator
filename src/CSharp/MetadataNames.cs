// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;

namespace Roslynator
{
    internal static class MetadataNames
    {
        public static readonly MetadataName System_ArgumentException = new MetadataName(Namespaces.System, "ArgumentException");
        public static readonly MetadataName System_ArgumentNullException = new MetadataName(Namespaces.System, "ArgumentNullException");
        public static readonly MetadataName System_Collections_Generic = new MetadataName(Namespaces.System_Collections, "Generic");
        public static readonly MetadataName System_Collections_Generic_List_T = new MetadataName(Namespaces.System_Collections_Generic, "List`1");
        public static readonly MetadataName System_Collections_IDictionary = new MetadataName(Namespaces.System_Collections, "IDictionary");
        public static readonly MetadataName System_Collections_Immutable_ImmutableArray_T = new MetadataName(Namespaces.System_Collections_Immutable, "ImmutableArray`1");
        public static readonly MetadataName System_ComponentModel_INotifyPropertyChanged = new MetadataName(Namespaces.System_ComponentModel, "INotifyPropertyChanged");
        public static readonly MetadataName System_Diagnostics_Debug = new MetadataName(Namespaces.System_Diagnostics, "Debug");
        public static readonly MetadataName System_Diagnostics_DebuggerDisplayAttribute = new MetadataName(Namespaces.System_Diagnostics, "DebuggerDisplayAttribute");
        public static readonly MetadataName System_Enum = new MetadataName(Namespaces.System, "Enum");
        public static readonly MetadataName System_EventArgs = new MetadataName(Namespaces.System, "EventArgs");
        public static readonly MetadataName System_EventHandler = new MetadataName(Namespaces.System, "EventHandler");
        public static readonly MetadataName System_FlagsAttribute = new MetadataName(Namespaces.System, "FlagsAttribute");
        public static readonly MetadataName System_Func_T2 = new MetadataName(Namespaces.System, "Func`2");
        public static readonly MetadataName System_Func_T3 = new MetadataName(Namespaces.System, "Func`3");
        public static readonly MetadataName System_IEquatable_T = new MetadataName(Namespaces.System, "IEquatable`1");
        public static readonly MetadataName System_Linq_Enumerable = new MetadataName(Namespaces.System_Linq, "Enumerable");
        public static readonly MetadataName System_Linq_Expressions_Expression_T = new MetadataName(Namespaces.System_Linq_Expressions, "Expression`1");
        public static readonly MetadataName System_Linq_ImmutableArrayExtensions = new MetadataName(Namespaces.System_Linq, "ImmutableArrayExtensions");
        public static readonly MetadataName System_Linq_IOrderedEnumerable_T = new MetadataName(Namespaces.System_Linq, "IOrderedEnumerable`1");
        public static readonly MetadataName System_NonSerializedAttribute = new MetadataName(Namespaces.System, "NonSerializedAttribute");
        public static readonly MetadataName System_ObsoleteAttribute = new MetadataName(Namespaces.System, "ObsoleteAttribute");
        public static readonly MetadataName System_Runtime_CompilerServices_ConfiguredTaskAwaitable = new MetadataName(Namespaces.System_Runtime_CompilerServices, "ConfiguredTaskAwaitable");
        public static readonly MetadataName System_Runtime_CompilerServices_ConfiguredTaskAwaitable_T = new MetadataName(Namespaces.System_Runtime_CompilerServices, "ConfiguredTaskAwaitable`1");
        public static readonly MetadataName System_Runtime_InteropServices_LayoutKind = new MetadataName(Namespaces.System_Runtime_InteropServices, "LayoutKind");
        public static readonly MetadataName System_Runtime_InteropServices_StructLayoutAttribute = new MetadataName(Namespaces.System_Runtime_InteropServices, "StructLayoutAttribute");
        public static readonly MetadataName System_Runtime_Serialization_DataMemberAttribute = new MetadataName(Namespaces.System_Runtime_Serialization, "DataMemberAttribute");
        public static readonly MetadataName System_Runtime_Serialization_SerializationInfo = new MetadataName(Namespaces.System_Runtime_Serialization, "SerializationInfo");
        public static readonly MetadataName System_Runtime_Serialization_StreamingContext = new MetadataName(Namespaces.System_Runtime_Serialization, "StreamingContext");
        public static readonly MetadataName System_Text_RegularExpressions_Regex = new MetadataName(Namespaces.System_Text_RegularExpressions, "Regex");
        public static readonly MetadataName System_Text_RegularExpressions_RegexOptions = new MetadataName(Namespaces.System_Text_RegularExpressions, "RegexOptions");
        public static readonly MetadataName System_Text_StringBuilder = new MetadataName(Namespaces.System_Text, "StringBuilder");
        public static readonly MetadataName System_Threading_Tasks = new MetadataName(Namespaces.System_Threading, "Tasks");
        public static readonly MetadataName System_Threading_Tasks_Task = new MetadataName(Namespaces.System_Threading_Tasks, "Task");
        public static readonly MetadataName System_Threading_Tasks_Task_T = new MetadataName(Namespaces.System_Threading_Tasks, "Task`1");
        public static readonly MetadataName System_Threading_Tasks_ValueTask_T = new MetadataName(Namespaces.System_Threading_Tasks, "ValueTask`1");
        public static readonly MetadataName System_TimeSpan = new MetadataName(Namespaces.System, "TimeSpan");

        private static class Namespaces
        {
            public static readonly ImmutableArray<string> System = ImmutableArray.Create("System");
            public static readonly ImmutableArray<string> System_Collections = ImmutableArray.Create("System", "Collections");
            public static readonly ImmutableArray<string> System_Collections_Generic = ImmutableArray.Create("System", "Collections", "Generic");
            public static readonly ImmutableArray<string> System_Collections_Immutable = ImmutableArray.Create("System", "Collections", "Immutable");
            public static readonly ImmutableArray<string> System_ComponentModel = ImmutableArray.Create("System", "ComponentModel");
            public static readonly ImmutableArray<string> System_Diagnostics = ImmutableArray.Create("System", "Diagnostics");
            public static readonly ImmutableArray<string> System_Linq = ImmutableArray.Create("System", "Linq");
            public static readonly ImmutableArray<string> System_Linq_Expressions = ImmutableArray.Create("System", "Linq", "Expressions");
            public static readonly ImmutableArray<string> System_Runtime_CompilerServices = ImmutableArray.Create("System", "Runtime", "CompilerServices");
            public static readonly ImmutableArray<string> System_Runtime_InteropServices = ImmutableArray.Create("System", "Runtime", "InteropServices");
            public static readonly ImmutableArray<string> System_Runtime_Serialization = ImmutableArray.Create("System", "Runtime", "Serialization");
            public static readonly ImmutableArray<string> System_Text = ImmutableArray.Create("System", "Text");
            public static readonly ImmutableArray<string> System_Text_RegularExpressions = ImmutableArray.Create("System", "Text", "RegularExpressions");
            public static readonly ImmutableArray<string> System_Threading = ImmutableArray.Create("System", "Threading");
            public static readonly ImmutableArray<string> System_Threading_Tasks = ImmutableArray.Create("System", "Threading", "Tasks");
        }
    }
}
