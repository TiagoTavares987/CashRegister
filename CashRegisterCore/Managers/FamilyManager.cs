using CashRegisterCore.Entities;
using CashRegisterCore.Providers;
using System;
using System.Collections.Generic;

namespace CashRegisterCore.Managers
{
    public class FamilyManager
    {
        private FamilyProvider _provider;

        public FamilyManager() => _provider = new FamilyProvider();

        public IEnumerable<Family> GetAll() => _provider.GetAll();

        public Family GetFamily(int id) => _provider.GetFamily(id);

        public void SaveFamily(Family family)
        {
            ValidateFamily(family);
            if (family.Id == 0)
            {
                var id = _provider.InsertFamily(family);
                if (id > 0)
                    family.Id = id;
            }
            else
                _provider.UpdateFamily(family);
        }

        public int DeleteFamily(int id)
        {
            Family family = GetFamily(id);
            if (family == null)
                throw new ArgumentNullException("Invalid family.");

            return _provider.DeleteFamily(family);
        }

        private void ValidateFamily(Family family)
        {
            if (string.IsNullOrWhiteSpace(family.Name))
                throw new ArgumentException("Invalid name.");
        }
    }
}
