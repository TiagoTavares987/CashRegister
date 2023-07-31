using CashRegisterCore.Entities;
using CashRegisterCore.Providers;
using CashRegisterCore.Utils;
using CashRegisterCore.Enumerators;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;

namespace CashRegisterCore.Managers
{
    public class DocumentManager
    {
        private DocumentProvider _provider;
        private DocumentLineProvider _providerLine;

        public DocumentManager()
        {
            _provider = new DocumentProvider();
            _providerLine = new DocumentLineProvider();
        }

        public int GetLastDocumentId() => _provider.GetLastInsertedId();

        public Document GetLastInsertedDoc() => GetDocument(_provider.GetLastInsertedId());

        public IEnumerable<Document> GetDocuments() => _provider.GetAll();

        public Document GetDocument(int id)
        {
            var doc = _provider.GetDocument(id);
            if (doc is null)
                return null;

            doc.Lines.Clear();
            var lines = _providerLine.GetDocumentLines(id);
            foreach (var line in lines)
                doc.Lines.Add(line);

            DocumentBuilder.SetQrCode(doc);
            DocumentBuilder.SetHash(doc);

            return doc;
        }

        public Document GetDocument(int serie, int num)
        {
            var doc = _provider.GetDocument(serie, num);
            if (doc is null)
                return null;

            doc.Lines.Clear();
            var lines = _providerLine.GetDocumentLines(doc.Id);
            foreach (var line in lines)
                doc.Lines.Add(line);

            DocumentBuilder.SetQrCode(doc);
            DocumentBuilder.SetHash(doc);

            return doc;
        }

        public Document GetDocumentHeader(int id) => _provider.GetDocument(id);

        public void UpdateDocumentPrinted(int Id) => _provider.UpdateDocumentPrinted(Id);

        public void SaveDocument(Document document)
        {
            ValidateDocument(document);

            document.Number = _provider.GetLastInsertedNumber() + 1;

            var id = _provider.InsertDocument(document);
            if (id > 0)
            {
                document.Id = id;
                document.Lines.ForEach(line =>
                {
                    line.DocumentId = id;
                    var lineProvider = new DocumentLineProvider();
                    line.Id = lineProvider.InsertDocumentLine(line);

                    if (line.Id <= 0)
                        return;
                });
            }
            else
                return;
        }

        private void ValidateDocument(Document document)
        {
            if (AppCore.TerminalId > 0)
            {
                if (!Validate.Nif(document.ClientAddress.CountryShort, document.ClientNif))
                    throw new ArgumentException("Invalid nif.");

                if (Country.PT.Equals(document.ClientAddress.CountryShort) && !string.IsNullOrEmpty(document.ClientAddress.PostalCode) && !Regex.IsMatch(document.ClientAddress.PostalCode, @"^\d{4}-\d{3}$"))
                    throw new ArgumentException("Invalid postal code.");

                document.Lines.ForEach(line =>
                {
                    if (document.Total <= 0)
                        throw new ArgumentException("Invalid total.");
                });
            }
        }
    }
}
