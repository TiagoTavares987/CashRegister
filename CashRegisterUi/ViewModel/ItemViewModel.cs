using CashRegisterCore.Entities;
using CashRegisterCore;
using CashRegisterUi.Windows;
using System.Linq;
using Gtk;

namespace CashRegisterUi.ViewModel
{
    internal class ItemViewModel : EntityViewModel
    {
        public override string Title { get => "Items"; }

        public override void NewEntity(Window window)
            => new ConfigItemWindow(window, new CashRegisterCore.Entities.Item(), Refresh);

        public override void EditEntity(Window window, int id)
        {
            var item = AppCore.ItemManager.GetItem(id);
            if (item != null)
                new ConfigItemWindow(window, item, Refresh);
        }

        public override void DeleteEntity(Window window, int id)
        {
            var item = AppCore.ItemManager.GetItem(id);
            if (item != null)
            {
                AppCore.ItemManager.DeleteItem(item.Id);
                Refresh();
            }
        }

        protected override void FillStore()
            => AppCore.ItemManager.GetAll().ToList().ForEach(item => Store.AppendValues(item.Id, item.Name, item.ShortName, item.Price, item.Cost, item.BarCode));
    }
}
