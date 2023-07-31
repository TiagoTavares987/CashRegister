using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Database
{
    internal class SqlComposer<T>
    {
        private string _table;
        private List<PropertyInfo> _tableKeyFields = new List<PropertyInfo>();
        private List<PropertyInfo> _fields = new List<PropertyInfo>();
        private List<PropertyInfo> _fieldsJson = new List<PropertyInfo>();
        private Dictionary<string, object> _parameters = new Dictionary<string, object>();

        public SqlComposer()
        {
            var table = (IsDbTable)typeof(T).GetCustomAttributes(false).FirstOrDefault(x => x is IsDbTable);
            if (table != null)
                _table = table.TableName;

            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(false);
                foreach (var attribute in attributes)
                {
                    if (attribute is IsDbField field)
                    {
                        if (field.IsTableKey)
                            _tableKeyFields.Add(property);
                        else if (field.IsJsonObject)
                            _fieldsJson.Add(property);
                        else
                            _fields.Add(property);
                    }
                }
            }
        }

        public IReadOnlyList<PropertyInfo> TableKeyFields => _tableKeyFields;
        public IReadOnlyList<PropertyInfo> Fields => _fields;
        public IReadOnlyList<PropertyInfo> FieldsJson => _fieldsJson;
        public IReadOnlyDictionary<string, object> Parameters => _parameters.ToDictionary(x => "@p_" + x.Key, y => y.Value);

        public string GetTableQuery(IReadOnlyDictionary<string, object> find)
        {
            if (find == null)
            {
                if (string.IsNullOrEmpty(_table))
                    return null;

                return "select * from " + _table + ";";
            }

            find.ToList().ForEach((property) =>
            {
                var field = _tableKeyFields.SingleOrDefault(x => x.Name.Equals(property.Key, StringComparison.InvariantCultureIgnoreCase));
                if (field != null)
                    _parameters.Add(field.Name, property.Value);
                else
                {
                    field = _fields.SingleOrDefault(x => x.Name.Equals(property.Key, StringComparison.InvariantCultureIgnoreCase));
                    if (field != null)
                        _parameters.Add(field.Name, property.Value);
                }
            });

            var where = _parameters.Count == 0 ? string.Empty : " where " + string.Join(" and ", _parameters.Keys.Select(y => y + " = @p_" + y));

            return $"select * from {_table}{where};";
        }

        public string GetTableQuery(IReadOnlyDictionary<string, IList<object>> findIn)
        {
            // TODO
            return null;
        }

        public string GetRowQuery(params object[] tableKeysValues)
        {
            if (string.IsNullOrEmpty(_table))
                return null;

            var i = 0;
            var tableKeyFields = _tableKeyFields?.Select(x => x.Name);
            foreach (string tableKey in tableKeyFields)
            {
                if (tableKeysValues.Length <= i)
                    break;

                _parameters.Add(tableKey, tableKeysValues[i]);
            }

            var where = _parameters.Count == 0 ? string.Empty : " where " + string.Join(" and ", _parameters.Keys.Select(y => y + " = @p_" + y));

            return $"select * from {_table}{where};";
        }

        public string GetInsertQuery(T entity)
        {
            if (string.IsNullOrEmpty(_table))
                return null;

            var columns = new List<string>();
            _tableKeyFields.ForEach(field =>
            {
                columns.Add(field.Name);
                _parameters.Add(field.Name, field.GetValue(entity));
            });
            _fieldsJson.ForEach(field =>
            {
                columns.Add(field.Name);
                _parameters.Add(field.Name, JsonConvert.SerializeObject(field.GetValue(entity)));
            });
            _fields.ForEach(field =>
            {
                columns.Add(field.Name);
                _parameters.Add(field.Name, field.GetValue(entity));
            });

            return $"insert into {_table} ({string.Join(", ", columns)}) values ({string.Join(", ", _parameters.Keys.Select(x => "@p_" + x))});";
        }

        public string GetUpdateQuery(T entity)
        {
            if (string.IsNullOrEmpty(_table))
                return null;

            var tableKeys = new List<string>();
            var columns = new List<string>();
            _tableKeyFields.ForEach(field =>
            {
                tableKeys.Add(field.Name);
                _parameters.Add(field.Name, field.GetValue(entity));
            });
            _fieldsJson.ForEach(field =>
            {
                columns.Add(field.Name);
                _parameters.Add(field.Name, JsonConvert.SerializeObject(field.GetValue(entity)));
            });
            _fields.ForEach(field =>
            {
                columns.Add(field.Name);
                _parameters.Add(field.Name, field.GetValue(entity));
            });

            var where = tableKeys.Count == 0 ? string.Empty : " where " + string.Join(" and ", tableKeys.Select(y => y + " = @p_" + y));

            return $"update {_table} set {string.Join(", ", columns.Select(y => y + " = @p_" + y))}{where};";
        }

        public string GetDeleteQuery(T entity)
        {
            if (string.IsNullOrEmpty(_table))
                return null;

            var tableKeys = new List<string>();
            _tableKeyFields.ForEach(field =>
            {
                tableKeys.Add(field.Name);
                _parameters.Add(field.Name, field.GetValue(entity));
            });

            var where = tableKeys.Count == 0 ? string.Empty : " where " + string.Join(" and ", tableKeys.Select(y => y + " = @p_" + y));

            return $"delete from {_table}{where};";
        }

        public string GetMaxIdQuery()
        {
            var idField = _tableKeyFields.FirstOrDefault();
            return idField is null ? null : $"select max({idField.Name}) as MaxVal from {_table};";
        }
    }
}
