using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace Bloop.Editor.Model
{
    internal class BloopFolder : BloopModel
    {
        protected DirectoryInfo _info;
        protected List<BloopFolder> _folders;
        protected List<BloopDocument> _documents;

        public BloopFolder(string name, string path) : base(name, path)
        {
            _info = new DirectoryInfo(path);
            _folders = new List<BloopFolder>();
            _documents = new List<BloopDocument>();

            Collapsed = true;
            Load();
        }

        public BloopFolder(DirectoryInfo info) : base(info.Name, info.FullName)
        {
            _info = info;
            _folders = new List<BloopFolder>();
            _documents = new List<BloopDocument>();

            Collapsed = true;
        }

        public bool Collapsed { get; protected set; }

        protected void Load()
        {
            LoadDirectories();
            LoadDocuments();
        }

        private void LoadDirectories()
        {
            foreach (var subdirectory in _info.GetDirectories())
            {
                var newFolder = new BloopFolder(subdirectory.Name, subdirectory.FullName);
                AddChild(newFolder);
            }
        }

        protected void LoadDocuments()
        {
            foreach (var file in _info.GetFiles())
            {
                if (!file.Name.EndsWith(".bloop"))
                    continue;

                var newDocumemnt = new BloopDocument(file.Name, file.Directory.FullName);
                AddChild(newDocumemnt);
            }
        }

        internal ImmutableArray<BloopFolder> GetFolders()
        {
            return _folders.ToImmutableArray();
        }

        internal ImmutableArray<BloopDocument> GetDocuments()
        {
            return _documents.ToImmutableArray();
        }

        internal IEnumerable<BloopModel> GetChildren()
        {
            var builder = ImmutableArray.CreateBuilder<BloopModel>();
            builder.AddRange(_folders);
            builder.AddRange(_documents);
            return builder.ToImmutable();
        }

        internal BloopModel? GetLastChild()
        {
            if (_documents.Any())
                return _documents.Last();

            return _folders.LastOrDefault();
        }

        internal virtual void Toggle()
        {
            Collapsed = !Collapsed;
        }

        internal void NewFolder()
        {
            var subdirectory = _info.CreateSubdirectory("./NewFolder");
            AddChild(new BloopFolder(subdirectory));
            Collapsed = false;
        }

        internal void NewDocument()
        {
            var document = new BloopDocument("NewDocument.bloop", _info.FullName);
            AddChild(document);
            Collapsed = false;
        }

        internal void AddChild(BloopModel child)
        {
            if (child is BloopFolder folder)
            {
                _folders.Add(folder);
            }
            else if (child is BloopDocument document)
            {
                _documents.Add(document);
            }

            child.Parent = this;
        }

        internal void RemoveChild(BloopModel child)
        {
            if (child is BloopFolder folder)
            {
                _folders.Remove(folder);
                var info = new DirectoryInfo(folder.Path);
                info.Delete(true);
            }
            else if (child is BloopDocument document)
            {
                _documents.Remove(document);
                File.SetAttributes(document.Path + "\\" + document.Name, FileAttributes.Normal);
                File.Delete(document.Path + "\\" + document.Name);
            }

            child.Parent = null;
        }

        public override void Save()
        {
            
        }
    }
}
