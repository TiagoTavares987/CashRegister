using CashRegisterCore.Entities;
using CashRegisterCore.Providers;
using System;
using System.Collections.Generic;
using ThermalPrinter.Utils;

namespace CashRegisterCore.Managers
{
    public class ItemManager
    {
        private ItemProvider _provider;

        public ItemManager() => _provider = new ItemProvider();

        public IEnumerable<Item> GetAll() => _provider.GetAll();

        public IEnumerable<Item> GetItemByBarcode(string barcode) => _provider.GetItemByBarcode(barcode);

        public Item GetItem(int id) => _provider.GetItem(id);

        public IEnumerable<Item> GetFamilyItems(int familyId) => _provider.GetFamilyItems(familyId);

        public void SaveItem(Item item)
        {
            ValidateItem(item);
            if (item.Id == 0)
            {
                var id = _provider.InsertItem(item);
                if (id > 0)
                    item.Id = id;
            }
            else
                _provider.UpdateItem(item);
        }

        public int DeleteItem(int id)
        {
            Item item = GetItem(id);
            if (item == null)
                throw new ArgumentNullException("Invalid item.");

            return _provider.DeleteItem(item);
        }

        private void ValidateItem(Item item)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
                throw new ArgumentException("Invalid name.");
            if (string.IsNullOrWhiteSpace(item.ShortName))
                throw new ArgumentException("Invalid short name.");
            if (item.FamilyId <= 0)
                throw new ArgumentException("Invalid family.");
            if (!Barcode.Verify(BarcodeType.EAN_13, item.BarCode))
                throw new ArgumentException("Invalid barcode.");
            if (item.Price <= 0)
                throw new ArgumentException("Invalid price.");
            if (item.Cost <= 0)
                throw new ArgumentException("Invalid cost.");
        }

    }
}
