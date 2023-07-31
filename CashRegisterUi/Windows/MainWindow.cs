using CashRegisterCore;
using CashRegisterCore.Entities;
using CashRegisterCore.Managers;
using CashRegisterUi.Controls;
using CashRegisterUi.Utils;
using CashRegisterUi.ViewModel;
using Gtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using ThermalPrinter.PrinterConfig;

using Item = CashRegisterCore.Entities.Item;

namespace CashRegisterUi.Windows
{
    internal class MainWindow : Window
    {
        private const string Families = "Familias";

        private string _barcode;

        private Family _selectedFamily;

        private Label _familyLabel = new Label(Families);
        private Label _userLabel = new Label();
        private Image _userImage = new Image() { WidthRequest = 32, HeightRequest = 32 };
        private VBox _matrix;
        private HBox _toolBar = new HBox(true, 1);
        private ToggleButton _configButton = new ToggleButton("Configurações") { HeightRequest = 80 };
        private List<Button> _adminButtons = new List<Button>();
        private List<Button> _buttons = new List<Button>();
        private Grid _listView;
        private Label _totalLabel = new Label("0.00 €") { Xalign = .5f };
        private MainWindow _instance;

        public MainWindow()
            : base(WindowType.Toplevel)
        {
            _instance = this;

            Title = "Cash Register";
            this.DefaultWidth = 1024;
            this.DefaultHeight = 800;
            WindowPosition = WindowPosition.Center;

            //WidthRequest = 1024;
            //HeightRequest = 800;
            //Resizable = false;

            this.KeyPressEvent += BarcodeParse;

            _matrix = new VBox(true, 1);
            OpenFamilies();

            _configButton.Clicked += (s, e) => { ChangeButtons(_configButton.Active); };
            _toolBar.PackStart(_configButton, true, true, 1);

            var terminalConfigButton = new Button("Terminal");
            terminalConfigButton.Clicked += delegate { };
            _adminButtons.Add(terminalConfigButton);
            var userConfigButton = new Button("Utilizador");
            userConfigButton.Clicked += delegate { new UserViewModel().OpenList(this); };
            _adminButtons.Add(userConfigButton);
            var familyConfigButton = new Button("Familia");
            familyConfigButton.Clicked += delegate { new FamilyViewModel().OpenList(this); };
            _adminButtons.Add(familyConfigButton);
            var itemConfigButton = new Button("Item");
            itemConfigButton.Clicked += delegate { new ItemViewModel().OpenList(this); };
            _adminButtons.Add(itemConfigButton);
            var clientConfigButton = new Button("Cliente");
            clientConfigButton.Clicked += delegate { new ClientViewModel().OpenList(this); };
            _adminButtons.Add(clientConfigButton);

            var a = new Button("Teste de impressão");
            a.Clicked += delegate { AppCore.PritingManager.PrintTest(); };
            _buttons.Add(a);
            var documentsListButton = new Button("Documentos");
            documentsListButton.Clicked += delegate { new ListDocumentWindow(this); };
            _buttons.Add(documentsListButton);
            var printOrEmailLastDocumentButton = new Button("Segunda Via");
            printOrEmailLastDocumentButton.Clicked += delegate { new PrintOrEmailWindow(this, AppCore.DocumentManager.GetLastInsertedDoc()); };
            _buttons.Add(printOrEmailLastDocumentButton);
            var drawerButton = new Button("Gaveta");
            drawerButton.Clicked += delegate { AppCore.PritingManager.OpenDrawer(); };
            _buttons.Add(drawerButton);
            var homeButton = new Button("Home");
            homeButton.Clicked += delegate { OpenFamilies(); };
            _buttons.Add(homeButton);

            _familyLabel.ScaleFont(1.5);
            var familyBox = new EventBox();
            familyBox.Add(_familyLabel);
            familyBox.ButtonReleaseEvent += delegate { OpenFamilies(); };

            var leftSide = new VBox(false, 1);
            leftSide.PackStart(familyBox, false, true, 0);
            leftSide.PackStart(_matrix, true, true, 0);
            leftSide.PackStart(_toolBar, false, true, 0);
            ChangeButtons(false);

            _userLabel.ScaleFont(1.3);
            var relogioLabel = new Label();
            relogioLabel.ScaleFont(2);
            var relogioTimer = new Timer() { Interval = 100, AutoReset = true, Enabled = true };
            relogioTimer.Elapsed += delegate
            {
                if (!DateTime.Now.ToString("HH:mm:ss").Equals(relogioLabel.Text))
                    Application.Invoke((o, e) => relogioLabel.Text = DateTime.Now.ToString("HH:mm:ss"));
            };
            FillUser();

            var userBox = new HBox(false, 0);
            userBox.PackStart(_userImage, false, false, 5);
            userBox.PackStart(_userLabel, false, false, 0);

            var userButton = new EventBox();
            userButton.Add(userBox);
            userButton.ButtonReleaseEvent += delegate { ChangeUser(); };

            var statusBox = new HBox(false, 1);
            statusBox.PackStart(userButton, false, false, 0);
            statusBox.PackStart(new Dummy(), true, true, 0);
            statusBox.PackStart(relogioLabel, false, true, 5);

            var columns = new List<(Type, string, XAlign)>
            {
                (typeof(int), "Ref", XAlign.Right),
                (typeof(string), "Nome", XAlign.Left),
                (typeof(int), "Quant", XAlign.Right),
                (typeof(decimal), "Preço unit", XAlign.Right),
                (typeof(string), "Taxa", XAlign.Right),
                (typeof(decimal), "Total", XAlign.Right),
            };

            _listView = new Grid(columns) { WidthRequest = 250 };

            var addQuantityButton = new Button() { Label = "+", HeightRequest = 80 };
            addQuantityButton.Clicked += delegate { ItemQuantityAdd(); };
            var removeQuantityButton = new Button() { Label = "-" };
            removeQuantityButton.Clicked += delegate { ItemQuantityRemove(); };
            var removeItemButton = new Button() { Label = "x" };
            removeItemButton.Clicked += delegate { ItemRemove(); };

            var listButtons = new HBox(true, 1);
            listButtons.PackStart(addQuantityButton, true, true, 0);
            listButtons.PackStart(removeQuantityButton, true, true, 0);
            listButtons.PackStart(removeItemButton, true, true, 0);

            var totalLabel = new Label("Total");
            totalLabel.ScaleFont(2.5);

            var totalLabelBox = new HBox(true, 0);
            totalLabelBox.PackStart(totalLabel, true, true, 0);
            totalLabelBox.PackStart(new Dummy(), true, true, 0);

            _totalLabel.ScaleFont(5);

            var totalBox = new VBox(false, 1);
            totalBox.PackStart(new Dummy(), false, true, 10);
            totalBox.PackStart(totalLabelBox, false, true, 0);
            totalBox.PackStart(_totalLabel, true, true, 0);
            totalBox.PackStart(new Dummy(), false, true, 10);

            var closeAccountButton = new Button("Fechar Conta") { HeightRequest = 80 };
            closeAccountButton.Clicked += delegate { CloseAccount(); };

            var rightSide = new VBox(false, 1);
            rightSide.PackStart(statusBox, false, true, 0);
            rightSide.PackStart(_listView, true, true, 0);
            rightSide.PackStart(listButtons, false, true, 0);
            rightSide.PackStart(totalBox, false, true, 0);
            rightSide.PackStart(closeAccountButton, false, true, 0);

            var hbox = new HBox(false, 1);
            hbox.PackStart(leftSide, true, true, 0);
            hbox.PackStart(rightSide, true, true, 0);

            Add(hbox);

            ShowAll();

            Destroyed += (sender, e) => Application.Quit();
        }

        private void BarcodeParse(object o, KeyPressEventArgs args)
        {
            if (args.Event.Key.Equals(Gdk.Key.Key_0))
                _barcode += 0;
            else if (args.Event.Key.Equals(Gdk.Key.Key_1))
                _barcode += 1;
            else if (args.Event.Key.Equals(Gdk.Key.Key_2))
                _barcode += 2;
            else if (args.Event.Key.Equals(Gdk.Key.Key_3))
                _barcode += 3;
            else if (args.Event.Key.Equals(Gdk.Key.Key_4))
                _barcode += 4;
            else if (args.Event.Key.Equals(Gdk.Key.Key_5))
                _barcode += 5;
            else if (args.Event.Key.Equals(Gdk.Key.Key_6))
                _barcode += 6;
            else if (args.Event.Key.Equals(Gdk.Key.Key_7))
                _barcode += 7;
            else if (args.Event.Key.Equals(Gdk.Key.Key_8))
                _barcode += 8;
            else if (args.Event.Key.Equals(Gdk.Key.Key_9))
                _barcode += 9;
            else
                _barcode = string.Empty;

            if (_barcode.Length == 13)
            {
                var item = AppCore.ItemManager.GetItemByBarcode(_barcode).FirstOrDefault();
                if (item != null)
                    RegisterItem(item.Id);
                _barcode = string.Empty;
            }
            else if(_barcode.Length > 13)
                _barcode = string.Empty;
        }

        private void OpenFamilies()
        {
            _selectedFamily = null;
            _familyLabel.Text = Families;

            foreach (var child in _matrix.Children)
                _matrix.Remove(child);

            var families = AppCore.FamilyManager.GetAll().ToList();
            families = families.OrderBy(x => x.ParentId).ToList();

            int i = 0;
            for (int l = 0; l < 4; l++)
            {
                var line = new HBox(true, 1);
                _matrix.PackStart(line, true, true, 0);
                for (int c = 0; c < 5; c++)
                {
                    if (i < families.Count)
                    {
                        var family = families[i];

                        Gdk.Pixbuf image = null;
                        if (families[i].ImageId > 0)
                            image = AppCore.ImageResourceManager.GetImageResource(families[i].ImageId)?.GetPixbuf();

                        Widget content;
                        if (image is null)
                            content = new Label(family.Name);
                        else
                        {
                            var vbox = new VBox(false, 1);
                            vbox.PackStart(new Gtk.Image(image), true, true, 0);
                            vbox.PackStart(new Label(family.Name), false, true, 10);
                            content = vbox;
                        }

                        var b = new Button(content);
                        b.Clicked += delegate { _selectedFamily = family; OpenItems(); };
                        line.PackStart(b, true, true, 0);
                    }
                    else
                        line.PackStart(new Dummy(), true, true, 0);

                    i++;
                }
            }

            _matrix.ShowAll();
        }

        private void OpenItems()
        {
            if (_selectedFamily is null)
                return;

            _familyLabel.Text = _selectedFamily.Name;
            foreach (var child in _matrix.Children)
                _matrix.Remove(child);

            var items = AppCore.ItemManager.GetFamilyItems(_selectedFamily.Id).ToList();

            int i = 0;
            for (int l = 0; l < 4; l++)
            {
                var line = new HBox(true, 1);
                _matrix.PackStart(line, true, true, 0);
                for (int c = 0; c < 5; c++)
                {
                    int id = -1;
                    string name = string.Empty;
                    string price = string.Empty;
                    if (i < items.Count)
                    {
                        id = items[i].Id;
                        name = items[i].Name;
                        price = items[i].Price.ToString("0.00 €");

                        Gdk.Pixbuf image = null;
                        if (items[i].ImageId > 0)
                            image = AppCore.ImageResourceManager.GetImageResource(items[i].ImageId)?.GetPixbuf();

                        var vbox = new VBox(false, 1);
                        if (!(image is null))
                            vbox.PackStart(new Gtk.Image(image), true, true, 0);

                        vbox.PackStart(new Label(name), false, true, 2);
                        vbox.PackStart(new Label(price), false, true, 2);

                        var b = new Button(vbox);
                        b.Clicked += delegate { RegisterItem(id); };
                        line.PackStart(b, true, true, 0);
                    }
                    else
                        line.PackStart(new Dummy(), true, true, 0);
                    i++;
                }
            }

            _matrix.ShowAll();
        }

        private void ChangeButtons(bool config)
        {
            _adminButtons.ForEach(b => _toolBar.Remove(b));
            _buttons.ForEach(b => _toolBar.Remove(b));

            if (config)
                _adminButtons.ForEach(b => _toolBar.PackStart(b, true, true, 1));
            else
                _buttons.ForEach(b => _toolBar.PackStart(b, true, true, 1));

            _toolBar.ShowAll();
        }

        private void RegisterItem(int id)
        {
            var item = AppCore.ItemManager.GetItem(id);
            if (item != null)
                _listView.AddRow(item.Id, item.Name, 1, item.Price, item.Tax.ToString("0%"), item.Price);

            TotalCalc();
        }

        private void ItemQuantityAdd()
        {
            var quantity = Convert.ToInt32(_listView.GetSelectedValue(2));
            var price = Convert.ToDecimal(_listView.GetSelectedValue(3));
            _listView.SetSelectedValue(2, ++quantity);
            _listView.SetSelectedValue(5, quantity * price);

            TotalCalc();
        }

        private void ItemQuantityRemove()
        {
            var quantity = Convert.ToInt32(_listView.GetSelectedValue(2));
            if (quantity == 1)
                return;
            var price = Convert.ToDecimal(_listView.GetSelectedValue(3));
            _listView.SetSelectedValue(2, --quantity);
            _listView.SetSelectedValue(5, quantity * price);

            TotalCalc();
        }

        private void ItemRemove()
        {
            _listView.RemoveSelectedRow();
            TotalCalc();
        }

        private void TotalCalc()
            => _totalLabel.Text = _listView.GetAll().Sum(line => Convert.ToDecimal(line[5])).ToString("0.00 €");

        private void FillUser()
        {
            var user = AppCore.UserManager.GetUser(AppCore.UserId);
            _userLabel.Text = user.Id + " - " + user.Username;
            _userImage.Pixbuf = AppCore.ImageResourceManager.GetImageResource(user.ImageId).GetPixbuf()?.ScaleSimple(32, 32, Gdk.InterpType.Bilinear);

            _configButton.Sensitive = user.IsAdmin;
        }

        private void ChangeUser()
        {
            var login = new LoginWindow(null);
            login.Modal = true;
            login.Parent = this;
            login.TransientFor = this;
            login.TypeHint = Gdk.WindowTypeHint.Dialog;
            login.WindowPosition = WindowPosition.CenterOnParent;
        }

        private void CloseAccount()
        {
            var documentBuilder = new DocumentBuilder();
            _listView.GetAll().ForEach(line => documentBuilder.AddItem(Convert.ToInt32(line[0]), Convert.ToInt32(line[2])));
            new CloseAccountViewModel(this, documentBuilder, RemoveAllLines);
        }

        private void RemoveAllLines()
        {
            _listView.Clean();
            TotalCalc();
        }

        private void InternalRefresh()
        {
            FillUser();

            if (_selectedFamily is null)
                OpenFamilies();
            else
                OpenItems();

            ChangeButtons(false);
        }

        public static void Refresh()
            => _instance.InternalRefresh();
    }
}
