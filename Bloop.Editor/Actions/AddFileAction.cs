using Bloop.Editor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloop.Editor.Actions
{
    internal class AddFileAction : BloopAction
    {
        public AddFileAction()
        {
            SetName("New File");
            SetShortcut("Ctrl+M");
        }

        public override void Action(object obj)
        {
            if (obj is BloopFolder folder)
            {
                folder.NewDocument();
            }
        }
    }
}
