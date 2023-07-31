using CashRegisterUi.Properties;
using Gtk;
using System;
using System.Drawing.Imaging;
using System.IO;

namespace CashRegisterUi.Windows
{
    public class SplashWindow : Window
    {
        private Label _label = new Label() { Xalign = 0 };

        public SplashWindow(Action<SplashWindow> realized)
            : base(WindowType.Toplevel)
        {
            Title = "Cash Register";
            Resizable = false;
            Decorated = false;
            WindowPosition = WindowPosition.Center;

            var vbox = new VBox(false, 0);
            using (var stream = new MemoryStream())
            {
                ((System.Drawing.Bitmap)Resources.ResourceManager.GetObject("logo")).Save(stream, ImageFormat.Png);
                stream.Position = 0;
                vbox.PackStart(new Image(new Gdk.Pixbuf(stream)));
            }

            var hbox = new HBox(false, 0);
            hbox.PackStart(_label, true, true, 10);

            vbox.PackStart(hbox, false, false, 10);
            Add(vbox);

            Realized += delegate { realized(this); };
            ShowAll();
        }

        public string Loading
        {
            get => _label.Text;
            set => _label.Text = value;
        }
    }
}
