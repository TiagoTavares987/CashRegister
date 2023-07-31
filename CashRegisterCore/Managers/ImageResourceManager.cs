using CashRegisterCore.Entities;
using CashRegisterCore.Providers;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace CashRegisterCore.Managers
{
    public class ImageResourceManager
    {
        private ImageResourceProvider _provider;

        public ImageResourceManager() => _provider = new ImageResourceProvider();

        public ImageResource GetImageResource(int id) => _provider.GetImageResource(id);

        public void SaveImageResource(ImageResource imageResource)
        {
            if (imageResource is null || string.IsNullOrWhiteSpace(imageResource.FilePath) || !File.Exists(imageResource.FilePath))
                return;

            imageResource.Name = Path.GetFileNameWithoutExtension(imageResource.FilePath);
            imageResource.Ext = Path.GetExtension(imageResource.FilePath);
            using (var ms = new MemoryStream())
            {
                Scale(Bitmap.FromFile(imageResource.FilePath), 100, 100).Save(ms, ImageFormat.Png);
                ms.Position = 0;
                imageResource.Image = ms.ToArray();
            }

            if (imageResource.Id == 0)
            {
                var id = _provider.InsertImageResource(imageResource);
                if (id > 0)
                    imageResource.Id = id;
            }
            else
                _provider.UpdateImageResource(imageResource);
        }

        public int DeleteImageResource(int id)
        {
            ImageResource imageResource = GetImageResource(id);
            if (imageResource == null)
                throw new ArgumentNullException("Invalid image.");

            return _provider.DeleteImageResource(imageResource);
        }

        public static Image Scale(Image inputBitmap, int width, int height)
        {
            if (width > 0 && height > 0 && (inputBitmap.Width > width || inputBitmap.Height > height))
            {
                var w = width;
                var h = inputBitmap.Height * width / inputBitmap.Width;
                if (h > height)
                {
                    w = inputBitmap.Width * height / inputBitmap.Height;
                    h = height;
                }

                Bitmap bitmap = new Bitmap(w, h);
                using (Graphics graphics = Graphics.FromImage((Image)bitmap))
                {
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(inputBitmap, 0, 0, w, h);
                }
                return bitmap;
            }
            else
                return inputBitmap;
        }
    }
}
