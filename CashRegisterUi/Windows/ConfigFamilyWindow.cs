using CashRegisterCore.Entities;
using CashRegisterUi.Controls;
using CashRegisterUi.Languages;
using Gtk;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using CashRegisterCore.Managers;
using CashRegisterCore;
using CashRegisterUi.Utils;
using System;

namespace CashRegisterUi.Windows
{
    internal class ConfigFamilyWindow : Window //Dialog
    {
        private Family _family;
        private System.Action _refresh;

        private bool _imageChanged = false;
        private string _filePath;

        private Entry _nameEntry = new Entry();
        private Gtk.Image _image = new Gtk.Image() { WidthRequest = 300, HeightRequest = 300 };

        public ConfigFamilyWindow(Window parent, Family family, System.Action refresh)
            : base(WindowType.Toplevel)//: base()
        {
            _family = family;
            _refresh = refresh;

            this.Title = "Nova Familia";
            this.Modal = true;
            this.Parent = parent;
            this.DefaultWidth = 500;
            this.DefaultHeight = 400;
            this.TransientFor = parent;
            this.TypeHint = Gdk.WindowTypeHint.Dialog;
            this.WindowPosition = WindowPosition.CenterOnParent;

            var nameBox = new HBox(false, 2);
            nameBox.PackStart(new Dummy { WidthRequest = 10 }, false, false, 0);
            nameBox.PackStart(new Label(Translate.Language.Name + ":") { Xalign = 0, WidthRequest = 150 }, false, false, 0);
            nameBox.PackStart(_nameEntry, false, false, 0);

            var setImageButton = new ImageButton("openfolder").Get(25);
            setImageButton.Clicked += ChooseImage;

            var clearImageButton = new ImageButton("clear").Get(25);
            clearImageButton.Clicked += delegate
            {
                _imageChanged = true;
                _filePath = null;
                _image.Pixbuf = null;
            };

            var imgControlBox = new HBox(false, 0);
            imgControlBox.PackStart(new Dummy { WidthRequest = 10 }, false, false, 0);
            imgControlBox.PackStart(new Label("Imagem" + ":") { Xalign = 0, WidthRequest = 110 }, false, false, 0);
            imgControlBox.PackStart(setImageButton, false, false, 0);
            imgControlBox.PackStart(clearImageButton, false, false, 0);

            var imageControlBox = new VBox(false, 0);
            imageControlBox.PackStart(imgControlBox, false, false, 0);
            imageControlBox.PackStart(new Dummy(), true, true, 0);

            var imgBox = new HBox(false, 0);
            imgBox.PackStart(imageControlBox, false, false, 0);
            imgBox.PackStart(_image, true, true, 0);

            var saveButton = new Button("Salvar");
            saveButton.Clicked += SaveFamily;

            var cancelButton = new Button("Cancelar");
            cancelButton.Clicked += delegate { this.Destroy(); };

            var buttons = new HBox(false, 2);
            buttons.PackStart(new Dummy(), true, true, 0);
            buttons.PackStart(saveButton, false, false, 0);
            buttons.PackStart(cancelButton, false, false, 0);

            var box = new VBox(false, 2);
            box.PackStart(nameBox, true, true, 0);
            box.PackStart(imgBox, true, true, 0);
            box.PackStart(buttons, false, true, 0);

            Add(box);

            if (_family.Id > 0)
            {
                this.Title = $"Editar familia: {_family.Name}";
                _nameEntry.Text = _family.Name;
                if(_family.ImageId > 0)
                    _image.Pixbuf = AppCore.ImageResourceManager.GetImageResource(_family.ImageId).GetPixbuf();
            }

            ShowAll();
        }

        private void ChooseImage(object sender, EventArgs e)
        {
            var filePath = new FileDialogWindow(this, "Image", FileChooserAction.Open, new List<string>() { "png", "bmp", "gif", "jpg", "jpeg" }).GetFilePath();
            if (!string.IsNullOrEmpty(filePath))
            {
                _imageChanged = true;
                _filePath = filePath;

                Gdk.Pixbuf pix;
                using (var ms = new MemoryStream())
                {
                    ImageResourceManager.Scale(Bitmap.FromFile(filePath), 300, 300).Save(ms, ImageFormat.Png);
                    ms.Position = 0;
                    pix = new Gdk.Pixbuf(ms.ToArray());
                }
                _image.Pixbuf = pix;
            }
        }

        private void SaveFamily(object sender, EventArgs e)
        {
            try
            {
                if (_imageChanged)
                {
                    if (string.IsNullOrEmpty(_filePath))
                    {
                        if (_family.ImageId > 0)
                        {
                            AppCore.ImageResourceManager.DeleteImageResource(_family.ImageId);
                            _family.ImageId = 0;
                        }
                    }
                    else
                    {
                        if (_family.ImageId > 0)
                        {
                            var imageResource = new ImageResource() { Id = _family.ImageId, FilePath = _filePath };
                            AppCore.ImageResourceManager.SaveImageResource(imageResource);
                        }
                        else
                        {
                            var imageResource = new ImageResource() { FilePath = _filePath };
                            AppCore.ImageResourceManager.SaveImageResource(imageResource);
                            _family.ImageId = imageResource.Id;
                        }
                    }
                }

                _family.Name = _nameEntry.Text;
                AppCore.FamilyManager.SaveFamily(_family);
                _refresh();
                this.Destroy();
            }
            catch (Exception ex)
            {
                this.ShowError("Não gravou!!!\n" + ex.Message);
            }
        }
    }
}
