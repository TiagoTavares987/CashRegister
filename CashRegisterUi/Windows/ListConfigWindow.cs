using System;
using CashRegisterUi.Controls;
using CashRegisterUi.ViewModel;
using Gtk;

namespace CashRegisterUi.Windows
{
    internal class ListConfigWindow : Window //Dialog
    {
        private EntityViewModel _viewModel;

        public ListConfigWindow(Window parent, EntityViewModel viewModel)
            : base(viewModel.Title)//: base()
        {
            _viewModel = viewModel;

            this.Title = _viewModel.Title;
            this.Modal = true;
            this.Parent = parent;
            this.DefaultWidth = 800;
            this.DefaultHeight = 600;
            this.TransientFor = parent;
            this.TypeHint = Gdk.WindowTypeHint.Dialog;
            this.WindowPosition = WindowPosition.CenterOnParent;

            var cellRender = new CellRendererText();
            var column = new TreeViewColumn();
            column.PackStart(cellRender, true);
            column.AddAttribute(cellRender, "text", 1);

            var listView = new TreeView();
            listView.AppendColumn(column);
            listView.Model = _viewModel.Store;
            listView.RowActivated += delegate { _viewModel.EditEntity(this, GetSelectedId(listView)); };

            var newButton = new Button(new Label("Novo"));
            newButton.Clicked += delegate { _viewModel.NewEntity(this); };
            var editButton = new Button(new Label("Editar"));
            editButton.Clicked += delegate { _viewModel.EditEntity(this, GetSelectedId(listView)); };
            var deleteButton = new Button(new Label("Apagar"));
            deleteButton.Clicked += delegate { _viewModel.DeleteEntity(this, GetSelectedId(listView)); };
            var exitButton = new Button(new Label("Sair"));
            exitButton.Clicked += delegate { this.Destroy(); };

            var buttons = new HBox(false, 2);
            buttons.PackStart(new Dummy(), true, true, 0);
            buttons.PackStart(newButton, false, false, 0);
            buttons.PackStart(editButton, false, false, 0);
            buttons.PackStart(deleteButton, false, false, 0);
            buttons.PackStart(exitButton, false, false, 0);

            var box = new VBox(false, 2);
            box.PackStart(listView, true, true, 0);
            box.PackStart(buttons, false, true, 0);

            Add(box);

            ShowAll();
        }

        private static int GetSelectedId(TreeView view)
        {
            view.Selection.GetSelected(out TreeIter iter1);
            return Convert.ToInt32(view.Model.GetValue(iter1, 0));
        }
    }
}
