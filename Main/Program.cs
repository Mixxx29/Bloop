using Bloop.Editor.Configuration;
using System.Runtime.InteropServices;
using System.Text;

namespace Bloop.Editor;

internal class Program
{
    static void Main(string[] args)
    {
        Configure.LoadSettings();

        var editor = new BloopEditor2();
        editor.Run();
    }
}