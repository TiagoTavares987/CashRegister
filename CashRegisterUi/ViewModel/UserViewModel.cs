using CashRegisterCore.Entities;
using CashRegisterCore;
using Gtk;
using System.Linq;
using CashRegisterUi.Windows;

namespace CashRegisterUi.ViewModel
{
    internal class UserViewModel : EntityViewModel
    {
        public override string Title { get => "Utilizadores"; }

        public override void NewEntity(Window window)
            => new ConfigUserWindow(window, new User(), Refresh);

        public override void EditEntity(Window window, int id)
        {
            var user = AppCore.UserManager.GetUser(id);
            if (user != null)
                new ConfigUserWindow(window, user, Refresh);
        }

        public override void DeleteEntity(Window window, int id)
        {
            var user = AppCore.UserManager.GetUser(id);
            if (user != null)
            {
                AppCore.UserManager.DeleteUser(user.Id);
                Refresh();
            }
        }

        protected override void FillStore()
            => AppCore.UserManager.GetAll().ToList().ForEach(user => Store.AppendValues(user.Id, user.Username, user.Password));
    }
}
