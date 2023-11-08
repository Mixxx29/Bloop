using Bloop.Editor.Configuration;
using Bloop.Editor;
using Microsoft.Win32;
using System.Runtime.InteropServices;

public class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        Settings.LoadSettings();

        if (args.Length == 1)
        {
            string? path = null;
            if (args[0].EndsWith(".bproj"))
            {
                path = args[0];
            }
            else if (args[0].EndsWith(".bloop"))
            {
                var file = new FileInfo(args[0]);
                var directory = file.Directory;
                path = GetProjectPath(directory);
            }
            
            if (path != null)
            {
                var editor = new BloopEditor(path);
                editor.Run();
                return;
            }
        }

        //SetDefaultExecutionApp();

        var launcher = new BloopLauncher();
        launcher.Run();
    }

    private static string? GetProjectPath(DirectoryInfo? directory)
    {
        if (directory == null || !directory.Exists)
            return null;

        foreach (var subfile in directory.GetFiles())
        {
            if (subfile.FullName.EndsWith(".bproj"))
                return subfile.FullName;
        }

        var path = GetProjectPath(directory.Parent);
        if (path != null)
            return path;

        return null;
    }

    // Constants for the SHCNE_ASSOCCHANGED and SHCNF_IDLIST flags
    private const uint SHCNE_ASSOCCHANGED = 0x08000000;
    private const uint SHCNF_IDLIST = 0x0000;

    [DllImport("shell32.dll")]
    public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);


    private static void SetDefaultExecutionApp()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var editorPath = Path.Combine(baseDirectory, "BloopIDE.exe");

        var iconPath = Path.Combine(baseDirectory, "bloop.ico");
        SetDefaultProgramForFileExtension(editorPath, ".bproj", "Bloop Project", iconPath);

        iconPath = Path.Combine(baseDirectory, "bloop-file.ico");
        SetDefaultProgramForFileExtension(editorPath, ".bloop", "Bloop File", iconPath);

        SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);

    }

    static void SetDefaultProgramForFileExtension(
        string exePath, string fileExtension, string description, string iconPath)
    {
        // Set the registry key for the custom program
        using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(fileExtension))
        {
            if (key != null)
            {
                // Set the default value to the description of your program
                key.SetValue("", description);

                // Create a subkey for the default icon
                using (RegistryKey iconKey = key.CreateSubKey("DefaultIcon"))
                {
                    if (iconKey != null)
                    {
                        // Set the default icon value (e.g., the path to your .exe)
                        iconKey.SetValue("", iconPath); // Set the path to your icon file
                    }
                }

                // Create a subkey for the shell open command
                using (RegistryKey shellKey = key.CreateSubKey("shell\\open\\command"))
                {
                    if (shellKey != null)
                    {
                        // Set the default value to the path to your .exe
                        shellKey.SetValue("", "\"" + exePath + "\" \"%1\"");
                    }
                }
            }
        }
    }

}