using System;
using System.Collections.Generic;

using ThermalPrinter.ContentConfig;
using ThermalPrinter.PrinterConfig;
using ThermalPrinter.Printers;

namespace ThermalPrinter
{
    public class Printer
    {
        private readonly IPrinter _printer;

        public Printer(IPrinterConfig printerConfig) => _printer = GetPrinter(printerConfig);

        public void Print(IEnumerable<ContentLine> content) => _printer.Print(content);

        private IPrinter GetPrinter(IPrinterConfig printerConfig)
        {
            if (printerConfig is SerialPrinterConfig portConfig)
                return new SerialPrinter(portConfig);

            if (printerConfig is DocumentPrinterConfig documentConfig)
                return new DocumentPrinter(documentConfig);

            throw new InvalidOperationException("Invalid printer configuration");
        }
    }
}
