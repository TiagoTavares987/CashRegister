namespace CashRegisterCore.Utils
{
    internal class Constant
    {
        public const string Company = "Cash Register, Lda";
        public const string CompanyNif = "123456789";
        public const string Address = "Praça 9 de Abril, nº 349";
        public const string PostalCode = "4249-004 Porto";
        public const string Phone = "Tel. 225 071 300 Tlm. 912 137 300";
        public const string PhoneWarning = "(Chamada para rede fixa/movel nacional)";

        public const string SoftwareProcess = "-Processado por programa\ncertificado n. ****/AT";

        public const string DocumentsPath = "Documents";

        public const decimal TaxRed = 0.06m;
        public const decimal TaxInt = 0.13m;
        public const decimal TaxNor = 0.23m;

        public const string SmtpAddress = "sandbox.smtp.mailtrap.io";
        public const int SmtpPort = 2525;
        public const string SmtpUser = "392da75e956865";
        public const string SmtpPass = "3f913aa44e4176";
        public const string CloseAccSubject = "Envio de fatura";
        public const string CloseAccBody = "Prezado cliente,\n\nVimos por este meio enviar o comprovativo da sua fatura que segue em anexo.\n\nCom os melhores cumprimentos,\n" + Company;

    }
}
