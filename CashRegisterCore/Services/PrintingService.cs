using CashRegisterCore.Providers;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using ThermalPrinter;

namespace CashRegisterCore.Services
{
    internal class PrintingService
    {
        private readonly Timer _pollTimer = new Timer(1000) { AutoReset = true };
        private readonly PrintingProvider _provider = new PrintingProvider();
        private bool _printing;

        public PrintingService()
        {
            Pooling();
            _pollTimer.Elapsed += delegate { Pooling(); };
            _pollTimer.Enabled = true;
        }

        private void Pooling()
        {
            if (!_printing)
            {
                _printing = true;
                var bg = new BackgroundWorker();
                bg.DoWork += Print;
                bg.RunWorkerCompleted += delegate { _printing = false; };
                bg.RunWorkerAsync();
            }
        }

        private void Print(object sender, DoWorkEventArgs e)
        {
            var printing = _provider.GetAllFromTerminal()?.FirstOrDefault();
            if (printing != null)
            {
                new Printer(printing.Config).Print(printing.Content);
                _provider.DeletePrinting(new Entities.Printing() { Id = printing.Id });
            }
        }
    }
}
