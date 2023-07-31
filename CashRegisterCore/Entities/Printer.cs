using CashRegisterCore.Enumerators;
using Database;
using System.Net;
using ThermalPrinter.PrinterConfig;

namespace CashRegisterCore.Entities
{
    [IsDbTable(nameof(Printer))]
    public class Printer
    {
        [IsDbField(true)]
        public int Id { get; set; }
        [IsDbField]
        public int TerminalId { get; set; }
        [IsDbField]
        public PrinterType Type { get; set; }
        [IsDbField(isJsonObject: true)]
        public IPrinterConfig Config { get; set; }

    }
}
