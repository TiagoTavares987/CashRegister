using CashRegisterCore.Entities;
using CashRegisterCore.Providers;
using CashRegisterCore.Utils;
using System.IO;
using System.Net;
using System.Net.Mail;
using ThermalPrinter.PrinterConfig;

namespace CashRegisterCore.Managers
{
    public class PrintingManager
    {
        //private PrinterProvider _printerProvider;
        private PrintingProvider _printingProvider;

        public PrintingManager()
        {
            //_printerProvider = new PrinterProvider();
            _printingProvider = new PrintingProvider();
        }

        //public void PrintTest(IPrinterConfig config)
        //=> new ThermalPrinter.Printer(config).Print(Resources.GetPrintTestLayout());

        public void PrintTest()
        {
            new ThermalPrinter.Printer(new SerialPrinterConfig()).Print(Resources.GetPrintTestLayout());

            if (!Directory.Exists(Constant.DocumentsPath))
                Directory.CreateDirectory(Constant.DocumentsPath);

            //_printingProvider.InsertPrinting(new Printing() {  });

            new ThermalPrinter.Printer(new DocumentPrinterConfig() { PdfPath = Path.Combine(Constant.DocumentsPath, "TestPrint.pdf") }).Print(Resources.GetPrintTestLayout());
        }

        public void PrintKitchen(Document document)
        {
            var printContent = Resources.GetKitchenLayout(document);
            //ir buscar lista de impressoras de cozinha do terminal
            //inserir na tabela printing para cada uma
        }

        public void Print(Document document)
        {
            var printContent = Resources.GetCloseAccountLayout(document);
            //ir buscar lista de impressoras de faturaçao do terminal 
            //inserir na tabela printing para cada uma

            new ThermalPrinter.Printer(new DocumentPrinterConfig() { PdfPath = Path.Combine(Constant.DocumentsPath, "test.pdf") }).Print(printContent);
            new ThermalPrinter.Printer(new SerialPrinterConfig()).Print(printContent);

            if (!document.Printed)
                AppCore.DocumentManager.UpdateDocumentPrinted(document.Id);
        }

        public void Email(Document document, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

            if (!Directory.Exists(Constant.DocumentsPath))
                Directory.CreateDirectory(Constant.DocumentsPath);

            var docName = document.DocumentType + "_" + document.SellerId + "_" + document.Number.ToString().PadLeft(3, '0') + ".pdf";
            var path = Path.Combine(Constant.DocumentsPath, docName);

            new ThermalPrinter.Printer(new DocumentPrinterConfig() { PdfPath = path }).Print(Resources.GetCloseAccountLayout(document));

            var server = new SmtpClient(Constant.SmtpAddress, Constant.SmtpPort)
            {
                Credentials = new NetworkCredential(Constant.SmtpUser, Constant.SmtpPass),
                EnableSsl = true
            };

            var mail = new MailMessage(email, email, Constant.CloseAccSubject, Constant.CloseAccBody);
            mail.Attachments.Add(new Attachment(path));
            server.Send(mail);

            if (!document.Printed)
                AppCore.DocumentManager.UpdateDocumentPrinted(document.Id);
        }

        public void OpenDrawer() => new ThermalPrinter.Printer(new SerialPrinterConfig()).Print(Resources.GetOpenDrawerLayout());
    }
}
