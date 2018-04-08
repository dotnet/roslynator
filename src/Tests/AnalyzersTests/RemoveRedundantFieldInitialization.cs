// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#pragma warning disable RCS1081

namespace Roslynator.CSharp.Analyzers.Tests
{
    public class RemoveRedundantFieldInitialization
    {
        private const bool BoolConst = false;
        private const char CharConst = '\0';
        private const int IntConst = 0;
        private const ulong ULongConst = 0;
        private const RegexOptions RegexOptionsConst = RegexOptions.None;
        private const string StringConst = null;

        private object _f = null, _ff;

        private bool _f2 = false;
        private bool _f3 = BoolConst;
        private char _ch2 = '\0';
        private char _ch3 = CharConst;
        private int _i2 = 0;
        private int _i3 = IntConst;
        private ulong _l2 = 0;
        private ulong _l3 = ULongConst;
        private RegexOptions _ro2 = 0;
        private RegexOptions _ro3 = RegexOptions.None;
        private RegexOptions _ro4 = (RegexOptions)0;
        private string _s2 = null;
        private string _s3 = default(string);
        private string _s4 = StringConst;
        private bool? _n2 = null;
        private bool? _n3 = default(bool?);
        private sbyte _sb = (sbyte)0;
        private byte _be = (byte)0;
        private short _st = (short)0;
        private ushort _us = (ushort)0;
        private int _ii = (int)0;
        private uint _ui = (uint)0;
        private long _lg = (long)0;
        private ulong _ul = (ulong)0;
        private float _ft = (float)0;
        private double _de = (double)0;
        private decimal _dl = (decimal)0;
    }
}
