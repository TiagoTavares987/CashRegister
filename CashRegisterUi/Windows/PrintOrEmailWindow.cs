using CashRegisterCore;
using CashRegisterCore.Entities;
using CashRegisterUi.Controls;
using CashRegisterUi.Properties;
using CashRegisterUi.Utils;
using Gtk;
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CashRegisterUi.Windows
{
    internal class PrintOrEmailWindow : Window
    {
        private Document _document;

        private Entry _emailEntry = new Entry();
        private Gtk.Image _emailImageInvalid = new Gtk.Image(((Bitmap)Resources.ResourceManager.GetObject("clear")).ResizeBitmap(20, 20).GetPixbuf());

        public PrintOrEmailWindow(Window parent, Document document)
            : base(WindowType.Toplevel)
        {
            if(document is null)
            {
                this.Destroy();
                return;
            }

            this.Title = "Impressão ou envio de email";
            this.Modal = true;
            this.Parent = parent;
            //this.DefaultWidth = 100;
            this.DefaultHeight = 180;
            this.TransientFor = parent;
            this.TypeHint = Gdk.WindowTypeHint.Dialog;
            this.WindowPosition = WindowPosition.CenterOnParent;

            _document = document;

            var printButton = new Button("Imprimir");
            printButton.Clicked += Print;
            var emailButton = new Button("Email");
            emailButton.Clicked += Email;

            var buttonsBox = new HBox(true, 2);
            buttonsBox.PackStart(printButton, true, true, 5);
            buttonsBox.PackStart(emailButton, true, true, 5);

            _emailEntry.Changed += HideEmailInvaild;
            _emailImageInvalid.Realized += HideEmailInvaild;

            var emailBox = new HBox(false, 1);
            emailBox.PackStart(new Dummy() { WidthRequest = 5 }, false, false, 0);
            emailBox.PackStart(new Label("Email:") { Xalign = 0 }, false, false, 0);
            emailBox.PackStart(_emailEntry, true, true, 0);
            emailBox.PackStart(_emailImageInvalid, false, false, 0);
            emailBox.PackStart(new Dummy() { WidthRequest = 5 }, false, false, 0);

            var cancelButton = new Button("Cancelar");
            cancelButton.Clicked += delegate { this.Destroy(); };

            var exitBox = new HBox(false, 2);
            exitBox.PackStart(new Dummy(), true, true, 0);
            exitBox.PackStart(cancelButton, false, false, 0);

            var vbox = new VBox(false, 2);
            vbox.PackStart(new Dummy() { HeightRequest = 5 }, false, false, 0);
            vbox.PackStart(buttonsBox, true, true, 0);
            vbox.PackStart(emailBox, false, true, 5);

            var hbox = new HBox(false, 2);
            hbox.PackStart(new Dummy(), false, false, 0);
            hbox.PackStart(vbox, true, true, 0);
            hbox.PackStart(new Dummy(), false, false, 0);

            var box = new VBox(false, 2);
            box.PackStart(hbox, true, true, 0);
            box.PackStart(exitBox, false, true, 0);

            Add(box);

            ShowAll();
        }

        private void Print(object sender, EventArgs e) => Task.Run(() => AppCore.PritingManager.Print(_document));

        private void Email(object sender, EventArgs e)
        {
            if (Regex.IsMatch(_emailEntry.Text, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                Task.Run(() => AppCore.PritingManager.Email(_document, _emailEntry.Text));
                this.Destroy();
            }
            else
                _emailImageInvalid.Visible = true;
        }

        private void HideEmailInvaild(object sender, EventArgs e)
            => _emailImageInvalid.Visible = false;
    }
}
