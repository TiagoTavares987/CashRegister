using CashRegisterCore.Entities;
using System.Collections.Generic;

namespace CashRegisterCore.Providers
{
    internal class FamilyProvider
    {
        public IEnumerable<Family> GetAll() => AppCore.Db.GetAll<Family>();

        public Family GetFamily(int id) => AppCore.Db.GetEntity<Family>(id);

        public int InsertFamily(Family family) => AppCore.Db.Insert(family);

        public int UpdateFamily(Family family) => AppCore.Db.Update(family);

        public int DeleteFamily(Family family) => AppCore.Db.Delete(family);
    }
}
