

using Bloop.Editor.Document;
using Bloop.Editor.Configuration;
using Bloop.Editor.Window;

namespace Bloop
{
    public class BloopEditor2
    {
        private bool _processing = true;

        private BloopWindow _focusedWindow;

        private DocumentWindow _documentWindow;
        private ProjectWindow _projectWindow;

        public BloopEditor2()
        {
            Configure.LoadSettings();

            var leftOffset = 0;
            var topOffset = 0;
            var projectWindowFrame = new WindowFrame(
                "Demo Project",
                leftOffset,
                topOffset,
                30,
                Console.BufferHeight - 1
            );
            _projectWindow = new ProjectWindow(projectWindowFrame);

            var document = new BloopDocument();

            leftOffset = 30;
            topOffset = 0;
            var documentWindowFrame = new WindowFrame(
                document.Name,
                leftOffset,
                topOffset,
                Console.BufferWidth - leftOffset,
                Console.BufferHeight - 1
            );
            _documentWindow = new DocumentWindow(document, documentWindowFrame);

            _focusedWindow = _documentWindow;
        }

        public void Run()
        {
            _projectWindow.Render();
            _documentWindow.Render();

            ProccessInput();
        }

        private void ProccessInput()
        {
            while (_processing)
            {
                var key = Console.ReadKey(true);
                HandleKey(key);
            }
        }

        private void HandleKey(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.Escape:
                    HandleEscape();
                    break;
            }

            _focusedWindow?.HandleKey(key);
        }

        private void HandleEscape()
        {
            _processing = false;
        }
    }
}