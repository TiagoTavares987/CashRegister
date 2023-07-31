using CashRegisterCore.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ThermalPrinter.ContentConfig;

namespace CashRegisterCore.Utils
{
    internal class Resources
    {
        private const int Chars = 42;
        private const string LogoBase64 = "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";
        private static string SingleSeparator = new string('-', Chars);
        private static string DoubleSeparator = new string('=', Chars);

        public static Bitmap Logo
        {
            get
            {
                using (var ms = new MemoryStream(Convert.FromBase64String(LogoBase64)))
                    return new Bitmap(ms);
            }
        }

        public static IEnumerable<ContentLine> GetPrintTestLayout()
        {
            var content = new List<ContentLine>();
            content.Add(ContentLine.AddText("Printing test"));

            content.Add(ContentLine.SetInstruction(ContentLineInstruction.AlignLeft));
            content.Add(ContentLine.AddText("left"));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.AlignCenter));
            content.Add(ContentLine.AddText("center"));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.AlignRight));
            content.Add(ContentLine.AddText("right"));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.AlignLeft));

            content.Add(ContentLine.SetInstruction(ContentLineInstruction.FontWeightNormal));
            content.Add(ContentLine.AddText("normal"));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.FontWeightBold));
            content.Add(ContentLine.AddText("bold"));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.FontWeightNormal));

            content.Add(ContentLine.SetInstruction(ContentLineInstruction.FontSize8));
            content.Add(ContentLine.AddText("size 8"));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.FontSize10));
            content.Add(ContentLine.AddText("size 10"));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.FontSize12));
            content.Add(ContentLine.AddText("size 12"));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.FontSize14));
            content.Add(ContentLine.AddText("size 14"));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.FontSize10));

            //content.Add(ContentLine.AddBarcode("123456789012"));
            content.Add(ContentLine.AddQrCode("Test QrCode Test QrCode Test QrCode Test QrCode Test QrCode"));
            content.Add(ContentLine.AddImage(Logo)); content.Add(ContentLine.SetInstruction(ContentLineInstruction.ChangeLine));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.ChangeLine));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.ChangeLine));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.ChangeLine));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.PaperCut));

            return content;
        }

        public static IEnumerable<ContentLine> GetKitchenLayout(Document document)
        {
            var content = new List<ContentLine>();
            return content;
        }

        public static IEnumerable<ContentLine> GetCloseAccountLayout(Document document)
        {
            var nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            var content = new List<ContentLine>();
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.AlignCenter));
            content.Add(ContentLine.AddImage(Logo));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.AlignCenter));
            content.Add(ContentLine.AddText(Constant.Company));
            content.Add(ContentLine.AddText("Contribuinte n. " + Constant.CompanyNif));
            content.Add(ContentLine.AddText(Constant.Address));
            content.Add(ContentLine.AddText(Constant.PostalCode));
            content.Add(ContentLine.AddText(Constant.Phone));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.FontSize8));
            content.Add(ContentLine.AddText(Constant.PhoneWarning));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.FontSize10));
            content.Add(ContentLine.AddText(DoubleSeparator));

            var docType = "Fatura Simplificada";
            var docId = document.DocumentType + " " + document.Serie.ToString().PadLeft(3, '0') + "/" + document.Number;
            content.Add(ContentLine.AddText(docType + new string(' ', Chars - docType.Length - docId.Length) + docId));
            content.Add(ContentLine.AddText(DoubleSeparator));

            content.Add(ContentLine.AddText(document.Hash + Constant.SoftwareProcess));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.AlignCenter));
            content.Add(ContentLine.AddQrCode(document.QrCode, 3));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.FontSize8));
            content.Add(ContentLine.AddText("ATCUD: 0-" + document.Number));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.FontSize10));
            content.Add(ContentLine.AddText(DoubleSeparator));

            var org = document.Printed ? "Duplicado" : "Original";
            var time = document.Date.ToString("yyyy-MM-dd HH:mm");
            content.Add(ContentLine.AddText(org + new string(' ', Chars - org.Length - time.Length) + time));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.AlignLeft));
            content.Add(ContentLine.AddText("Atendido por: " + document.SellerName));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.AlignCenter));
            content.Add(ContentLine.AddText(SingleSeparator));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.AlignLeft));
            if (new FinalCustomer().Nif.Equals(document.ClientNif))
            {
                content.Add(ContentLine.AddText("NIF     : " + FinalCustomer.FillHiddenField));
                content.Add(ContentLine.AddText("CLIENTE : Consumidor Final"));
                content.Add(ContentLine.AddText("MORADA  : " + FinalCustomer.FillHiddenField));
            }
            else
            {
                content.Add(ContentLine.AddText("NIF     : " + document.ClientNif));
                content.Add(ContentLine.AddText("CLIENTE : " + document.ClientName));
                if (document.ClientAddress is null || string.IsNullOrWhiteSpace(document.ClientAddress.Address))
                    content.Add(ContentLine.AddText("MORADA  : " + FinalCustomer.FillHiddenField));
                else
                    content.Add(ContentLine.AddText("MORADA  : " + document.ClientAddress.Address + " " + document.ClientAddress.PostalCode + " " + document.ClientAddress.City));
            }
            
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.AlignCenter));
            content.Add(ContentLine.AddText(DoubleSeparator));

            var taxes = new Dictionary<decimal, decimal>();
            content.Add(ContentLine.AddText("Produto        IVA  Qtd   P.Unit      SubT"));
            content.Add(ContentLine.AddText(SingleSeparator));
            foreach (var line in document.Lines)
            {
                var name = (line.ItemShortName.Length > 12 ? line.ItemShortName.Substring(0, 12) : line.ItemShortName).PadRight(12, ' ');
                var iva = line.Tax.ToString("0%").PadLeft(6, ' ');
                var qnt = line.Quantity.ToString("0").PadLeft(4, ' ');
                var unit = line.ItemPrice.ToString("0.00", nfi).PadLeft(8, ' ');
                var subt = line.Total.ToString("0.00", nfi).PadLeft(8, ' ');
                content.Add(ContentLine.AddText(name + iva + qnt + " x" + unit + " =" + subt));

                if (taxes.ContainsKey(line.Tax))
                    taxes[line.Tax] += line.Total;
                else
                    taxes.Add(line.Tax, line.Total);
            }
            content.Add(ContentLine.AddText(DoubleSeparator));

            content.Add(ContentLine.SetInstruction(ContentLineInstruction.FontWeightBold));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.FontSize14));
            var tot = "Total";
            var din = "Dinheiro";
            var total = document.Total.ToString("0.00", nfi);
            content.Add(ContentLine.AddText(tot + new string(' ', 21 - tot.Length - total.Length) + total));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.FontSize10));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.FontWeightNormal));
            content.Add(ContentLine.AddText(din + new string(' ', Chars - din.Length - total.Length) + total));
            content.Add(ContentLine.AddText(DoubleSeparator));

            content.Add(ContentLine.AddText("  TAXA        BASE        IVA       TOTAL"));
            content.Add(ContentLine.AddText(SingleSeparator));
            foreach (var tax in taxes.OrderBy(x => x.Key))
            {
                var t = Math.Round(tax.Value / (tax.Key + 1), 2, MidpointRounding.AwayFromZero);
                var tx = tax.Key.ToString("0%").PadLeft(6, ' ');
                var ba = t.ToString("0.00", nfi).PadLeft(12, ' ');
                var iva = (tax.Value - t).ToString("0.00", nfi).PadLeft(11, ' ');
                var totalTx = tax.Value.ToString("0.00", nfi).PadLeft(12, ' ');
                content.Add(ContentLine.AddText(tx + ba + iva + totalTx));
            }
            content.Add(ContentLine.AddText(DoubleSeparator));

            content.Add(ContentLine.SetInstruction(ContentLineInstruction.ChangeLine));
            content.Add(ContentLine.AddText("IVA Incluído à taxa em vigor"));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.ChangeLine));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.FontSize12));
            content.Add(ContentLine.AddText("* OBRIGADO *"));
            content.Add(ContentLine.AddText("* VOLTE SEMPRE *"));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.ChangeLine));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.ChangeLine));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.ChangeLine));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.ChangeLine));
            content.Add(ContentLine.SetInstruction(ContentLineInstruction.PaperCut));
            return content;
        }

        public static IEnumerable<ContentLine> GetOpenDrawerLayout()
            => new List<ContentLine>() { ContentLine.SetInstruction(ContentLineInstruction.OpenDrawer) };
    }
}
