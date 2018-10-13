// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Xunit;

namespace Roslynator.CSharp.Tests
{
    public static class MetadataNameTests
    {
        [Fact]
        public static void TestParse1()
        {
            const string x = "a";

            MetadataName name = MetadataName.Parse(x);

            Assert.Equal(x, name.ToString());
        }

        [Fact]
        public static void TestParse2()
        {
            const string x = "a.b";

            MetadataName name = MetadataName.Parse(x);

            Assert.Equal(x, name.ToString());
        }

        [Fact]
        public static void TestParse3()
        {
            const string x = "a.b.c";

            MetadataName name = MetadataName.Parse(x);

            Assert.Equal(x, name.ToString());
        }

        [Fact]
        public static void TestParse4()
        {
            const string x = "a+b";

            MetadataName name = MetadataName.Parse(x);

            Assert.Equal(x, name.ToString());
        }

        [Fact]
        public static void TestParse5()
        {
            const string x = "a.b+c+d";

            MetadataName name = MetadataName.Parse(x);

            Assert.Equal(x, name.ToString());
        }

        [Fact]
        public static void TestParse6()
        {
            const string x = "a.b.c+d+e+f";

            MetadataName name = MetadataName.Parse(x);

            Assert.Equal(x, name.ToString());
        }
    }
}
