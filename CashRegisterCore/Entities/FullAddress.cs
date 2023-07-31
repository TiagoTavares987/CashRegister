using CashRegisterCore.Enumerators;

namespace CashRegisterCore.Entities
{
    public class FullAddress
    {
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public Country CountryShort { get; set; }

        public FullAddress Clone() => (FullAddress)MemberwiseClone();
    }
}
