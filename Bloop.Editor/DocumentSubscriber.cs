using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloop.Editor
{
    public interface DocumentSubscriber
    {
        void OnDocumentChanged(object? sender, NotifyCollectionChangedEventArgs e);
        void OnDocumentChanged();
        void OnLineChanged();
    }
}
