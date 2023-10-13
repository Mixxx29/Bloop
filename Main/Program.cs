using Bloop.Editor.Configuration;
using System.Runtime.InteropServices;
using System.Text;

namespace Bloop.Editor;

internal class Program
{
    [DllImport("ConsoleManager.dll")]
    public static extern int SetupConsoleFontSize(int size);

    [DllImport("ConsoleManager.dll")]
    public static extern int SetupConsoleSize(int width, int height);

    static void Main(string[] args)
    {
        Configure.LoadSettings();

        SetupConsoleFontSize(28);

        var maxWidth = Console.LargestWindowWidth;
        var maxHeigth = Console.LargestWindowHeight;

        Console.SetWindowSize(maxWidth, maxHeigth);
        Console.SetBufferSize(maxWidth, maxHeigth);

        var editor = new BloopEditor2();
        editor.Run();
    }
}