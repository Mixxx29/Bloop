#include <iostream>
#include <iomanip>
#include <fstream>
#include <Windows.h>

using namespace std;

extern "C" __declspec(dllexport) int SetupConsoleFontSize(int size)
{
    const wchar_t* fontFilePath = L"./Font/CascadiaMono.ttf";

    AddFontResourceW(fontFilePath);

    // Get the handle to the standard output console
    HANDLE hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
    
    // Set the new font face (Cascadia Code)
    CONSOLE_FONT_INFOEX fontInfo;
    fontInfo.cbSize = sizeof(CONSOLE_FONT_INFOEX);
    fontInfo.nFont = 0; // Use the first available font
    fontInfo.dwFontSize.X = size / 2; // Set the font width (in pixels)
    fontInfo.dwFontSize.Y = size; // Set the font height (in pixels)
    wcscpy_s(fontInfo.FaceName, L"Cascadia Mono");

    // Apply the new font and cursor settings
    SetCurrentConsoleFontEx(hConsole, FALSE, &fontInfo);

    return 0;
}

struct Rect
{
    int X;
    int Y;
    int Width;
    int Height;
};

extern "C" __declspec(dllexport) int ConsoleWrite(const CHAR_INFO* data, Rect& rect)
{
    HANDLE hConsole = GetStdHandle(STD_OUTPUT_HANDLE);

    CONSOLE_SCREEN_BUFFER_INFO csbi;
    GetConsoleScreenBufferInfo(hConsole, &csbi);

    COORD size =
    {
        rect.Width,
        rect.Height
    };

    SMALL_RECT writeRegion =
    {
        rect.X,
        rect.Y,
        rect.X + rect.Width - 1,
        rect.Y + rect.Height - 1
    };

    SetConsoleOutputCP(CP_UTF8);
    WriteConsoleOutputW(hConsole, data, size, {0, 0}, &writeRegion);

    SetConsoleTextAttribute(
        hConsole, 
        FOREGROUND_INTENSITY | FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE
    );

    return 0;
}

extern "C" __declspec(dllexport) void DrawVerticalLines()
{
    // Get the handle to the standard output console
    HANDLE hConsole = GetStdHandle(STD_OUTPUT_HANDLE);

    // Get console screen buffer information
    CONSOLE_SCREEN_BUFFER_INFO csbi;
    GetConsoleScreenBufferInfo(hConsole, &csbi);

    // Get the dimensions of the console buffer
    COORD bufferSize = { 1, csbi.dwSize.Y };

    // Create a buffer for the '│' character
    CHAR_INFO* buffer = new CHAR_INFO[bufferSize.Y];

    // Character info for drawing '│'
    for (int i = 0; i < bufferSize.Y; i++) {
        buffer[i].Char.UnicodeChar = L'│';
        buffer[i].Attributes = FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE; // Character color attributes
    }

    // Define a write region to draw the vertical lines on the first column
    SMALL_RECT writeRegionFirst;
    writeRegionFirst.Left = 0; // First column
    writeRegionFirst.Top = 0; // Top row
    writeRegionFirst.Right = 0; // First column
    writeRegionFirst.Bottom = bufferSize.Y - 1; // Bottom row

    // Define a write region to draw the vertical lines on the last column
    SMALL_RECT writeRegionLast;
    writeRegionLast.Left = csbi.dwSize.X - 1; // Last column
    writeRegionLast.Top = 0; // Top row
    writeRegionLast.Right = csbi.dwSize.X - 1; // Last column
    writeRegionLast.Bottom = bufferSize.Y - 1; // Bottom row

    SetConsoleOutputCP(CP_UTF8);

    // Write the buffer to the console for the first and last columns
    WriteConsoleOutputW(hConsole, buffer, bufferSize, { 0, 0 }, &writeRegionFirst);
    WriteConsoleOutputW(hConsole, buffer, bufferSize, { 0, 0 }, &writeRegionLast);

    // Clean up
    delete[] buffer;
}