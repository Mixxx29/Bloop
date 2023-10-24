using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloop.Editor.Tree
{
    internal class TreeView : TreeItemListener
    {
        private TreeItem _root;

        public TreeView(TreeItem root)
        {
            _root = root;
            _root.Subscribe(this);
        }

        public void ItemAdded(TreeItem item)
        {
            
        }

        public void ItemRemoved(TreeItem item)
        {
            
        }

        public void ItemChanged(TreeItem item)
        {

        }
    }

    internal interface TreeItemListener
    {
        void ItemAdded(TreeItem item);
        void ItemRemoved(TreeItem item);
        void ItemChanged(TreeItem item);
    }
}
