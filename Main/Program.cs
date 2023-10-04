using System.Runtime.InteropServices;
using System.Text;

namespace Bloop.Editor;

internal class Program
{
    static void Main(string[] args)
    {
        var editor = new BloopEditor();
        editor.Run();
    }
}