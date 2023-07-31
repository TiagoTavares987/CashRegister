using CashRegisterCore;
using CashRegisterCore.Entities;
using CashRegisterUi.Controls;
using CashRegisterUi.Languages;
using CashRegisterUi.Properties;
using CashRegisterUi.Utils;
using Gtk;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;


namespace CashRegisterUi.Windows
{
    internal class LoginWindow : Window
    {
        private List<User> users = AppCore.UserManager.GetAll().ToList();

        public LoginWindow(System.Action realized)
            : base(WindowType.Toplevel)
        {
            Title = "Cash Register";
            Resizable = false;
            Decorated = false;
            WindowPosition = WindowPosition.Center;

            var error = new Label();
            error.ModifyForeground(new Gdk.Color(255, 0, 0));

            var selectedUser = realized == null ? users.FirstOrDefault(x => x.Id == AppCore.UserId) : users.First();

            var image = new Image(AppCore.ImageResourceManager.GetImageResource(selectedUser.ImageId).GetPixbuf()?.ScaleSimple(100, 100, Gdk.InterpType.Bilinear)) { WidthRequest = 100, HeightRequest = 100 };

            var combo = new ComboBox(users.Select(x => x.Username).ToArray()) { Active = users.IndexOf(selectedUser) };
            combo.Changed += (sender, e) =>
            {
                error.Text = string.Empty;

                var index = combo.Active;
                if (index < 0)
                    return;

                var user = users[index];
                if (user is null)
                    return;

                image.Pixbuf = AppCore.ImageResourceManager.GetImageResource(user.ImageId).GetPixbuf()?.ScaleSimple(100, 100, Gdk.InterpType.Bilinear);
            };

            var passwordEntry = new Entry() { Visibility = false };
            passwordEntry.Changed += (sender, e) => error.Text = string.Empty;
            passwordEntry.Realized += (sender, e) => passwordEntry.GrabFocus();

            var vbox = new VBox(false, 0);
            vbox.PackStart(new Dummy(), false, false, 30);
            vbox.PackStart(image, false, false, 10);
            vbox.PackStart(new Label(Translate.Language.Username), false, false, 10);
            vbox.PackStart(combo, false, false, 10);
            vbox.PackStart(new Label(Translate.Language.Password), false, false, 10);
            vbox.PackStart(passwordEntry, false, false, 10);

            var loginButton = new Button() { Label = Translate.Language.Login, WidthRequest = 100, HeightRequest = 40 };
            passwordEntry.KeyReleaseEvent += (sender, e) => { if (e.Event.Key.Equals(Gdk.Key.Return)) loginButton.Click(); };
            loginButton.Clicked += (sender, e) =>
            {
                var index = combo.Active;
                if (index < 0)
                    return;

                var user = users[index];
                if (user is null)
                    return;

                if (AppCore.UserManager.ValidateLogin(user.Id, passwordEntry.Text))
                {
                    if (realized is null)
                        MainWindow.Refresh();
                    else
                        new MainWindow();
                    Destroy();
                }
                else
                {
                    error.Text = "Credenciais Inválidas.";
                }
            };
            var exitButton = new Button() { Label = Translate.Language.Exit, WidthRequest = 100, HeightRequest = 40 };
            exitButton.Clicked += (sender, e) =>
            {
                Destroy();
                Application.Quit();
            };

            var buttons = new HBox(true, 10);
            buttons.PackStart(loginButton, false, false, 0);
            buttons.PackStart(exitButton, false, false, 0);

            vbox.PackStart(buttons, false, false, 0);
            vbox.PackStart(error, false, false, 20);

            var hbox = new HBox(true, 0);
            using (var stream = new MemoryStream())
            {
                ((System.Drawing.Bitmap)Resources.ResourceManager.GetObject("logo")).Save(stream, ImageFormat.Png);
                stream.Position = 0;
                hbox.PackStart(new Image(new Gdk.Pixbuf(stream)), false, false, 10);
            }
            hbox.PackStart(vbox, false, false, 10);
            Add(hbox);

            if (realized != null)
                Realized += delegate { realized(); };

            ShowAll();
        }
    }
}
