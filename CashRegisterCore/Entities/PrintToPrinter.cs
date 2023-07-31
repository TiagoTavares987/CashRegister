using Database;
using System.Collections.Generic;
using ThermalPrinter.ContentConfig;
using ThermalPrinter.PrinterConfig;

namespace CashRegisterCore.Entities
{
    public class PrintToPrinter
    {
        [IsDbField]
        public int Id { get; set; }
        [IsDbField(isJsonObject: true)]
        public IPrinterConfig Config { get; set; }
        [IsDbField(isJsonObject: true)]
        public IEnumerable<ContentLine> Content { get; set; }
    }
}
