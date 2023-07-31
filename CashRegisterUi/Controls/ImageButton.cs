using CashRegisterUi.Properties;
using CashRegisterUi.Utils;
using Gtk;
using System.Drawing;

namespace CashRegisterUi.Controls
{
    internal class ImageButton
    {
        private Bitmap _image;

        public ImageButton(string image)
            => _image = (Bitmap)Resources.ResourceManager.GetObject(image);

        public Button Get(int width)
            => new Button(new Gtk.Image(_image.ResizeBitmap(width - 10, width - 10).GetPixbuf())) { WidthRequest = width };
    }
}
