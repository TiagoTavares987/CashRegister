using System.Collections.Generic;

using ThermalPrinter.ContentConfig;

namespace ThermalPrinter.PrinterConfig
{
    internal interface IPrinter
    {
        void Print(IEnumerable<ContentLine> content);
    }
}
