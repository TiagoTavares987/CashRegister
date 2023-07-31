using System;

namespace Database
{
    public class IsDbTable : Attribute
    {
        public IsDbTable(string tableName) => TableName = tableName;

        public string TableName { get; }
    }
}
