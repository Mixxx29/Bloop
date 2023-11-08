using Bloop.Editor.Model;

namespace Bloop.Editor.Actions
{
    internal class DeleteAction : BloopAction
    {
        public DeleteAction()
        {
            SetName("Delete");
            SetShortcut("Del");
        }

        public override void Action(object obj)
        {
            if (obj is BloopModel model)
            {
                model.Parent?.RemoveChild(model);
            }
        }
    }
}
