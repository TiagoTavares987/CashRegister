
using CashRegisterCore;
using CashRegisterCore.Entities;
using CashRegisterUi.Windows;
using Gtk;
using System.Linq;

namespace CashRegisterUi.ViewModel
{
    internal class FamilyViewModel : EntityViewModel
    {
        public override string Title { get => "Familias"; }

        public override void NewEntity(Window window)
            => new ConfigFamilyWindow(window, new Family(), Refresh);

        public override void EditEntity(Window window, int id)
        {
            var family = AppCore.FamilyManager.GetFamily(id);
            if (family != null)
                new ConfigFamilyWindow(window, family, Refresh);
        }

        public override void DeleteEntity(Window window, int id)
        {
            var family = AppCore.FamilyManager.GetFamily(id);
            if (family != null)
            {
                AppCore.FamilyManager.DeleteFamily(family.Id);
                Refresh();
            }
        }

        protected override void FillStore()
            => AppCore.FamilyManager.GetAll().ToList().ForEach(family => Store.AppendValues(family.Id, family.Name));
    }
}
