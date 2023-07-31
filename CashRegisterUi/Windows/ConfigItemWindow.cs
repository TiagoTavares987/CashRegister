using CashRegisterCore;
using CashRegisterCore.Entities;
using CashRegisterCore.Managers;
using CashRegisterUi.Controls;
using CashRegisterUi.Languages;
using Gtk;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System;
using CashRegisterUi.Utils;
using System.Linq;
using ThermalPrinter.Utils;
using CashRegisterUi.Properties;

namespace CashRegisterUi.Windows
{
    internal class ConfigItemWindow : Gtk.Window
    {

        private CashRegisterCore.Entities.Item _item;
        private System.Action _refresh;

        private bool _imageChanged = false;
        private string _filePath;

        private Entry _nameEntry = new Entry();
        private Entry _shortNameEntry = new Entry();
        private ComboBox _familyCombo;
        private List<Family> _families = AppCore.FamilyManager.GetAll().ToList();
        private SpinButton _priceEntry = new SpinButton(0.01, double.MaxValue, 1) { Value = 1 }; // 3 casas decimais
        private SpinButton _costEntry = new SpinButton(0.01, double.MaxValue, 1) { Value = 1 };
        private Entry _barCodeEntry = new Entry();

        private Gtk.Image _image = new Gtk.Image() { WidthRequest = 300, HeightRequest = 300 };

        public ConfigItemWindow(Window parent, CashRegisterCore.Entities.Item item, System.Action refresh)
            : base(WindowType.Toplevel)//: base()
        {
            _item = item;
            _refresh = refresh;

            this.Title = "Novo Item";
            this.Modal = true;
            this.Parent = parent;
            this.DefaultWidth = 500;
            this.DefaultHeight = 400;
            this.TransientFor = parent;
            this.TypeHint = Gdk.WindowTypeHint.Dialog;
            this.WindowPosition = WindowPosition.CenterOnParent;

            _priceEntry.Digits = 3;
            _costEntry.Digits = 3;

            var nameBox = new HBox(false, 2);
            nameBox.PackStart(new Dummy { WidthRequest = 10 }, false, false, 0);
            nameBox.PackStart(new Label(Translate.Language.Name + ":") { Xalign = 0, WidthRequest = 150 }, false, false, 0);
            nameBox.PackStart(_nameEntry, false, false, 0);

            var shortNameBox = new HBox(false, 2);
            shortNameBox.PackStart(new Dummy { WidthRequest = 10 }, false, false, 0);
            shortNameBox.PackStart(new Label("Abreviatura" + ":") { Xalign = 0, WidthRequest = 150 }, false, false, 0);
            shortNameBox.PackStart(_shortNameEntry, false, false, 0);

            _familyCombo = new ComboBox(_families.Select(x => x.Name).ToArray()) { Active = -1 };
            var familyBox = new HBox(false, 2);
            familyBox.PackStart(new Dummy { WidthRequest = 10 }, false, false, 0);
            familyBox.PackStart(new Label("Familia" + ":") { Xalign = 0, WidthRequest = 150 }, false, false, 0);
            familyBox.PackStart(_familyCombo, false, false, 0);

            var priceBox = new HBox(false, 2);
            priceBox.PackStart(new Dummy { WidthRequest = 10 }, false, false, 0);
            priceBox.PackStart(new Label("Preço" + ":") { Xalign = 0, WidthRequest = 150 }, false, false, 0);
            priceBox.PackStart(_priceEntry, false, false, 0);

            var costBox = new HBox(false, 2);
            costBox.PackStart(new Dummy { WidthRequest = 10 }, false, false, 0);
            costBox.PackStart(new Label("Custo" + ":") { Xalign = 0, WidthRequest = 150 }, false, false, 0);
            costBox.PackStart(_costEntry, false, false, 0);

            var bitmap = (Bitmap)Resources.ResourceManager.GetObject("refresh");
            var generateBarcodeButton = new Button(new Gtk.Image(bitmap.ResizeBitmap(15, 15).GetPixbuf())) { WidthRequest = 25 };
            generateBarcodeButton.Clicked += delegate
            {
                var r = new Random();
                var c1 = r.Next(0, 99999).ToString();
                if (c1.Length < 5)
                    c1 = c1.PadLeft(5, '0');
                var c2 = r.Next(0, 999999).ToString();
                if (c2.Length < 5)
                    c2 = c2.PadLeft(6, '0');
                var code = r.Next(1, 9).ToString() + c1 + c2;
                _barCodeEntry.Text = code + Barcode.CalculateCheckDigit(code);
            };

            var barCodeBox = new HBox(false, 2);
            barCodeBox.PackStart(new Dummy { WidthRequest = 10 }, false, false, 0);
            barCodeBox.PackStart(new Label("CodigoBarras" + ":") { Xalign = 0, WidthRequest = 123 }, false, false, 0);
            barCodeBox.PackStart(generateBarcodeButton, false, false, 0);
            barCodeBox.PackStart(_barCodeEntry, false, false, 0);

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
            saveButton.Clicked += SaveItem;

            var cancelButton = new Button("Cancelar");
            cancelButton.Clicked += delegate { this.Destroy(); };

            var buttons = new HBox(false, 2);
            buttons.PackStart(new Dummy(), true, true, 0);
            buttons.PackStart(saveButton, false, false, 0);
            buttons.PackStart(cancelButton, false, false, 0);

            var box = new VBox(false, 2);
            box.PackStart(nameBox, true, true, 0);
            box.PackStart(shortNameBox, true, true, 0);
            box.PackStart(familyBox, true, true, 0);
            box.PackStart(priceBox, true, true, 0);
            box.PackStart(costBox, true, true, 0);
            box.PackStart(barCodeBox, true, true, 0);
            box.PackStart(imgBox, true, true, 0);
            box.PackStart(buttons, false, true, 0);

            Add(box);

            if (_item.Id > 0)
            {
                this.Title = $"Editar item: {_item.Name}";
                _nameEntry.Text = _item.Name;
                _shortNameEntry.Text = _item.ShortName;
                _familyCombo.Active = _families.IndexOf(_families.FirstOrDefault(family => family.Id == _item.FamilyId));
                _barCodeEntry.Text = _item.BarCode;

                if (_item.ImageId > 0)
                    _image.Pixbuf = AppCore.ImageResourceManager.GetImageResource(_item.ImageId).GetPixbuf();
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

        private void SaveItem(object sender, EventArgs e)
        {
            try
            {
                if (_imageChanged)
                {
                    if (string.IsNullOrEmpty(_filePath))
                    {
                        if (_item.ImageId > 0)
                        {
                            AppCore.ImageResourceManager.DeleteImageResource(_item.ImageId);
                            _item.ImageId = 0;
                        }
                    }
                    else
                    {
                        if (_item.ImageId > 0)
                        {
                            var imageResource = new ImageResource() { Id = _item.ImageId, FilePath = _filePath };
                            AppCore.ImageResourceManager.SaveImageResource(imageResource);
                        }
                        else
                        {
                            var imageResource = new ImageResource() { FilePath = _filePath };
                            AppCore.ImageResourceManager.SaveImageResource(imageResource);
                            _item.ImageId = imageResource.Id;
                        }
                    }
                }

                var index = _familyCombo.Active;

                _item.Name = _nameEntry.Text;
                _item.ShortName = _shortNameEntry.Text;
                _item.FamilyId = index < 0 ? 0 : _families[index].Id;
                _item.Price = (decimal)_priceEntry.Value;
                _item.Cost = (decimal)_costEntry.Value;
                _item.BarCode = _barCodeEntry.Text;
                AppCore.ItemManager.SaveItem(_item);
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
