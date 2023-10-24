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
    public class ConsoleManager
    {
        public static void Initialize()
        {
            InitializeConsole();
        }

        public static void NotidyShell(string directory)
        {
            UpdateShell(directory);
        }

        public static void Write(ImmutableArray<CharInfo> data, Rect rect)
        {
            ConsoleWrite(data.ToArray(), ref rect);
        }

        public static ImmutableArray<CharInfo> Read(Rect rect)
        {
            IntPtr charInfoPtr = ReadChunk(ref rect);

            CharInfo[] charInfoArray = new CharInfo[rect.Width * rect.Height];

            if (charInfoPtr != IntPtr.Zero)
            {
                // Create a pointer to the start of the memory block
                IntPtr currentPtr = charInfoPtr;

                for (int i = 0; i < charInfoArray.Length; i++)
                {
                    // Marshal the data at the current pointer location into CharInfo
                    charInfoArray[i] = Marshal.PtrToStructure<CharInfo>(currentPtr);

                    // Move the pointer to the next CharInfo element
                    currentPtr = IntPtr.Add(currentPtr, Marshal.SizeOf(typeof(CharInfo)));
                }
            }
            return ImmutableArray.Create(charInfoArray);
        }

        [DllImport("ConsoleManager.dll")]
        private static extern int InitializeConsole();

        [DllImport("ConsoleManager.dll")]
        private static extern int UpdateShell(string directory);

        [DllImport("ConsoleManager.dll")]
        private static extern int ConsoleWrite(CharInfo[] data, ref Rect rect);

        [DllImport("ConsoleManager.dll")]
        private static extern IntPtr ReadChunk(ref Rect rect);
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

        public static CharInfo[] FromText(string text, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
        {
            CharInfo[] chars = new CharInfo[text.Length];
            for (var i = 0; i < chars.Length; i++)
            {
                chars[i].UnicodeChar = text[i];
                chars[i].Attributes = (byte)(Convert.ToByte(foreground) | 
                                             Convert.ToByte(background) << 4);
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