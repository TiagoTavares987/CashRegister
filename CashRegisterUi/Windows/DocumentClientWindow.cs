using CashRegisterCore.Entities;
using CashRegisterCore;
using CashRegisterUi.Controls;
using CashRegisterUi.Languages;
using CashRegisterCore.Utils;
using Gtk;
using System;
using System.Collections.Generic;
using CashRegisterCore.Enumerators;
using System.Linq;

namespace CashRegisterUi.Windows
{
    internal class DocumentClientWindow : Window
    {
        private const string Other = "Outro";
        private List<Client> _clients;
        private Client _client;
        private System.Action<Client> _getClient;

        private List<(string, Country, string)> _countries = new List<(string, Country, string)>();
        private Dictionary<string, int> _countriesNames = new Dictionary<string, int>();

        private Entry _nameEntry = new Entry();
        private ComboBox _nifCombo;
        private Entry _nifEntry = new Entry() { WidthRequest = 100 };
        private Entry _emailEntry = new Entry();
        private Entry _addressEntry = new Entry();
        private Entry _postalCodeEntry = new Entry();
        private Entry _cityEntry = new Entry();
        private Entry _countryEntry = new Entry();

        public DocumentClientWindow(Window parent, Action<Client> getClient)
            : base(WindowType.Toplevel)//: base()
        {
            _getClient = getClient;

            this.Title = "Documento cliente";
            this.Modal = true;
            this.Parent = parent;
            this.DefaultWidth = 330;
            this.DefaultHeight = 180;
            this.TransientFor = parent;
            this.TypeHint = Gdk.WindowTypeHint.Dialog;
            this.WindowPosition = WindowPosition.CenterOnParent;

            FillCountryCombo();

            var nifBox = new HBox(false, 2);
            nifBox.PackStart(new Dummy { WidthRequest = 10 }, false, false, 0);
            nifBox.PackStart(new Label("Nif :") { Xalign = 0, WidthRequest = 150 }, false, false, 0);
            nifBox.PackStart(_nifCombo, false, false, 0);
            nifBox.PackStart(_nifEntry, false, false, 0);

            _nifEntry.Changed += delegate { LoadClient(); };

            var nameBox = new HBox(false, 2);
            nameBox.PackStart(new Dummy { WidthRequest = 10 }, false, false, 0);
            nameBox.PackStart(new Label(Translate.Language.Name + ":") { Xalign = 0, WidthRequest = 150 }, false, false, 0);
            nameBox.PackStart(_nameEntry, false, false, 0);

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
            countryBox.PackStart(new Label("Pais :") { Xalign = 0, WidthRequest = 150 }, false, false, 0);
            countryBox.PackStart(_countryEntry, false, false, 0);
            _countryEntry.Changed += delegate
            {
                if (_countriesNames.ContainsKey(_countryEntry.Text))
                    _nifCombo.Active = _countriesNames[_countryEntry.Text];
            };

            var setButton = new Button("Confirmar");
            setButton.Clicked += SetClient;

            var cancelButton = new Button("Cancelar");
            cancelButton.Clicked += delegate { this.Destroy(); };

            var buttons = new HBox(false, 2);
            buttons.PackStart(new Dummy(), true, true, 0);
            buttons.PackStart(setButton, false, false, 0);
            buttons.PackStart(cancelButton, false, false, 0);

            var box = new VBox(false, 2);
            box.PackStart(nifBox, true, true, 0);
            box.PackStart(nameBox, true, true, 0);
            box.PackStart(emailBox, true, true, 0);
            box.PackStart(addressBox, true, true, 0);
            box.PackStart(postalCodeBox, true, true, 0);
            box.PackStart(cityBox, true, true, 0);
            box.PackStart(countryBox, true, true, 0);
            box.PackStart(buttons, false, true, 0);

            Add(box);
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

        private void LoadClient()
        {
            if (_clients is null)
                _clients = AppCore.ClientManager.GetAll().ToList();

            _client = _clients.FirstOrDefault(c => c.Nif.Equals(_nifEntry.Text));

            if (_client is null)
                return;

            _nameEntry.Text = _client.Name;
            _emailEntry.Text = _client.Email;
            _addressEntry.Text = _client.Address.Address;
            _postalCodeEntry.Text = _client.Address.PostalCode;
            _cityEntry.Text = _client.Address.City;
            _countryEntry.Text = _client.Address.Country;
        }

        private void SetClient(object sender, EventArgs e)
        {
            var client = _client is null ? new Client() { Id = -1 } : _client;

            var index = _nifCombo.Active;
            client.Name = _nameEntry.Text;
            client.Nif = _nifEntry.Text;
            client.Email = _emailEntry.Text;

            client.Address.Address = _addressEntry.Text;
            client.Address.PostalCode = _postalCodeEntry.Text;
            client.Address.City = _cityEntry.Text;
            if(index>-1){
                client.Address.Country = _countries[index].Item3;
                client.Address.CountryShort = index < 0 ? Country.None : _countries[index].Item2;
            }

            _getClient(client);

            this.Destroy();
        }
    }
}
