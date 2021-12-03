// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Text;

namespace Roslynator.Text.RegularExpressions
{
    internal static class RegexEscape
    {
        public static string Escape(string input, bool inCharGroup = false)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            for (int i = 0; i < input.Length; i++)
            {
                CharEscapeMode mode = GetEscapeMode(input[i], inCharGroup);

                if (mode != CharEscapeMode.None)
                {
                    var sb = new StringBuilder();
                    char ch = input[i];
                    int lastPos;
                    sb.Append(input, 0, i);

                    do
                    {
                        switch (mode)
                        {
                            case CharEscapeMode.AsciiHexadecimal:
                                {
                                    sb.Append(@"\x");
                                    sb.Append(((int)ch).ToString("X2", CultureInfo.InvariantCulture));
                                    break;
                                }
                            case CharEscapeMode.Backslash:
                                {
                                    sb.Append('\\');
                                    sb.Append(ch);
                                    break;
                                }
                            case CharEscapeMode.Bell:
                                {
                                    sb.Append(@"\a");
                                    break;
                                }
                            case CharEscapeMode.CarriageReturn:
                                {
                                    sb.Append(@"\r");
                                    break;
                                }
                            case CharEscapeMode.Escape:
                                {
                                    sb.Append(@"\e");
                                    break;
                                }
                            case CharEscapeMode.FormFeed:
                                {
                                    sb.Append(@"\f");
                                    break;
                                }
                            case CharEscapeMode.Linefeed:
                                {
                                    sb.Append(@"\n");
                                    break;
                                }
                            case CharEscapeMode.VerticalTab:
                                {
                                    sb.Append(@"\v");
                                    break;
                                }
                            case CharEscapeMode.Tab:
                                {
                                    sb.Append(@"\t");
                                    break;
                                }
                        }

                        i++;
                        lastPos = i;

                        while (i < input.Length)
                        {
                            ch = input[i];
                            mode = GetEscapeMode(ch, inCharGroup);

                            if (mode != CharEscapeMode.None)
                                break;

                            i++;
                        }

                        sb.Append(input, lastPos, i - lastPos);

                    } while (i < input.Length);

                    return sb.ToString();
                }
            }

            return input;
        }

        public static string Escape(char value, bool inCharGroup = false)
        {
            return (GetEscapeMode(value, inCharGroup)) switch
            {
                CharEscapeMode.None => value.ToString(),
                CharEscapeMode.AsciiHexadecimal => @"\x" + ((int)value).ToString("X2", CultureInfo.InvariantCulture),
                CharEscapeMode.Backslash => @"\" + value.ToString(),
                CharEscapeMode.Bell => @"\a",
                CharEscapeMode.CarriageReturn => @"\r",
                CharEscapeMode.Escape => @"\e",
                CharEscapeMode.FormFeed => @"\f",
                CharEscapeMode.Linefeed => @"\n",
                CharEscapeMode.Tab => @"\t",
                CharEscapeMode.VerticalTab => @"\v",
                _ => throw new InvalidOperationException(),
            };
        }

        internal static CharEscapeMode GetEscapeMode(int charCode, bool inCharGroup)
        {
            return (charCode > 0xFF)
                ? CharEscapeMode.None
                : ((inCharGroup) ? _charGroupEscapeModes[charCode] : _escapeModes[charCode]);
        }

        public static string EscapeSubstitution(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '$')
                {
                    var sb = new StringBuilder();

                    char ch;
                    int lastPos;
                    sb.Append(input, 0, i);

                    do
                    {
                        sb.Append("$$");
                        i++;
                        lastPos = i;

                        while (i < input.Length)
                        {
                            ch = input[i];

                            if (ch == '$')
                                break;

                            i++;
                        }

                        sb.Append(input, lastPos, i - lastPos);

                    } while (i < input.Length);

                    return sb.ToString();
                }
            }

            return input;
        }

        internal static string ConvertCharacterEscapes(string replacement)
        {
            for (int i = 0; i < replacement.Length; i++)
            {
                if (IsChar(i))
                {
                    var sb = new StringBuilder();

                    int lastPos;
                    sb.Append(replacement, 0, i);

                    do
                    {
                        i++;
                        sb.Append(GetChar(replacement[i]));

                        i++;
                        lastPos = i;

                        while (i < replacement.Length)
                        {
                            if (IsChar(i))
                                break;

                            i++;
                        }

                        sb.Append(replacement, lastPos, i - lastPos);

                    } while (i < replacement.Length);

                    return sb.ToString();
                }
            }

            return replacement;

            bool IsChar(int i)
            {
                if (replacement[i] == '\\'
                    && i < replacement.Length - 1)
                {
                    switch (replacement[i + 1])
                    {
                        case 'a':
                        case 'b':
                        case 'f':
                        case 'n':
                        case 'r':
                        case 't':
                        case 'v':
                            return true;
                    }
                }

                return false;
            }

            static char GetChar(char ch)
            {
                return ch switch
                {
                    'a' => '\a',
                    'b' => '\b',
                    'f' => '\f',
                    'n' => '\n',
                    'r' => '\r',
                    't' => '\t',
                    'v' => '\v',
                    _ => throw new InvalidOperationException(),
                };
            }
        }

        private static readonly CharEscapeMode[] _escapeModes = new[] {
            // 0 0x00
            CharEscapeMode.AsciiHexadecimal,
            // 1 0x01
            CharEscapeMode.AsciiHexadecimal,
            // 2 0x02
            CharEscapeMode.AsciiHexadecimal,
            // 3 0x03
            CharEscapeMode.AsciiHexadecimal,
            // 4 0x04
            CharEscapeMode.AsciiHexadecimal,
            // 5 0x05
            CharEscapeMode.AsciiHexadecimal,
            // 6 0x06
            CharEscapeMode.AsciiHexadecimal,
            // 7 0x07
            CharEscapeMode.Bell,
            // 8 0x08
            CharEscapeMode.AsciiHexadecimal,
            // 9 0x09
            CharEscapeMode.Tab,
            // 10 0x0A
            CharEscapeMode.Linefeed,
            // 11 0x0B
            CharEscapeMode.VerticalTab,
            // 12 0x0C
            CharEscapeMode.FormFeed,
            // 13 0x0D
            CharEscapeMode.CarriageReturn,
            // 14 0x0E
            CharEscapeMode.AsciiHexadecimal,
            // 15 0x0F
            CharEscapeMode.AsciiHexadecimal,
            // 16 0x10
            CharEscapeMode.AsciiHexadecimal,
            // 17 0x11
            CharEscapeMode.AsciiHexadecimal,
            // 18 0x12
            CharEscapeMode.AsciiHexadecimal,
            // 19 0x13
            CharEscapeMode.AsciiHexadecimal,
            // 20 0x14
            CharEscapeMode.AsciiHexadecimal,
            // 21 0x15
            CharEscapeMode.AsciiHexadecimal,
            // 22 0x16
            CharEscapeMode.AsciiHexadecimal,
            // 23 0x17
            CharEscapeMode.AsciiHexadecimal,
            // 24 0x18
            CharEscapeMode.AsciiHexadecimal,
            // 25 0x19
            CharEscapeMode.AsciiHexadecimal,
            // 26 0x1A
            CharEscapeMode.AsciiHexadecimal,
            // 27 0x1B
            CharEscapeMode.Escape,
            // 28 0x1C
            CharEscapeMode.AsciiHexadecimal,
            // 29 0x1D
            CharEscapeMode.AsciiHexadecimal,
            // 30 0x1E
            CharEscapeMode.AsciiHexadecimal,
            // 31 0x1F
            CharEscapeMode.AsciiHexadecimal,
            // 32 0x20
            CharEscapeMode.Backslash,
            // 33 0x21 !
            CharEscapeMode.None,
            // 34 0x22 "
            CharEscapeMode.None,
            // 35 0x23 #
            CharEscapeMode.Backslash,
            // 36 0x24 $
            CharEscapeMode.Backslash,
            // 37 0x25 %
            CharEscapeMode.None,
            // 38 0x26 &
            CharEscapeMode.None,
            // 39 0x27 '
            CharEscapeMode.None,
            // 40 0x28 (
            CharEscapeMode.Backslash,
            // 41 0x29 )
            CharEscapeMode.Backslash,
            // 42 0x2A *
            CharEscapeMode.Backslash,
            // 43 0x2B +
            CharEscapeMode.Backslash,
            // 44 0x2C ,
            CharEscapeMode.None,
            // 45 0x2D -
            CharEscapeMode.None,
            // 46 0x2E .
            CharEscapeMode.Backslash,
            // 47 0x2F /
            CharEscapeMode.None,
            // 48 0x30 0
            CharEscapeMode.None,
            // 49 0x31 1
            CharEscapeMode.None,
            // 50 0x32 2
            CharEscapeMode.None,
            // 51 0x33 3
            CharEscapeMode.None,
            // 52 0x34 4
            CharEscapeMode.None,
            // 53 0x35 5
            CharEscapeMode.None,
            // 54 0x36 6
            CharEscapeMode.None,
            // 55 0x37 7
            CharEscapeMode.None,
            // 56 0x38 8
            CharEscapeMode.None,
            // 57 0x39 9
            CharEscapeMode.None,
            // 58 0x3A :
            CharEscapeMode.None,
            // 59 0x3B ;
            CharEscapeMode.None,
            // 60 0x3C <
            CharEscapeMode.None,
            // 61 0x3D =
            CharEscapeMode.None,
            // 62 0x3E >
            CharEscapeMode.None,
            // 63 0x3F ?
            CharEscapeMode.Backslash,
            // 64 0x40 @
            CharEscapeMode.None,
            // 65 0x41 A
            CharEscapeMode.None,
            // 66 0x42 B
            CharEscapeMode.None,
            // 67 0x43 C
            CharEscapeMode.None,
            // 68 0x44 D
            CharEscapeMode.None,
            // 69 0x45 E
            CharEscapeMode.None,
            // 70 0x46 F
            CharEscapeMode.None,
            // 71 0x47 G
            CharEscapeMode.None,
            // 72 0x48 H
            CharEscapeMode.None,
            // 73 0x49 I
            CharEscapeMode.None,
            // 74 0x4A J
            CharEscapeMode.None,
            // 75 0x4B K
            CharEscapeMode.None,
            // 76 0x4C L
            CharEscapeMode.None,
            // 77 0x4D M
            CharEscapeMode.None,
            // 78 0x4E N
            CharEscapeMode.None,
            // 79 0x4F O
            CharEscapeMode.None,
            // 80 0x50 P
            CharEscapeMode.None,
            // 81 0x51 Q
            CharEscapeMode.None,
            // 82 0x52 R
            CharEscapeMode.None,
            // 83 0x53 S
            CharEscapeMode.None,
            // 84 0x54 T
            CharEscapeMode.None,
            // 85 0x55 U
            CharEscapeMode.None,
            // 86 0x56 V
            CharEscapeMode.None,
            // 87 0x57 W
            CharEscapeMode.None,
            // 88 0x58 X
            CharEscapeMode.None,
            // 89 0x59 Y
            CharEscapeMode.None,
            // 90 0x5A Z
            CharEscapeMode.None,
            // 91 0x5B [
            CharEscapeMode.Backslash,
            // 92 0x5C \
            CharEscapeMode.Backslash,
            // 93 0x5D ]
            CharEscapeMode.None,
            // 94 0x5E ^
            CharEscapeMode.Backslash,
            // 95 0x5F _
            CharEscapeMode.None,
            // 96 0x60 `
            CharEscapeMode.None,
            // 97 0x61 a
            CharEscapeMode.None,
            // 98 0x62 b
            CharEscapeMode.None,
            // 99 0x63 c
            CharEscapeMode.None,
            // 100 0x64 d
            CharEscapeMode.None,
            // 101 0x65 e
            CharEscapeMode.None,
            // 102 0x66 f
            CharEscapeMode.None,
            // 103 0x67 g
            CharEscapeMode.None,
            // 104 0x68 h
            CharEscapeMode.None,
            // 105 0x69 i
            CharEscapeMode.None,
            // 106 0x6A j
            CharEscapeMode.None,
            // 107 0x6B k
            CharEscapeMode.None,
            // 108 0x6C l
            CharEscapeMode.None,
            // 109 0x6D m
            CharEscapeMode.None,
            // 110 0x6E n
            CharEscapeMode.None,
            // 111 0x6F o
            CharEscapeMode.None,
            // 112 0x70 p
            CharEscapeMode.None,
            // 113 0x71 q
            CharEscapeMode.None,
            // 114 0x72 r
            CharEscapeMode.None,
            // 115 0x73 s
            CharEscapeMode.None,
            // 116 0x74 t
            CharEscapeMode.None,
            // 117 0x75 u
            CharEscapeMode.None,
            // 118 0x76 v
            CharEscapeMode.None,
            // 119 0x77 w
            CharEscapeMode.None,
            // 120 0x78 x
            CharEscapeMode.None,
            // 121 0x79 y
            CharEscapeMode.None,
            // 122 0x7A z
            CharEscapeMode.None,
            // 123 0x7B {
            CharEscapeMode.Backslash,
            // 124 0x7C |
            CharEscapeMode.Backslash,
            // 125 0x7D }
            CharEscapeMode.None,
            // 126 0x7E ~
            CharEscapeMode.None,
            // 127 0x7F
            CharEscapeMode.AsciiHexadecimal,
            // 128 0x80
            CharEscapeMode.AsciiHexadecimal,
            // 129 0x81
            CharEscapeMode.AsciiHexadecimal,
            // 130 0x82
            CharEscapeMode.AsciiHexadecimal,
            // 131 0x83
            CharEscapeMode.AsciiHexadecimal,
            // 132 0x84
            CharEscapeMode.AsciiHexadecimal,
            // 133 0x85
            CharEscapeMode.AsciiHexadecimal,
            // 134 0x86
            CharEscapeMode.AsciiHexadecimal,
            // 135 0x87
            CharEscapeMode.AsciiHexadecimal,
            // 136 0x88
            CharEscapeMode.AsciiHexadecimal,
            // 137 0x89
            CharEscapeMode.AsciiHexadecimal,
            // 138 0x8A
            CharEscapeMode.AsciiHexadecimal,
            // 139 0x8B
            CharEscapeMode.AsciiHexadecimal,
            // 140 0x8C
            CharEscapeMode.AsciiHexadecimal,
            // 141 0x8D
            CharEscapeMode.AsciiHexadecimal,
            // 142 0x8E
            CharEscapeMode.AsciiHexadecimal,
            // 143 0x8F
            CharEscapeMode.AsciiHexadecimal,
            // 144 0x90
            CharEscapeMode.AsciiHexadecimal,
            // 145 0x91
            CharEscapeMode.AsciiHexadecimal,
            // 146 0x92
            CharEscapeMode.AsciiHexadecimal,
            // 147 0x93
            CharEscapeMode.AsciiHexadecimal,
            // 148 0x94
            CharEscapeMode.AsciiHexadecimal,
            // 149 0x95
            CharEscapeMode.AsciiHexadecimal,
            // 150 0x96
            CharEscapeMode.AsciiHexadecimal,
            // 151 0x97
            CharEscapeMode.AsciiHexadecimal,
            // 152 0x98
            CharEscapeMode.AsciiHexadecimal,
            // 153 0x99
            CharEscapeMode.AsciiHexadecimal,
            // 154 0x9A
            CharEscapeMode.AsciiHexadecimal,
            // 155 0x9B
            CharEscapeMode.AsciiHexadecimal,
            // 156 0x9C
            CharEscapeMode.AsciiHexadecimal,
            // 157 0x9D
            CharEscapeMode.AsciiHexadecimal,
            // 158 0x9E
            CharEscapeMode.AsciiHexadecimal,
            // 159 0x9F
            CharEscapeMode.AsciiHexadecimal,
            // 160 0xA0
            CharEscapeMode.None,
            // 161 0xA1 ¡
            CharEscapeMode.None,
            // 162 0xA2 ¢
            CharEscapeMode.None,
            // 163 0xA3 £
            CharEscapeMode.None,
            // 164 0xA4 ¤
            CharEscapeMode.None,
            // 165 0xA5 ¥
            CharEscapeMode.None,
            // 166 0xA6 ¦
            CharEscapeMode.None,
            // 167 0xA7 §
            CharEscapeMode.None,
            // 168 0xA8 ¨
            CharEscapeMode.None,
            // 169 0xA9 ©
            CharEscapeMode.None,
            // 170 0xAA ª
            CharEscapeMode.None,
            // 171 0xAB «
            CharEscapeMode.None,
            // 172 0xAC ¬
            CharEscapeMode.None,
            // 173 0xAD ­
            CharEscapeMode.None,
            // 174 0xAE ®
            CharEscapeMode.None,
            // 175 0xAF ¯
            CharEscapeMode.None,
            // 176 0xB0 °
            CharEscapeMode.None,
            // 177 0xB1 ±
            CharEscapeMode.None,
            // 178 0xB2 ²
            CharEscapeMode.None,
            // 179 0xB3 ³
            CharEscapeMode.None,
            // 180 0xB4 ´
            CharEscapeMode.None,
            // 181 0xB5 µ
            CharEscapeMode.None,
            // 182 0xB6 ¶
            CharEscapeMode.None,
            // 183 0xB7 ·
            CharEscapeMode.None,
            // 184 0xB8 ¸
            CharEscapeMode.None,
            // 185 0xB9 ¹
            CharEscapeMode.None,
            // 186 0xBA º
            CharEscapeMode.None,
            // 187 0xBB »
            CharEscapeMode.None,
            // 188 0xBC ¼
            CharEscapeMode.None,
            // 189 0xBD ½
            CharEscapeMode.None,
            // 190 0xBE ¾
            CharEscapeMode.None,
            // 191 0xBF ¿
            CharEscapeMode.None,
            // 192 0xC0 À
            CharEscapeMode.None,
            // 193 0xC1 Á
            CharEscapeMode.None,
            // 194 0xC2 Â
            CharEscapeMode.None,
            // 195 0xC3 Ã
            CharEscapeMode.None,
            // 196 0xC4 Ä
            CharEscapeMode.None,
            // 197 0xC5 Å
            CharEscapeMode.None,
            // 198 0xC6 Æ
            CharEscapeMode.None,
            // 199 0xC7 Ç
            CharEscapeMode.None,
            // 200 0xC8 È
            CharEscapeMode.None,
            // 201 0xC9 É
            CharEscapeMode.None,
            // 202 0xCA Ê
            CharEscapeMode.None,
            // 203 0xCB Ë
            CharEscapeMode.None,
            // 204 0xCC Ì
            CharEscapeMode.None,
            // 205 0xCD Í
            CharEscapeMode.None,
            // 206 0xCE Î
            CharEscapeMode.None,
            // 207 0xCF Ï
            CharEscapeMode.None,
            // 208 0xD0 Ð
            CharEscapeMode.None,
            // 209 0xD1 Ñ
            CharEscapeMode.None,
            // 210 0xD2 Ò
            CharEscapeMode.None,
            // 211 0xD3 Ó
            CharEscapeMode.None,
            // 212 0xD4 Ô
            CharEscapeMode.None,
            // 213 0xD5 Õ
            CharEscapeMode.None,
            // 214 0xD6 Ö
            CharEscapeMode.None,
            // 215 0xD7 ×
            CharEscapeMode.None,
            // 216 0xD8 Ø
            CharEscapeMode.None,
            // 217 0xD9 Ù
            CharEscapeMode.None,
            // 218 0xDA Ú
            CharEscapeMode.None,
            // 219 0xDB Û
            CharEscapeMode.None,
            // 220 0xDC Ü
            CharEscapeMode.None,
            // 221 0xDD Ý
            CharEscapeMode.None,
            // 222 0xDE Þ
            CharEscapeMode.None,
            // 223 0xDF ß
            CharEscapeMode.None,
            // 224 0xE0 à
            CharEscapeMode.None,
            // 225 0xE1 á
            CharEscapeMode.None,
            // 226 0xE2 â
            CharEscapeMode.None,
            // 227 0xE3 ã
            CharEscapeMode.None,
            // 228 0xE4 ä
            CharEscapeMode.None,
            // 229 0xE5 å
            CharEscapeMode.None,
            // 230 0xE6 æ
            CharEscapeMode.None,
            // 231 0xE7 ç
            CharEscapeMode.None,
            // 232 0xE8 è
            CharEscapeMode.None,
            // 233 0xE9 é
            CharEscapeMode.None,
            // 234 0xEA ê
            CharEscapeMode.None,
            // 235 0xEB ë
            CharEscapeMode.None,
            // 236 0xEC ì
            CharEscapeMode.None,
            // 237 0xED í
            CharEscapeMode.None,
            // 238 0xEE î
            CharEscapeMode.None,
            // 239 0xEF ï
            CharEscapeMode.None,
            // 240 0xF0 ð
            CharEscapeMode.None,
            // 241 0xF1 ñ
            CharEscapeMode.None,
            // 242 0xF2 ò
            CharEscapeMode.None,
            // 243 0xF3 ó
            CharEscapeMode.None,
            // 244 0xF4 ô
            CharEscapeMode.None,
            // 245 0xF5 õ
            CharEscapeMode.None,
            // 246 0xF6 ö
            CharEscapeMode.None,
            // 247 0xF7 ÷
            CharEscapeMode.None,
            // 248 0xF8 ø
            CharEscapeMode.None,
            // 249 0xF9 ù
            CharEscapeMode.None,
            // 250 0xFA ú
            CharEscapeMode.None,
            // 251 0xFB û
            CharEscapeMode.None,
            // 252 0xFC ü
            CharEscapeMode.None,
            // 253 0xFD ý
            CharEscapeMode.None,
            // 254 0xFE þ
            CharEscapeMode.None,
            // 255 0xFF ÿ
            CharEscapeMode.None
        };

        private static readonly CharEscapeMode[] _charGroupEscapeModes = new[] {
            // 0 0x00
            CharEscapeMode.AsciiHexadecimal,
            // 1 0x01
            CharEscapeMode.AsciiHexadecimal,
            // 2 0x02
            CharEscapeMode.AsciiHexadecimal,
            // 3 0x03
            CharEscapeMode.AsciiHexadecimal,
            // 4 0x04
            CharEscapeMode.AsciiHexadecimal,
            // 5 0x05
            CharEscapeMode.AsciiHexadecimal,
            // 6 0x06
            CharEscapeMode.AsciiHexadecimal,
            // 7 0x07
            CharEscapeMode.Bell,
            // 8 0x08
            CharEscapeMode.AsciiHexadecimal,
            // 9 0x09
            CharEscapeMode.Tab,
            // 10 0x0A
            CharEscapeMode.Linefeed,
            // 11 0x0B
            CharEscapeMode.VerticalTab,
            // 12 0x0C
            CharEscapeMode.FormFeed,
            // 13 0x0D
            CharEscapeMode.CarriageReturn,
            // 14 0x0E
            CharEscapeMode.AsciiHexadecimal,
            // 15 0x0F
            CharEscapeMode.AsciiHexadecimal,
            // 16 0x10
            CharEscapeMode.AsciiHexadecimal,
            // 17 0x11
            CharEscapeMode.AsciiHexadecimal,
            // 18 0x12
            CharEscapeMode.AsciiHexadecimal,
            // 19 0x13
            CharEscapeMode.AsciiHexadecimal,
            // 20 0x14
            CharEscapeMode.AsciiHexadecimal,
            // 21 0x15
            CharEscapeMode.AsciiHexadecimal,
            // 22 0x16
            CharEscapeMode.AsciiHexadecimal,
            // 23 0x17
            CharEscapeMode.AsciiHexadecimal,
            // 24 0x18
            CharEscapeMode.AsciiHexadecimal,
            // 25 0x19
            CharEscapeMode.AsciiHexadecimal,
            // 26 0x1A
            CharEscapeMode.AsciiHexadecimal,
            // 27 0x1B
            CharEscapeMode.Escape,
            // 28 0x1C
            CharEscapeMode.AsciiHexadecimal,
            // 29 0x1D
            CharEscapeMode.AsciiHexadecimal,
            // 30 0x1E
            CharEscapeMode.AsciiHexadecimal,
            // 31 0x1F
            CharEscapeMode.AsciiHexadecimal,
            // 32 0x20
            CharEscapeMode.None,
            // 33 0x21 !
            CharEscapeMode.None,
            // 34 0x22 "
            CharEscapeMode.None,
            // 35 0x23 #
            CharEscapeMode.None,
            // 36 0x24 $
            CharEscapeMode.None,
            // 37 0x25 %
            CharEscapeMode.None,
            // 38 0x26 &
            CharEscapeMode.None,
            // 39 0x27 '
            CharEscapeMode.None,
            // 40 0x28 (
            CharEscapeMode.None,
            // 41 0x29 )
            CharEscapeMode.None,
            // 42 0x2A *
            CharEscapeMode.None,
            // 43 0x2B +
            CharEscapeMode.None,
            // 44 0x2C ,
            CharEscapeMode.None,
            // 45 0x2D -
            CharEscapeMode.Backslash,
            // 46 0x2E .
            CharEscapeMode.None,
            // 47 0x2F /
            CharEscapeMode.None,
            // 48 0x30 0
            CharEscapeMode.None,
            // 49 0x31 1
            CharEscapeMode.None,
            // 50 0x32 2
            CharEscapeMode.None,
            // 51 0x33 3
            CharEscapeMode.None,
            // 52 0x34 4
            CharEscapeMode.None,
            // 53 0x35 5
            CharEscapeMode.None,
            // 54 0x36 6
            CharEscapeMode.None,
            // 55 0x37 7
            CharEscapeMode.None,
            // 56 0x38 8
            CharEscapeMode.None,
            // 57 0x39 9
            CharEscapeMode.None,
            // 58 0x3A :
            CharEscapeMode.None,
            // 59 0x3B ;
            CharEscapeMode.None,
            // 60 0x3C <
            CharEscapeMode.None,
            // 61 0x3D =
            CharEscapeMode.None,
            // 62 0x3E >
            CharEscapeMode.None,
            // 63 0x3F ?
            CharEscapeMode.None,
            // 64 0x40 @
            CharEscapeMode.None,
            // 65 0x41 A
            CharEscapeMode.None,
            // 66 0x42 B
            CharEscapeMode.None,
            // 67 0x43 C
            CharEscapeMode.None,
            // 68 0x44 D
            CharEscapeMode.None,
            // 69 0x45 E
            CharEscapeMode.None,
            // 70 0x46 F
            CharEscapeMode.None,
            // 71 0x47 G
            CharEscapeMode.None,
            // 72 0x48 H
            CharEscapeMode.None,
            // 73 0x49 I
            CharEscapeMode.None,
            // 74 0x4A J
            CharEscapeMode.None,
            // 75 0x4B K
            CharEscapeMode.None,
            // 76 0x4C L
            CharEscapeMode.None,
            // 77 0x4D M
            CharEscapeMode.None,
            // 78 0x4E N
            CharEscapeMode.None,
            // 79 0x4F O
            CharEscapeMode.None,
            // 80 0x50 P
            CharEscapeMode.None,
            // 81 0x51 Q
            CharEscapeMode.None,
            // 82 0x52 R
            CharEscapeMode.None,
            // 83 0x53 S
            CharEscapeMode.None,
            // 84 0x54 T
            CharEscapeMode.None,
            // 85 0x55 U
            CharEscapeMode.None,
            // 86 0x56 V
            CharEscapeMode.None,
            // 87 0x57 W
            CharEscapeMode.None,
            // 88 0x58 X
            CharEscapeMode.None,
            // 89 0x59 Y
            CharEscapeMode.None,
            // 90 0x5A Z
            CharEscapeMode.None,
            // 91 0x5B [
            CharEscapeMode.None,
            // 92 0x5C \
            CharEscapeMode.Backslash,
            // 93 0x5D ]
            CharEscapeMode.Backslash,
            // 94 0x5E ^
            CharEscapeMode.None,
            // 95 0x5F _
            CharEscapeMode.None,
            // 96 0x60 `
            CharEscapeMode.None,
            // 97 0x61 a
            CharEscapeMode.None,
            // 98 0x62 b
            CharEscapeMode.None,
            // 99 0x63 c
            CharEscapeMode.None,
            // 100 0x64 d
            CharEscapeMode.None,
            // 101 0x65 e
            CharEscapeMode.None,
            // 102 0x66 f
            CharEscapeMode.None,
            // 103 0x67 g
            CharEscapeMode.None,
            // 104 0x68 h
            CharEscapeMode.None,
            // 105 0x69 i
            CharEscapeMode.None,
            // 106 0x6A j
            CharEscapeMode.None,
            // 107 0x6B k
            CharEscapeMode.None,
            // 108 0x6C l
            CharEscapeMode.None,
            // 109 0x6D m
            CharEscapeMode.None,
            // 110 0x6E n
            CharEscapeMode.None,
            // 111 0x6F o
            CharEscapeMode.None,
            // 112 0x70 p
            CharEscapeMode.None,
            // 113 0x71 q
            CharEscapeMode.None,
            // 114 0x72 r
            CharEscapeMode.None,
            // 115 0x73 s
            CharEscapeMode.None,
            // 116 0x74 t
            CharEscapeMode.None,
            // 117 0x75 u
            CharEscapeMode.None,
            // 118 0x76 v
            CharEscapeMode.None,
            // 119 0x77 w
            CharEscapeMode.None,
            // 120 0x78 x
            CharEscapeMode.None,
            // 121 0x79 y
            CharEscapeMode.None,
            // 122 0x7A z
            CharEscapeMode.None,
            // 123 0x7B {
            CharEscapeMode.None,
            // 124 0x7C |
            CharEscapeMode.None,
            // 125 0x7D }
            CharEscapeMode.None,
            // 126 0x7E ~
            CharEscapeMode.None,
            // 127 0x7F
            CharEscapeMode.AsciiHexadecimal,
            // 128 0x80
            CharEscapeMode.AsciiHexadecimal,
            // 129 0x81
            CharEscapeMode.AsciiHexadecimal,
            // 130 0x82
            CharEscapeMode.AsciiHexadecimal,
            // 131 0x83
            CharEscapeMode.AsciiHexadecimal,
            // 132 0x84
            CharEscapeMode.AsciiHexadecimal,
            // 133 0x85
            CharEscapeMode.AsciiHexadecimal,
            // 134 0x86
            CharEscapeMode.AsciiHexadecimal,
            // 135 0x87
            CharEscapeMode.AsciiHexadecimal,
            // 136 0x88
            CharEscapeMode.AsciiHexadecimal,
            // 137 0x89
            CharEscapeMode.AsciiHexadecimal,
            // 138 0x8A
            CharEscapeMode.AsciiHexadecimal,
            // 139 0x8B
            CharEscapeMode.AsciiHexadecimal,
            // 140 0x8C
            CharEscapeMode.AsciiHexadecimal,
            // 141 0x8D
            CharEscapeMode.AsciiHexadecimal,
            // 142 0x8E
            CharEscapeMode.AsciiHexadecimal,
            // 143 0x8F
            CharEscapeMode.AsciiHexadecimal,
            // 144 0x90
            CharEscapeMode.AsciiHexadecimal,
            // 145 0x91
            CharEscapeMode.AsciiHexadecimal,
            // 146 0x92
            CharEscapeMode.AsciiHexadecimal,
            // 147 0x93
            CharEscapeMode.AsciiHexadecimal,
            // 148 0x94
            CharEscapeMode.AsciiHexadecimal,
            // 149 0x95
            CharEscapeMode.AsciiHexadecimal,
            // 150 0x96
            CharEscapeMode.AsciiHexadecimal,
            // 151 0x97
            CharEscapeMode.AsciiHexadecimal,
            // 152 0x98
            CharEscapeMode.AsciiHexadecimal,
            // 153 0x99
            CharEscapeMode.AsciiHexadecimal,
            // 154 0x9A
            CharEscapeMode.AsciiHexadecimal,
            // 155 0x9B
            CharEscapeMode.AsciiHexadecimal,
            // 156 0x9C
            CharEscapeMode.AsciiHexadecimal,
            // 157 0x9D
            CharEscapeMode.AsciiHexadecimal,
            // 158 0x9E
            CharEscapeMode.AsciiHexadecimal,
            // 159 0x9F
            CharEscapeMode.AsciiHexadecimal,
            // 160 0xA0
            CharEscapeMode.None,
            // 161 0xA1 ¡
            CharEscapeMode.None,
            // 162 0xA2 ¢
            CharEscapeMode.None,
            // 163 0xA3 £
            CharEscapeMode.None,
            // 164 0xA4 ¤
            CharEscapeMode.None,
            // 165 0xA5 ¥
            CharEscapeMode.None,
            // 166 0xA6 ¦
            CharEscapeMode.None,
            // 167 0xA7 §
            CharEscapeMode.None,
            // 168 0xA8 ¨
            CharEscapeMode.None,
            // 169 0xA9 ©
            CharEscapeMode.None,
            // 170 0xAA ª
            CharEscapeMode.None,
            // 171 0xAB «
            CharEscapeMode.None,
            // 172 0xAC ¬
            CharEscapeMode.None,
            // 173 0xAD ­
            CharEscapeMode.None,
            // 174 0xAE ®
            CharEscapeMode.None,
            // 175 0xAF ¯
            CharEscapeMode.None,
            // 176 0xB0 °
            CharEscapeMode.None,
            // 177 0xB1 ±
            CharEscapeMode.None,
            // 178 0xB2 ²
            CharEscapeMode.None,
            // 179 0xB3 ³
            CharEscapeMode.None,
            // 180 0xB4 ´
            CharEscapeMode.None,
            // 181 0xB5 µ
            CharEscapeMode.None,
            // 182 0xB6 ¶
            CharEscapeMode.None,
            // 183 0xB7 ·
            CharEscapeMode.None,
            // 184 0xB8 ¸
            CharEscapeMode.None,
            // 185 0xB9 ¹
            CharEscapeMode.None,
            // 186 0xBA º
            CharEscapeMode.None,
            // 187 0xBB »
            CharEscapeMode.None,
            // 188 0xBC ¼
            CharEscapeMode.None,
            // 189 0xBD ½
            CharEscapeMode.None,
            // 190 0xBE ¾
            CharEscapeMode.None,
            // 191 0xBF ¿
            CharEscapeMode.None,
            // 192 0xC0 À
            CharEscapeMode.None,
            // 193 0xC1 Á
            CharEscapeMode.None,
            // 194 0xC2 Â
            CharEscapeMode.None,
            // 195 0xC3 Ã
            CharEscapeMode.None,
            // 196 0xC4 Ä
            CharEscapeMode.None,
            // 197 0xC5 Å
            CharEscapeMode.None,
            // 198 0xC6 Æ
            CharEscapeMode.None,
            // 199 0xC7 Ç
            CharEscapeMode.None,
            // 200 0xC8 È
            CharEscapeMode.None,
            // 201 0xC9 É
            CharEscapeMode.None,
            // 202 0xCA Ê
            CharEscapeMode.None,
            // 203 0xCB Ë
            CharEscapeMode.None,
            // 204 0xCC Ì
            CharEscapeMode.None,
            // 205 0xCD Í
            CharEscapeMode.None,
            // 206 0xCE Î
            CharEscapeMode.None,
            // 207 0xCF Ï
            CharEscapeMode.None,
            // 208 0xD0 Ð
            CharEscapeMode.None,
            // 209 0xD1 Ñ
            CharEscapeMode.None,
            // 210 0xD2 Ò
            CharEscapeMode.None,
            // 211 0xD3 Ó
            CharEscapeMode.None,
            // 212 0xD4 Ô
            CharEscapeMode.None,
            // 213 0xD5 Õ
            CharEscapeMode.None,
            // 214 0xD6 Ö
            CharEscapeMode.None,
            // 215 0xD7 ×
            CharEscapeMode.None,
            // 216 0xD8 Ø
            CharEscapeMode.None,
            // 217 0xD9 Ù
            CharEscapeMode.None,
            // 218 0xDA Ú
            CharEscapeMode.None,
            // 219 0xDB Û
            CharEscapeMode.None,
            // 220 0xDC Ü
            CharEscapeMode.None,
            // 221 0xDD Ý
            CharEscapeMode.None,
            // 222 0xDE Þ
            CharEscapeMode.None,
            // 223 0xDF ß
            CharEscapeMode.None,
            // 224 0xE0 à
            CharEscapeMode.None,
            // 225 0xE1 á
            CharEscapeMode.None,
            // 226 0xE2 â
            CharEscapeMode.None,
            // 227 0xE3 ã
            CharEscapeMode.None,
            // 228 0xE4 ä
            CharEscapeMode.None,
            // 229 0xE5 å
            CharEscapeMode.None,
            // 230 0xE6 æ
            CharEscapeMode.None,
            // 231 0xE7 ç
            CharEscapeMode.None,
            // 232 0xE8 è
            CharEscapeMode.None,
            // 233 0xE9 é
            CharEscapeMode.None,
            // 234 0xEA ê
            CharEscapeMode.None,
            // 235 0xEB ë
            CharEscapeMode.None,
            // 236 0xEC ì
            CharEscapeMode.None,
            // 237 0xED í
            CharEscapeMode.None,
            // 238 0xEE î
            CharEscapeMode.None,
            // 239 0xEF ï
            CharEscapeMode.None,
            // 240 0xF0 ð
            CharEscapeMode.None,
            // 241 0xF1 ñ
            CharEscapeMode.None,
            // 242 0xF2 ò
            CharEscapeMode.None,
            // 243 0xF3 ó
            CharEscapeMode.None,
            // 244 0xF4 ô
            CharEscapeMode.None,
            // 245 0xF5 õ
            CharEscapeMode.None,
            // 246 0xF6 ö
            CharEscapeMode.None,
            // 247 0xF7 ÷
            CharEscapeMode.None,
            // 248 0xF8 ø
            CharEscapeMode.None,
            // 249 0xF9 ù
            CharEscapeMode.None,
            // 250 0xFA ú
            CharEscapeMode.None,
            // 251 0xFB û
            CharEscapeMode.None,
            // 252 0xFC ü
            CharEscapeMode.None,
            // 253 0xFD ý
            CharEscapeMode.None,
            // 254 0xFE þ
            CharEscapeMode.None,
            // 255 0xFF ÿ
            CharEscapeMode.None
        };
    }
}
