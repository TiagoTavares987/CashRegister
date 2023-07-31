using CashRegisterCore.Entities;
using CashRegisterCore.Providers;
using System.Collections.Generic;
using System;
using CashRegisterCore.Utils;

namespace CashRegisterCore.Managers
{
    public class ClientManager
    {
        private ClientProvider _provider;

        public ClientManager() => _provider = new ClientProvider();

        public IEnumerable<Client> GetAll() => _provider.GetAll();

        public Client GetClient(int id) => _provider.GetClient(id);

        public void SaveClient(Client client)
        {
            ValidateClient(client);
            if (client.Id == 0)
            {
                var id = _provider.InsertClient(client);
                if (id > 0)
                    client.Id = id;
            }
            else
                _provider.UpdateClient(client);
        }

        public int DeleteClient(int id)
        {
            Client client = GetClient(id);
            if (client == null)
                throw new ArgumentNullException("Invalid client.");

            return _provider.DeleteClient(client);
        }

        private void ValidateClient(Client client)
        {
            if (string.IsNullOrWhiteSpace(client.Name))
                throw new ArgumentException("Invalid name.");
            if (!Validate.Nif(client.Address.CountryShort, client.Nif))
                throw new ArgumentException("Invalid nif.");
            if (client.Address == null)
                throw new ArgumentException("Invalid address.");
        }
    }
}
