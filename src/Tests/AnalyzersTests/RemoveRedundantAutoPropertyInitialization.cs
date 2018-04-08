// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#pragma warning disable RCS1170

namespace Roslynator.CSharp.Analyzers.Tests
{
    public class RemoveRedundantAutoPropertyInitialization
    {
        private const bool BoolConst = false;
        private const char CharConst = '\0';
        private const int IntConst = 0;
        private const ulong ULongConst = 0;
        private const RegexOptions RegexOptionsConst = RegexOptions.None;
        private const string StringConst = null;

        private bool Pf2 { get; set; } = false;
        private bool Pf3 { get; set; } = BoolConst;
        private char Pch2 { get; set; } = '\0';
        private char Pch3 { get; set; } = CharConst;
        private int Pi2 { get; set; } = 0;
        private int Pi3 { get; set; } = IntConst;
        private ulong Pl2 { get; set; } = 0;
        private ulong Pl3 { get; set; } = ULongConst;
        private RegexOptions Pro2 { get; set; } = 0;
        private RegexOptions Pro3 { get; set; } = RegexOptions.None;
        private RegexOptions Pro4 { get; set; } = (RegexOptions)0;
        private string Ps2 { get; set; } = null;
        private string Ps3 { get; set; } = default(string);
        private string Ps4 { get; set; } = StringConst;
        private bool? Pn2 { get; set; } = null;
        private bool? Pn3 { get; set; } = default(bool?);
        private sbyte Psb { get; set; } = (sbyte)0;
        private byte Pbe { get; set; } = (byte)0;
        private short Pst { get; set; } = (short)0;
        private ushort Pus { get; set; } = (ushort)0;
        private int Pii { get; set; } = (int)0;
        private uint Pui { get; set; } = (uint)0;
        private long Plg { get; set; } = (long)0;
        private ulong Pul { get; set; } = (ulong)0;
        private float Pft { get; set; } = (float)0;
        private double Pde { get; set; } = (double)0;
        private decimal Pdl { get; set; } = (decimal)0;
    }
}
