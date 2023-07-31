using Database;

namespace CashRegisterCore.Entities
{
    [IsDbTable(nameof(Client))]
    public class Client
    {
        [IsDbField(true)]
        public int Id { get; set; }
        [IsDbField]
        public string Name { get; set; }
        [IsDbField]
        public string Nif { get; set; }
        [IsDbField(isJsonObject:true)]
        public FullAddress Address { get; set; } = new FullAddress();
        [IsDbField]
        public string Email { get; set; }
        [IsDbField]
        public int ImageId { get; set; }

        public Client Clone()
        {
            var clone = (Client)MemberwiseClone();
            clone.Address = Address.Clone();
            return clone;
        }
    }
}
