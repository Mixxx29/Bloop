using Bloop.Editor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloop.Editor.Actions
{
    internal class AddFolderAction : BloopAction
    {
        public AddFolderAction()
        {
            SetName("New Folder");
            SetShortcut("Ctrl+N");
        }

        public override void Action(object obj)
        {
            if (obj is BloopFolder folder)
            {
                folder.NewFolder();
            }
        }
    }
}
