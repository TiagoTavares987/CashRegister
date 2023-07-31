using CashRegisterCore.Entities;
using CashRegisterCore;
using CashRegisterUi.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gtk;

namespace CashRegisterUi.ViewModel
{
    internal class ClientViewModel : EntityViewModel
    {
        public override string Title { get => "Clientes"; }

        public override void NewEntity(Window window)
            => new ConfigClientWindow(window, new Client(), Refresh);

        public override void EditEntity(Window window, int id)
        {
            var client = AppCore.ClientManager.GetClient(id);
            if (client != null)
                new ConfigClientWindow(window, client, Refresh);
        }

        public override void DeleteEntity(Window window, int id)
        {
            var client = AppCore.ClientManager.GetClient(id);
            if (client != null)
            {
                AppCore.ClientManager.DeleteClient(client.Id);
                Refresh();
            }
        }

        protected override void FillStore()
            => AppCore.ClientManager.GetAll().ToList().ForEach(client => Store.AppendValues(client.Id, client.Name, client.Nif, client.Address));
    }
}
