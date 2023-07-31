using CashRegisterCore.Entities;
using Gdk;
using Gtk;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace CashRegisterUi.Utils
{
    internal static class Extensions
    {
        public static void ModifyForeground(this Widget widget, Color color)
        {
            widget.ModifyFg(StateType.Normal, color);
            widget.ModifyFg(StateType.Prelight, color);
            widget.ModifyFg(StateType.Selected, color);
            widget.ModifyFg(StateType.Active, color);
            widget.ModifyFg(StateType.Insensitive, color);
        }

        public static Pixbuf GetPixbuf(this System.Drawing.Bitmap bitmap)
        {
            if (bitmap is null)
                return null;

            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                ms.Position = 0;
                return new Pixbuf(ms);
            }
        }

        public static Pixbuf GetPixbuf(this ImageResource imageResource)
        {
            if (imageResource is null || imageResource.Image is null || imageResource.Image.Count() == 0)
                return null;

            using (var ms = new MemoryStream())
            {
                ms.Write(imageResource.Image, 0, imageResource.Image.Count());
                ms.Position = 0;
                return new Pixbuf(ms);
            }
        }

        public static System.Drawing.Bitmap ResizeBitmap(this System.Drawing.Bitmap inputBitmap, int width, int height)
        {
            if (width > 0 && height > 0 && (inputBitmap.Width != width || inputBitmap.Height != height))
            {
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height);
                using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage((System.Drawing.Image)bitmap))
                {
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(inputBitmap, 0, 0, width, height);
                }
                return bitmap;
            }
            else
                return inputBitmap;
        }

        public static void ScaleFont(this Gtk.Label label, double scale)
        {
            var font = label.Style.FontDescription.Copy();
            font.Size = (int)Math.Floor(font.Size * scale);
            label.ModifyFont(font);
        }
    }
}
