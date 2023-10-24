using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloop.Editor.Model
{
    internal class BloopProject : BloopFolder
    {

        public BloopProject(string name, string path) : base(name, path)
        {
            Collapsed = false;
        }

        public BloopDocument? FirstDocument()
        {
            foreach (var child in GetChildren())
            {
                if (child is BloopDocument document)
                    return document;
            }

            return null;
        }

        internal override void Toggle()
        {
            
        }
    }
}
