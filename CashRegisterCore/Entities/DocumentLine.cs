using Database;

namespace CashRegisterCore.Entities
{
    [IsDbTable(nameof(DocumentLine))]
    public class DocumentLine
    {
        [IsDbField(true)]
        public int Id { get; set; }
        [IsDbField]
        public int DocumentId { get; set; }
        [IsDbField]
        public string ItemRef { get; set; }
        [IsDbField]
        public string ItemName { get; set; }
        [IsDbField]
        public string ItemShortName { get; set; }
        [IsDbField]
        public decimal ItemPrice { get; set; }
        [IsDbField]
        public int Quantity { get; set; }
        [IsDbField]
        public decimal Tax { get; set; }
        [IsDbField]
        public decimal Total { get; set; }

        public DocumentLine Clone() => (DocumentLine)MemberwiseClone();
    }
}
