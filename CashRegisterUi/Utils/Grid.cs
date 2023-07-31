using Gtk;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CashRegisterUi.Utils
{
    internal class Grid : TreeView
    {
        private ListStore _store;
        private Gdk.Color _highlightCellColor = new Gdk.Color(0, 150, 255);

        public Grid(IList<(Type, string, XAlign)> columns)
        {
            _store = new ListStore(columns.Select(x => x.Item1).ToArray());

            for (int i = 0; i < columns.Count; i++)
                AppendColumn(new GridColumn(i, columns[i].Item2, columns[i].Item1, columns[i].Item3));

            Model = _store;
            Selection.Mode = SelectionMode.Single;
            ModifyBase(StateType.Active, _highlightCellColor);
            ModifyBase(StateType.Selected, _highlightCellColor);
            ModifyText(StateType.Selected, new Gdk.Color(0, 0, 0));
        }

        public List<object[]> GetAll()
        {
            var list = new List<object[]>();
            if(_store.GetIterFirst(out var iter))
            {
                do
                {
                    var line = new object[_store.NColumns];
                    for(var column = 0; column < _store.NColumns; column++)
                        line[column] = _store.GetValue(iter, column);

                    list.Add(line);
                }
                while (_store.IterNext(ref iter));
            }
            return list;
        }

        public object GetSelectedValue(int column)
        {
            Selection.GetSelected(out TreeIter iter1);
            return _store.GetValue(iter1, column);
        }

        public void SetSelectedValue(int column, object value)
        {
            Selection.GetSelected(out TreeIter iter1);
            _store.SetValue(iter1, column, value);
        }

        public void AddRow(params object[] values)
        {
            _store.AppendValues(values);
            _store.GetIterFirst(out var iter);
            var last = iter;
            while (_store.IterNext(ref iter))
                last = iter;
            Selection.SelectIter(last);
        }

        public void RemoveSelectedRow()
        {
            Selection.GetSelected(out TreeIter iter1);
            _store.Remove(ref iter1);
        }

        public void Clean() => _store.Clear(); 
    }

    internal class GridColumn : TreeViewColumn
    {
        private Gdk.Color _backgroundColor = new Gdk.Color(255, 255, 255);
        private Gdk.Color _oddRowBackgroundColor = new Gdk.Color(238, 238, 238);

        public GridColumn(int columnId, string title, Type dataType, XAlign xAlign) : base(title, new CellRendererText())
        {
            ColumnId = columnId;
            DataType = dataType;
            XAlign = xAlign;
            SetCellDataFunc(Cells[0], new TreeCellDataFunc(RenderColumnCell));
        }

        public int ColumnId { get; }
        public Type DataType { get; }
        public XAlign XAlign { get; }

        private void RenderColumnCell(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
        {
            var gridColumn = (GridColumn)column;
            var cellRender = (CellRendererText)cell;
            var value = model.GetValue(iter, gridColumn.ColumnId);

            if (value != null)
            {
                if (gridColumn.DataType.Equals(typeof(decimal)))
                {
                    cellRender.Xalign = 1;
                    value = string.Format("{0:#0.00}", value);
                }

                cellRender.Xalign = gridColumn.XAlign == XAlign.Center ? .5f : (float)gridColumn.XAlign;
            }

            cellRender.Text = value?.ToString();

            var path = model.GetPath(iter);

            if (path.Indices[path.Indices.Length - 1] % 2 != 0 && !cell.CellBackgroundGdk.Equals(_oddRowBackgroundColor))
                cell.CellBackgroundGdk = _oddRowBackgroundColor;
            else
                cell.CellBackgroundGdk = _backgroundColor;
        }
    }
}
