using Bloop.Editor.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloop.Editor.Tree
{
    internal class TreeItem
    {
        private BloopModel _model;
        private TreeItem? _parent;

        private ObservableCollection<TreeItem> _children;

        public TreeItem(BloopModel model)
        {
            _model = model;
        }

        public void AddItem(TreeItem item)
        {
            _children.Add(item);
            item._parent = this;
        }

        public void RemoveItem(TreeItem item)
        {
            _children.Remove(item);
            item._parent = null;
        }

        internal void Subscribe(TreeItemListener treeItemListener)
        {
            
        }
    }
}
