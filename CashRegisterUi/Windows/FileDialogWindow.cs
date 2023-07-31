using Gtk;
using System.Collections.Generic;

namespace CashRegisterUi.Windows
{
    internal class FileDialogWindow : FileChooserDialog
    {
        public FileDialogWindow(Window parent, string title, FileChooserAction action, List<string> exts = null, bool selectMultiple = false)
            : base(title, parent, action)
        {
            //fcd.Icon = ((Bitmap)Resources.ResourceManager.GetObject("logo")).GetImage();

            this.Modal = true;
            this.TransientFor = this;
            this.TypeHint = Gdk.WindowTypeHint.Dialog;
            this.SelectMultiple = selectMultiple;
            this.AddButton(Stock.Cancel, ResponseType.Cancel);
            this.AddButton(Stock.Open, ResponseType.Ok);

            if(!(exts is null) && exts.Count > 0)
            {
                this.Filter = new FileFilter() { };
                exts.ForEach(ext => this.Filter.AddPattern("*." + ext));
            }
        }

        public string GetFilePath()
        {
            string path = null;
            ResponseType response = (ResponseType)this.Run();

            if (response == ResponseType.Ok)
                path = this.Filename;

            this.Destroy();

            return path;
        }
    }
}
