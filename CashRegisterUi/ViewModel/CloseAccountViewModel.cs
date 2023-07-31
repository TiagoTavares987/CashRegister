using CashRegisterCore;
using CashRegisterCore.Managers;
using CashRegisterUi.Windows;
using Gtk;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CashRegisterUi.ViewModel
{
    internal class CloseAccountViewModel
    {
        private readonly Window _parent;
        private readonly DocumentBuilder _documentBuilder;
        private readonly System.Action _cleanDocumentView;

        public CloseAccountViewModel(Window parent, DocumentBuilder documentBuilder, System.Action cleanDocumentView)
        {
            if (!documentBuilder.HasLines)
                return;

            _parent = parent;
            _documentBuilder = documentBuilder;
            _cleanDocumentView = cleanDocumentView;

            if (!_documentBuilder.HasClient)
            {
                if (MessagePopup.ShowQuestion(_parent, "Deseja nif?"))
                {
                    var w = new DocumentClientWindow(_parent, (client) => { _documentBuilder.SetClient(client); SaveAndPrint(); });
                    return;
                }
                else
                    _documentBuilder.SetFinalCustomer();
            }
            SaveAndPrint();
        }

        private void SaveAndPrint()
        {
            if (!_documentBuilder.HasClient)
                return;

            var document = _documentBuilder.GetDocument();

            try
            {
                AppCore.DocumentManager.SaveDocument(document);
            }
            catch (Exception ex) { MessagePopup.ShowError(_parent, "Documento não gravado.\n" + ex.Message); return; }

            Task.Run(() => AppCore.PritingManager.OpenDrawer());
            Task.Run(() => AppCore.PritingManager.PrintKitchen(document));

            if (MessagePopup.ShowQuestion(_parent, "Deseja imprimir?"))
                Task.Run(() => AppCore.PritingManager.Print(document));

            if(_documentBuilder.HasClient && !string.IsNullOrEmpty(_documentBuilder.GetClient().Email) && MessagePopup.ShowQuestion(_parent, "Deseja enviar por mail?"))
                AppCore.PritingManager.Email(document, _documentBuilder.GetClient().Email);

            _cleanDocumentView();
        }
    }
}
