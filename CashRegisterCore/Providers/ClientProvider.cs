using CashRegisterCore.Entities;
using System.Collections.Generic;

namespace CashRegisterCore.Providers
{
    internal class ClientProvider
    {
        public IEnumerable<Client> GetAll() => AppCore.Db.GetAll<Client>();

        public Client GetClient(int id) => AppCore.Db.GetEntity<Client>(id);

        public int InsertClient(Client client) => AppCore.Db.Insert(client);

        public int UpdateClient(Client client) => AppCore.Db.Update(client);

        public int DeleteClient(Client client) => AppCore.Db.Delete(client);
    }
}
