using CashRegisterCore.Entities;
using System.Collections.Generic;

namespace CashRegisterCore.Providers
{
    internal class PrintingProvider
    {
        public IEnumerable<PrintToPrinter> GetAllFromTerminal()
        {
            var query = @"select printing.Id, printer.Config, printing.Content from printing
                          left join printer
                          on printer.Id = printing.PrinterId
                          where printer.TerminalId = @p_TerminalId";

            return AppCore.Db.GetList<PrintToPrinter>(query, new Dictionary<string, object>() { { $"@p_{nameof(AppCore.TerminalId)}", AppCore.TerminalId } });
        }

        public int InsertPrinting(Printing printing) => AppCore.Db.Insert(printing);

        public int DeletePrinting(Printing printing) => AppCore.Db.Delete(printing);
    }
}
