using Database;

namespace CashRegisterCore.Entities
{
    [IsDbTable(nameof(Item))]
    public class Item
    {
        [IsDbField(true)]
        public int Id { get; set; }
        [IsDbField]
        public string Name { get; set; }
        [IsDbField]
        public string ShortName { get; set; }
        [IsDbField]
        public decimal Tax { get; set; }
        [IsDbField]
        public decimal Price { get; set; }
        [IsDbField]
        public decimal Cost { get; set; }
        [IsDbField]
        public string BarCode { get; set; }
        [IsDbField]
        public int FamilyId { get; set; }
        [IsDbField]
        public int ImageId { get; set; }

        public Item Clone() => (Item)MemberwiseClone();
    }
}
