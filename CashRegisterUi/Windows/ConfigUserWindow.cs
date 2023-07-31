using CashRegisterCore.Entities;
using CashRegisterCore.Managers;
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

namespace CashRegisterUi.Windows
{
    internal class ConfigUserWindow : Window
    {
        private User _user;
        private System.Action _refresh;

        private bool _imageChanged = false;
        private string _filePath;

        private Entry _nameEntry = new Entry();
        private Entry _passwordEntry = new Entry() { Visibility = false };
        private CheckButton _isAdminCheckBox = new CheckButton();

        private Gtk.Image _image = new Gtk.Image() { WidthRequest = 300, HeightRequest = 300 };

        public ConfigUserWindow(Window parent, User user, System.Action refresh)
            : base(WindowType.Toplevel)//: base()
        {
            _user = user;
            _refresh = refresh;

            this.Title = "Novo Utilizador";
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

            var passwordBox = new HBox(false, 2);
            passwordBox.PackStart(new Dummy { WidthRequest = 10 }, false, false, 0);
            passwordBox.PackStart(new Label("Senha :") { Xalign = 0, WidthRequest = 150 }, false, false, 0);
            passwordBox.PackStart(_passwordEntry, false, false, 0);

            var isAdminBox = new HBox(false, 2);
            isAdminBox.PackStart(new Dummy { WidthRequest = 10 }, false, false, 0);
            isAdminBox.PackStart(new Label("Administrador :") { Xalign = 0, WidthRequest = 150 }, false, false, 0);
            isAdminBox.PackStart(_isAdminCheckBox, false, false, 0);


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
            saveButton.Clicked += SaveUser;

            var cancelButton = new Button("Cancelar");
            cancelButton.Clicked += delegate { this.Destroy(); };

            var buttons = new HBox(false, 2);
            buttons.PackStart(new Dummy(), true, true, 0);
            buttons.PackStart(saveButton, false, false, 0);
            buttons.PackStart(cancelButton, false, false, 0);

            var box = new VBox(false, 2);
            box.PackStart(nameBox, true, true, 0);
            box.PackStart(passwordBox, true, true, 0);
            box.PackStart(isAdminBox, true, true, 0);
            box.PackStart(imgBox, true, true, 0);
            box.PackStart(buttons, false, true, 0);

            Add(box);

            if (_user.Id > 0)
            {
                this.Title = $"Editar utilizador: {_user.Username}";
                _nameEntry.Text = _user.Username;
                _isAdminCheckBox.Active = _user.IsAdmin;
                if (_user.ImageId > 0)
                    _image.Pixbuf = AppCore.ImageResourceManager.GetImageResource(_user.ImageId).GetPixbuf();
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

        private void SaveUser(object sender, EventArgs e)
        {
            try
            {
                if (_imageChanged)
                {
                    if (string.IsNullOrEmpty(_filePath))
                    {
                        if (_user.ImageId > 0)
                        {
                            AppCore.ImageResourceManager.DeleteImageResource(_user.ImageId);
                            _user.ImageId = 0;
                        }
                    }
                    else
                    {
                        if (_user.ImageId > 0)
                        {
                            var imageResource = new ImageResource() { Id = _user.ImageId, FilePath = _filePath };
                            AppCore.ImageResourceManager.SaveImageResource(imageResource);
                        }
                        else
                        {
                            var imageResource = new ImageResource() { FilePath = _filePath };
                            AppCore.ImageResourceManager.SaveImageResource(imageResource);
                            _user.ImageId = imageResource.Id;
                        }
                    }
                }

                _user.Username = _nameEntry.Text;

                if (_isAdminCheckBox.Active) 
                    _user.IsAdmin = true;
                else
                    _user.IsAdmin = false;

                AppCore.UserManager.SaveUser(_user);
                AppCore.UserManager.ChangePassword(_user.Id, _user.Password, _passwordEntry.Text);
                _user.Password = _passwordEntry.Text;

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
