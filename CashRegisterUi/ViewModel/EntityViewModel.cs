using CashRegisterUi.Windows;
using Gtk;

namespace CashRegisterUi.ViewModel
{
    internal abstract class EntityViewModel
    {
        public EntityViewModel()
        {
            Store = new ListStore(typeof(int), typeof(string));
            FillStore();
        }

        public abstract string Title { get; }

        public int Selected { get; set; }

        public ListStore Store { get; }

        public void OpenList(Window window) => new ListConfigWindow(window, this);

        public abstract void NewEntity(Window window);

        public abstract void EditEntity(Window window, int id);

        public abstract void DeleteEntity(Window window, int id);

        public void Refresh()
        {
            Store.Clear();
            FillStore();
            MainWindow.Refresh();
        }

        protected abstract void FillStore();
    }
}
