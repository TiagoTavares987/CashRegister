using System;
using System.Collections.Generic;
using CashRegisterCore;
using CashRegisterUi.Controls;
using CashRegisterUi.Utils;
using Gtk;

namespace CashRegisterUi.Windows
{
    internal class ListDocumentWindow : Window //Dialog
    {
        public ListDocumentWindow(Window parent)
            : base(WindowType.Toplevel)
        {
            this.Title = "Lista de documentos";
            this.Modal = true;
            this.Parent = parent;
            this.DefaultWidth = 500;
            this.DefaultHeight = 500;
            this.TransientFor = parent;
            this.TypeHint = Gdk.WindowTypeHint.Dialog;
            this.WindowPosition = WindowPosition.CenterOnParent;

            var columns = new List<(Type, string, XAlign)>
            {
                (typeof(int), "Serie", XAlign.Right),
                (typeof(int), "Numero", XAlign.Right),
                (typeof(DateTime), "Data", XAlign.Left),
                (typeof(string), "Vendedor", XAlign.Left),
                (typeof(string), "Cliente", XAlign.Left),
                (typeof(string), "Nif", XAlign.Left),
                (typeof(decimal), "Taxas", XAlign.Right),
                (typeof(decimal), "Total", XAlign.Right),
            };

            var listView = new Grid(columns);
            var documents = AppCore.DocumentManager.GetDocuments();
            foreach (var document in documents)
                listView.AddRow(document.Serie, document.Number, document.Date, document.SellerName, document.ClientName, document.ClientNif, document.TotalTaxes, document.Total);

            var printButton = new Button(new Label("Segunda via"));
            printButton.Clicked += delegate 
            {
                if(listView.GetSelectedValue(0) is int serie && listView.GetSelectedValue(1) is int num)
                {
                    var document = AppCore.DocumentManager.GetDocument(serie, num);
                    if(document != null)
                        new PrintOrEmailWindow(this, document);
                }
                
            };
            var exitButton = new Button(new Label("Sair"));
            exitButton.Clicked += delegate { this.Destroy(); };

            var buttons = new HBox(false, 2);
            buttons.PackStart(new Dummy(), true, true, 0);
            buttons.PackStart(printButton, false, false, 0);
            buttons.PackStart(exitButton, false, false, 0);

            var box = new VBox(false, 2);
            box.PackStart(listView, true, true, 0);
            box.PackStart(buttons, false, true, 0);

            Add(box);

            ShowAll();
        }
    }
}
