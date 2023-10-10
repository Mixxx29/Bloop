using System.Runtime.InteropServices;
using System.Text;

namespace Bloop.Editor;

internal class Program
{
    static void Main(string[] args)
    {
        var maxWidth = Console.LargestWindowWidth;
        var maxHeigth = Console.LargestWindowHeight;

        Console.SetBufferSize(maxWidth, maxHeigth);
        Console.SetWindowSize(maxWidth, maxHeigth);

        var editor = new BloopEditor2();
        editor.Run();
    }
}