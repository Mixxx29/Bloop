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
                _folders.Add(newFolder);
            }
        }

        protected void LoadDocuments()
        {
            foreach (var file in _info.GetFiles())
            {
                if (!file.Name.EndsWith(".bloop"))
                    continue;

                var newDocumemnt = new BloopDocument(file.Name, file.Directory.FullName);
                _documents.Add(newDocumemnt);
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
    }
}
