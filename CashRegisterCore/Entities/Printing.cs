using Database;
using System.Collections.Generic;
using ThermalPrinter.ContentConfig;

namespace CashRegisterCore.Entities
{
    [IsDbTable(nameof(Printing))]
    public class Printing
    {
        [IsDbField(true)]
        public int Id { get; set; }
        [IsDbField]
        public int PrinterId { get; set; }
        [IsDbField(isJsonObject: true)]
        public IEnumerable<ContentLine> Content { get; set; }
    }
}
