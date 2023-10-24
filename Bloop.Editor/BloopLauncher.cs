using Microsoft.WindowsAPICodePack.Dialogs;

namespace Bloop.Editor
{
    public class BloopLauncher
    {

        public void Run()
        {
            var projectName = "Demo";
            var selectedDirectory = SelectHomeDirectory();
            if (selectedDirectory == null)
                return;

            var projectDirectoryPath = selectedDirectory + "\\" + projectName;


            if (!CreateProject(projectDirectoryPath))
                return;

            var editor = new BloopEditor(projectDirectoryPath);
            editor.Run();
        }

        private bool CreateProject(string homePath)
        {
            if (Directory.Exists(homePath))
            {
                return false;
            }

            Directory.CreateDirectory(homePath);
            return true;
        }

        private string? SelectHomeDirectory()
        {
            string? selectedFolder = null;

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select a folder",
                InitialDirectory = desktopPath
            };

            CommonFileDialogResult result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
                selectedFolder = dialog.FileName;

            return selectedFolder;
        }

        private void CopyDirectory(string sourceDirectoryPath, string targetDirectoryPath)
        {
            var sourceDirectory = new DirectoryInfo(sourceDirectoryPath);
            var targetDirectory = new DirectoryInfo(targetDirectoryPath);

            if (!targetDirectory.Exists)
                targetDirectory.Create();

            CopyFiles(sourceDirectory, targetDirectory);

            var directories = sourceDirectory.GetDirectories();
            foreach (var directory in directories)
            {
                var targetPath = Path.Combine(targetDirectoryPath, directory.Name);
                CopyDirectory(directory.FullName, targetPath);
            }
        }

        private void CopyFiles(DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory)
        {
            var files = sourceDirectory.GetFiles();

            foreach (var file in files)
            {
                var filepath = Path.Combine(targetDirectory.FullName, file.Name);
                file.CopyTo(filepath);
            }
        }
    }
}
