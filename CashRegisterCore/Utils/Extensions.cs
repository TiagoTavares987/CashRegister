using CashRegisterCore.Enumerators;
using System.Collections.Generic;

namespace CashRegisterCore.Utils
{
    public static class Extensions
    {
        public static string GetName(this Country country)
        {
            switch (country)
            {
                case Country.PT:
                    return "Portugal";

                default:
                    return null;
            }
        }

        public static Dictionary<string, Country> GetNames(this Country country)
        {
            var result = new Dictionary<string, Country>();
            foreach (Country v in typeof(Country).GetEnumValues())
            {
                var x = v.GetName();
                if(x != null)
                    result.Add(x, v);
            }
            return result;
        }
    }
}
