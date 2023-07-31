using CashRegisterCore.Entities;
using System.Collections.Generic;

namespace CashRegisterCore.Providers
{
    internal class ItemProvider
    {
        public IEnumerable<Item> GetAll() => AppCore.Db.GetAll<Item>();

        public IEnumerable<Item> GetItemByBarcode(string barcode) => AppCore.Db.GetAll<Item>(new Dictionary<string, object>() { { nameof(Item.BarCode), barcode } });

        public IEnumerable<Item> GetFamilyItems(int familyId) => AppCore.Db.GetAll<Item>(new Dictionary<string, object>() { { nameof(Item.FamilyId), familyId } });

        public Item GetItem(int id) => AppCore.Db.GetEntity<Item>(id);

        public int InsertItem(Item item) => AppCore.Db.Insert(item);

        public int UpdateItem(Item item) => AppCore.Db.Update(item);

        public int DeleteItem(Item item) => AppCore.Db.Delete(item);

    }
}
