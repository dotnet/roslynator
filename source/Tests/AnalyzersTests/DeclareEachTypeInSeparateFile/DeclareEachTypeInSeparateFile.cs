// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Tests
{
    public class ClassName
    {
    }

    public struct StructName
    {
    }

    public interface InterfaceName
    {
    }

    public enum EnumName
    {
    }

    public delegate void DelegateName();

    internal class DeclareEachTypeInSeparateFile
    {
        public class ClassName
        {
        }

        public struct StructName
        {
        }

        public interface InterfaceName
        {
        }

        public enum EnumName
        {
        }

        public delegate void DelegateName();
    }
}
