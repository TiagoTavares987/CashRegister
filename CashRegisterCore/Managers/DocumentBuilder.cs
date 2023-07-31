using CashRegisterCore.Entities;
using CashRegisterCore.Utils;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CashRegisterCore.Managers
{
    public class DocumentBuilder
    {
        private Document _document = new Document();
        private Client _client;

        public bool HasLines => _document.Lines.Any();
        public bool HasClient => !string.IsNullOrEmpty(_document.ClientNif);

        public void AddItem(int itemRef, int quantity)
        {
            var item = AppCore.ItemManager.GetItem(itemRef);
            if (item != null)
            {
                _document.Lines.Add(new DocumentLine()
                {
                    ItemRef = item.Id.ToString(),
                    ItemName = item.Name,
                    ItemShortName = item.ShortName,
                    ItemPrice = item.Price,
                    Quantity = quantity,
                    Tax = item.Tax,
                    Total = item.Price * quantity
                });
            }
        }

        public void SetFinalCustomer()
            => SetClient(new FinalCustomer());

        public void SetClient(Client client)
        {
            _client = client;
            _document.ClientId = client.Id;
            _document.ClientNif = client.Nif;
            _document.ClientName = client.Name;
            _document.ClientAddress = client.Address;
        }

        public Document GetDocument()
        {
            var user = AppCore.UserManager.GetUser(AppCore.UserId);
            if (user != null)
            {
                _document.SellerId = user.Id;
                _document.SellerName = user.Username;
            }
            _document.TotalTaxes = 0;
            _document.Total = 0;
            _document.Lines.ForEach(line =>
            {
                _document.TotalTaxes += (line.Total * line.Tax / 100);
                _document.Total += line.Total;
            });

            _document.DocumentType = "FS";
            _document.Serie = AppCore.TerminalId;
            _document.Date = DateTime.Now;

            SetQrCode(_document);
            SetHash(_document);

            return _document;
        }

        public Client GetClient() => _client;

        public static void SetQrCode(Document document)
        {
            var nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            StringBuilder sb = new StringBuilder();
            sb.Append($"A:{Constant.CompanyNif}*");
            sb.Append($"B:{document.ClientNif}*");
            sb.Append($"C:{document.ClientAddress.CountryShort}*");
            sb.Append($"D:{document.DocumentType}*");
            sb.Append($"E:N*");
            sb.Append($"F:{document.Date.ToString("yyyyMMdd")}*");
            sb.Append($"G:{document.DocumentType} {document.Serie}/{document.Number} *");
            sb.Append($"H:ATCUD-0-{document.Number}*");

            var tRed = 0m;
            var tInt = 0m;
            var tNor = 0m;
            foreach (var line in document.Lines)
            {
                if (Constant.TaxNor.Equals(line.Tax))
                    tNor += line.Total;
                else if (Constant.TaxInt.Equals(line.Tax))
                    tInt += line.Total;
                else if (Constant.TaxRed.Equals(line.Tax))
                    tRed += line.Total;
            }

            sb.Append($"I1:PT*");
            if (tRed > 0)
            {
                var b = tRed / (1 + Constant.TaxRed);
                var i = tRed - b;
                sb.Append($"I3:{Math.Round(b, 2, MidpointRounding.AwayFromZero).ToString("0.00", nfi)}*I4:{Math.Round(i, 2, MidpointRounding.AwayFromZero).ToString("0.00", nfi)}*");
            }
            if (tInt > 0)
            {
                var b = tInt / (1 + Constant.TaxRed);
                var i = tInt - b;
                sb.Append($"I5:{Math.Round(b, 2, MidpointRounding.AwayFromZero).ToString("0.00", nfi)}*I6:{Math.Round(i, 2, MidpointRounding.AwayFromZero).ToString("0.00", nfi)}*");
            }
            if (tNor > 0)
            {
                var b = tNor / (1 + Constant.TaxRed);
                var i = tNor - b;
                sb.Append($"I7:{Math.Round(b, 2, MidpointRounding.AwayFromZero).ToString("0.00", nfi)}*I8:{Math.Round(i, 2, MidpointRounding.AwayFromZero).ToString("0.00", nfi)}*");
            }

            sb.Append($"N:{document.TotalTaxes.ToString("0.00", nfi)}*");
            sb.Append($"O:{document.Total.ToString("0.00", nfi)}*");
            sb.Append($"P:0*");
            //sb.Append($"Q:{Signature}*");
            sb.Append($"R:00000000*");
            document.QrCode = sb.ToString();
        }

        public static void SetHash(Document document)
        {
            var lastDocument = AppCore.DocumentManager.GetDocumentHeader(document.Id - 1) ?? document;
            document.Hash = string.Empty;
            var l = document.Lines.First();
            var x = Convert.ToBase64String(Encoding.UTF8.GetBytes(document.Date.ToString("ssmmHH") + document.TotalTaxes + document.Total + lastDocument.Date.ToString("ssmmHH") + lastDocument.TotalTaxes + lastDocument.Total));

            var hash = string.Empty;
            try
            {
                hash += x.Substring(0, 1);
                hash += x.Substring(10, 1);
                hash += x.Substring(20, 1);
                hash += x.Substring(30, 1);
                document.Hash = hash;
            }
            catch { }
        }
    }
}
