using CashRegisterCore.Entities;
using CashRegisterCore.Enumerators;
using System;

namespace CashRegisterCore.Utils
{
    internal class FinalCustomer : Client
    {
        public FinalCustomer()
        {
            Id = -1;
            Name = FillHiddenField;
            Nif = "999999990";
            Address = new FullAddress() { Country = Country.PT.GetName(), CountryShort = Country.PT };
        }

        public static string FillHiddenField => "*********";
    }
}
