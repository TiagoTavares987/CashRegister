using Database;

namespace CashRegisterCore.Entities
{
    [IsDbTable(nameof(Family))]
    public class Family
    {
        [IsDbField(true)]
        public int Id { get; set; }
        [IsDbField]
        public int ParentId { get; set; }
        [IsDbField]
        public string Name { get; set; }
        [IsDbField]
        public int ImageId { get; set; }

        public Family Clone() => (Family)MemberwiseClone();
    }
}
