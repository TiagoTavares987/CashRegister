using Database;
using System;
using System.Collections.Generic;

namespace CashRegisterCore.Entities
{
    [IsDbTable(nameof(Document))]
    public class Document
    {
        [IsDbField(true)]
        public int Id { get; set; }
        [IsDbField]
        public string DocumentType { get; set; }
        [IsDbField]
        public int Serie { get; set; }
        [IsDbField]
        public int Number { get; set; }
        [IsDbField]
        public int SellerId { get; set; }
        [IsDbField]
        public string SellerName { get; set; }
        [IsDbField]
        public int ClientId { get; set; }
        [IsDbField]
        public string ClientName { get; set; }
        [IsDbField]
        public string ClientNif { get; set; }
        [IsDbField(isJsonObject: true)]
        public FullAddress ClientAddress { get; set; }
        [IsDbField]
        public decimal TotalTaxes { get; set; }
        [IsDbField]
        public decimal Total { get; set; }
        [IsDbField]
        public DateTime Date { get; set; }
        [IsDbField]
        public bool Printed { get; set; }

        public string QrCode { get; set; }
        public string Hash { get; set; }
        public List<DocumentLine> Lines { get; } = new List<DocumentLine>();

        public Document Clone()
        {
            var clone = (Document)MemberwiseClone();
            clone.ClientAddress = ClientAddress.Clone();
            foreach(var line in Lines)
                clone.Lines.Add(line.Clone());

            return clone;
        }
    }
}
