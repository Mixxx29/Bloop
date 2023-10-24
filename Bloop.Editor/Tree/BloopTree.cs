using Bloop.Editor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloop.Editor.Tree
{
    internal class BloopTree
    {
        private TreeItem _root;
        private TreeView _view;

        public BloopTree(BloopModel model)
        {
            _root = new TreeItem(model);
            _view = new TreeView(_root);
        }
    }
}
