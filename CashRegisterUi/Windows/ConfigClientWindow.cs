using CashRegisterCore.Entities;
using CashRegisterCore.Managers;
using CashRegisterCore.Utils;
using CashRegisterCore;
using CashRegisterUi.Controls;
using CashRegisterUi.Languages;
using Gtk;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using CashRegisterUi.Utils;
using CashRegisterCore.Enumerators;
using System.Linq;

namespace CashRegisterUi.Windows
{
    internal class ConfigClientWindow : Window
    {
        private const string Other = "Outro";
        private Client _client;
        private System.Action _refresh;

        private List<(string, Country, string)> _countries = new List<(string, Country, string)>();
        private Dictionary<string, int> _countriesNames = new Dictionary<string, int>();
        private bool _imageChanged = false;
        private string _filePath;

        private Entry _nameEntry = new Entry();
        private ComboBox _nifCombo;
        private Entry _nifEntry = new Entry() { WidthRequest = 100 };
        private Entry _emailEntry = new Entry();
        private Entry _addressEntry = new Entry();
        private Entry _postalCodeEntry = new Entry();
        private Entry _cityEntry = new Entry();
        private Entry _countryEntry = new Entry();

        private Gtk.Image _image = new Gtk.Image() { WidthRequest = 300, HeightRequest = 300 };

        public ConfigClientWindow(Window parent, Client client, System.Action refresh)
            : base(WindowType.Toplevel)//: base()
        {
            _client = client;
            _refresh = refresh;

            this.Title = "Novo Cliente";
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

            FillCountryCombo();

            var nifBox = new HBox(false, 2);
            nifBox.PackStart(new Dummy { WidthRequest = 10 }, false, false, 0);
            nifBox.PackStart(new Label("Nif :") { Xalign = 0, WidthRequest = 150 }, false, false, 0);
            nifBox.PackStart(_nifCombo, false, false, 0);
            nifBox.PackStart(_nifEntry, false, false, 0);

            var emailBox = new HBox(false, 2);
            emailBox.PackStart(new Dummy { WidthRequest = 10 }, false, false, 0);
            emailBox.PackStart(new Label("Email :") { Xalign = 0, WidthRequest = 150 }, false, false, 0);
            emailBox.PackStart(_emailEntry, false, false, 0);

            var addressBox = new HBox(false, 2);
            addressBox.PackStart(new Dummy { WidthRequest = 10 }, false, false, 0);
            addressBox.PackStart(new Label("Endereço :") { Xalign = 0, WidthRequest = 150 }, false, false, 0);
            addressBox.PackStart(_addressEntry, false, false, 0);

            var postalCodeBox = new HBox(false, 2);
            postalCodeBox.PackStart(new Dummy { WidthRequest = 10 }, false, false, 0);
            postalCodeBox.PackStart(new Label("Código Postal :") { Xalign = 0, WidthRequest = 150 }, false, false, 0);
            postalCodeBox.PackStart(_postalCodeEntry, false, false, 0);

            var cityBox = new HBox(false, 2);
            cityBox.PackStart(new Dummy { WidthRequest = 10 }, false, false, 0);
            cityBox.PackStart(new Label("Cidade :") { Xalign = 0, WidthRequest = 150 }, false, false, 0);
            cityBox.PackStart(_cityEntry, false, false, 0);

            var countryBox = new HBox(false, 2);
            countryBox.PackStart(new Dummy { WidthRequest = 10 }, false, false, 0);
            countryBox.PackStart(new Label("País :") { Xalign = 0, WidthRequest = 150 }, false, false, 0);
            countryBox.PackStart(_countryEntry, false, false, 0);
            _countryEntry.Changed += delegate
            {
                if (_countriesNames.ContainsKey(_countryEntry.Text))
                    _nifCombo.Active = _countriesNames[_countryEntry.Text];
            };

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
            saveButton.Clicked += SaveClient;

            var cancelButton = new Button("Cancelar");
            cancelButton.Clicked += delegate { this.Destroy(); };

            var buttons = new HBox(false, 2);
            buttons.PackStart(new Dummy(), true, true, 0);
            buttons.PackStart(saveButton, false, false, 0);
            buttons.PackStart(cancelButton, false, false, 0);

            var box = new VBox(false, 2);
            box.PackStart(nameBox, true, true, 0);
            box.PackStart(nifBox, true, true, 0);
            box.PackStart(emailBox, true, true, 0);
            box.PackStart(addressBox, true, true, 0);
            box.PackStart(postalCodeBox, true, true, 0);
            box.PackStart(cityBox, true, true, 0);
            box.PackStart(countryBox, true, true, 0);
            box.PackStart(imgBox, true, true, 0);
            box.PackStart(buttons, false, true, 0);

            Add(box);

            if (_client.Id > 0)
            {
                this.Title = $"Editar client: {_client.Name}";
                _nameEntry.Text = _client.Name;
                _nifCombo.Active = _countries.IndexOf(_countries.FirstOrDefault(country => country.Item2 == _client.Address.CountryShort));
                _nifEntry.Text = _client.Nif;
                _emailEntry.Text = _client.Email;
                _addressEntry.Text = _client.Address.Address;
                _postalCodeEntry.Text = _client.Address.PostalCode;
                _cityEntry.Text = _client.Address.City;
                _countryEntry.Text = _client.Address.Country;

                if (_client.ImageId > 0)
                    _image.Pixbuf = AppCore.ImageResourceManager.GetImageResource(_client.ImageId).GetPixbuf();
            }

            ShowAll();
        }

        private void FillCountryCombo()
        {
            foreach (Country c in Enum.GetValues(typeof(Country)))
            {
                var name = c.GetName();
                if (c.Equals(Country.None))
                    _countries.Add((Other, c, name));
                else
                {
                    var item = (c.ToString(), c, name);
                    _countries.Add(item);
                    _countriesNames.Add(name, _countries.IndexOf(item));
                }
            }

            _nifCombo = new ComboBox(_countries.Select(c => c.Item1).ToArray()) { WidthRequest = 56 };
            _nifCombo.Changed += delegate
            {
                if (_nifCombo.Active >= 0 && !_nifCombo.ActiveText.Equals(Other))
                    _countryEntry.Text = _countries[_nifCombo.Active].Item3.ToString();
            };
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

        private void SaveClient(object sender, EventArgs e)
        {
            try
            {
                if (_imageChanged)
                {
                    if (string.IsNullOrEmpty(_filePath))
                    {
                        if (_client.ImageId > 0)
                        {
                            AppCore.ImageResourceManager.DeleteImageResource(_client.ImageId);
                            _client.ImageId = 0;
                        }
                    }
                    else
                    {
                        if (_client.ImageId > 0)
                        {
                            var imageResource = new ImageResource() { Id = _client.ImageId, FilePath = _filePath };
                            AppCore.ImageResourceManager.SaveImageResource(imageResource);
                        }
                        else
                        {
                            var imageResource = new ImageResource() { FilePath = _filePath };
                            AppCore.ImageResourceManager.SaveImageResource(imageResource);
                            _client.ImageId = imageResource.Id;
                        }
                    }
                }

                var index = _nifCombo.Active;

                _client.Name = _nameEntry.Text;
                _client.Address.CountryShort = index < 0 ? Country.None : _countries[index].Item2;
                _client.Nif = _nifEntry.Text;
                _client.Email = _emailEntry.Text;
                _client.Address.Address = _addressEntry.Text;
                _client.Address.PostalCode = _postalCodeEntry.Text;
                _client.Address.City = _cityEntry.Text;
                _client.Address.Country = _countryEntry.Text;

                AppCore.ClientManager.SaveClient(_client);
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
