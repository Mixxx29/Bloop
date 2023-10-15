using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bloop.Editor
{
    internal class ConsoleManager
    {
        public static void Write(ImmutableArray<CharInfo> data, Rect rect)
        {
            ConsoleWrite(data.ToArray(), ref rect);
        }

        [DllImport("ConsoleManager.dll")]
        private static extern int ConsoleWrite(CharInfo[] data, ref Rect rect);
    }

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public struct CharInfo
    {
        [FieldOffset(0)]
        public char UnicodeChar;

        [FieldOffset(0)]
        public byte AsciiChar;

        [FieldOffset(2)]
        public ushort Attributes;

        public static CharInfo[] FromText(string text, ConsoleColor color)
        {
            CharInfo[] chars = new CharInfo[text.Length];
            for (var i = 0; i < chars.Length; i++)
            {
                chars[i].UnicodeChar = text[i];
                chars[i].Attributes = Convert.ToByte(color);
            }

            return chars;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
    }
}