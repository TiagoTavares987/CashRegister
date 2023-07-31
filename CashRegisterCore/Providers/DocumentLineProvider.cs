using CashRegisterCore.Entities;
using System.Collections.Generic;

namespace CashRegisterCore.Providers
{
    internal class DocumentLineProvider
    {
        public IEnumerable<DocumentLine> GetAll() => AppCore.Db.GetAll<DocumentLine>();

        public IEnumerable<DocumentLine> GetDocumentLines(int docId) => AppCore.Db.GetAll<DocumentLine>(new Dictionary<string, object>() { { nameof(DocumentLine.DocumentId), docId } });

        public DocumentLine GetDocumentLine(int id) => AppCore.Db.GetEntity<DocumentLine>(id);

        public int InsertDocumentLine(DocumentLine documentLine) => AppCore.Db.Insert(documentLine);
    }
}
