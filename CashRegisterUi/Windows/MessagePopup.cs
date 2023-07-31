using Gtk;

namespace CashRegisterUi.Windows
{
    internal static class MessagePopup
    {
        public static void ShowError(this Window parent, string message)
        {
            var w = new MessageDialog(parent, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, message);
            w.Run();
            w.Destroy();
        }

        public static bool ShowQuestion(this Window parent, string message)
        {
            var w = new MessageDialog(parent, DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo, message);
            var response = w.Run();
            w.Destroy();
            return response == -8;
        }
    }
}
