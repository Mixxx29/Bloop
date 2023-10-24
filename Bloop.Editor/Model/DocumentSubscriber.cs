using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloop.Editor.Model
{
    public interface DocumentSubscriber
    {
        void OnDocumentChanged(int lineIndex);
        void OnLineChanged(int lineIndex, int charIndex);
    }
}
