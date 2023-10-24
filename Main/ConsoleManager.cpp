#include <iostream>
#include <iomanip>
#include <fstream>
#include <Windows.h>

using namespace std;

extern "C" __declspec(dllexport) int InitializeConsole()
{
    HANDLE hConsole = GetStdHandle(STD_INPUT_HANDLE);
    DWORD mode;

    // Get the current input mode
    GetConsoleMode(hConsole, &mode);

    // Disable "Quick Edit" mode by clearing the ENABLE_QUICK_EDIT_MODE flag
    mode &= ~(ENABLE_QUICK_EDIT_MODE | ENABLE_PROCESSED_INPUT);

    // Set the modified input mode
    SetConsoleMode(hConsole, mode);

    return 0;
}

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

extern "C" __declspec(dllexport) CHAR_INFO* ReadChunk(Rect &rect)
{
    HANDLE hConsole = GetStdHandle(STD_OUTPUT_HANDLE);

    COORD size =
    {
        rect.Width,
        rect.Height
    };

    SMALL_RECT readRegion =
    {
        rect.X,
        rect.Y,
        rect.X + rect.Width - 1,
        rect.Y + rect.Height - 1
    };

    CHAR_INFO* data = new CHAR_INFO[rect.Width * rect.Height];

    ReadConsoleOutputW(hConsole, data, size, { 0, 0 }, &readRegion);

    return data;
}
