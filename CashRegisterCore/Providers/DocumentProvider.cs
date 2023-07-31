using CashRegisterCore.Entities;
using System.Collections.Generic;
using System.Linq;

namespace CashRegisterCore.Providers
{
    internal class DocumentProvider
    {
        public IEnumerable<Document> GetAll() => AppCore.Db.GetAll<Document>();

        public Document GetDocument(int id) => AppCore.Db.GetEntity<Document>(id);

        public Document GetDocument(int serie, int num) => AppCore.Db.GetAll<Document>(new Dictionary<string, object>() { { nameof(Document.Serie), serie }, { nameof(Document.Number), num } }).FirstOrDefault();

        public int InsertDocument(Document document) => AppCore.Db.Insert(document);

        public int GetLastInsertedId() => AppCore.Db.SelectLastInsertedId<Document>();

        public Document GetLastInsertedDoc()
            => AppCore.Db.GetAll<Document>(new Dictionary<string, object>() { { nameof(Document.Serie), AppCore.TerminalId } }).LastOrDefault();

        public int GetLastInsertedNumber()
        {
            var doc = GetLastInsertedDoc();
            if (doc is null)
                return 0;
            else
                return doc.Number;
        }

        public int UpdateDocumentPrinted(int id)
        {
            var doc = GetDocument(id);
            if (doc is null)
                return -1;

            doc.Printed = true;
            return AppCore.Db.Update<Document>(doc);
        }
    }
}
