using CashRegisterCore;
using CashRegisterCore.Entities;
using Database;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CashRegisterDbCreateSeed
{
    internal class DbCreate
    {
        internal static void CreateTables()
        {
            foreach (var table in GetDbTableTypes())
            {
                var keys = new List<PropertyInfo>();
                var properties = table.Item1.GetProperties();

                var props = new List<string>();
                foreach (var property in properties)
                {
                    var attributes = property.GetCustomAttributes(typeof(IsDbField), false);
                    if (attributes.Length > 0)
                    {
                        var attribute = (IsDbField)attributes[0];
                        if (attribute.IsTableKey)
                            keys.Add(property);

                        var prop = property.Name + " " + GetPropertyType(property.PropertyType, attribute.IsJsonObject) + " " + GetDefault(property.PropertyType, attribute.IsTableKey && keys.Count == 1);
                        props.Add(prop);
                    }
                }

                if (keys.Count > 0)
                {
                    props.Add("PRIMARY KEY (" + keys[0].Name + ")");
                    keys.RemoveAt(0);
                }

                foreach (var key in keys)
                {
                    props.Add("UNIQUE KEY (" + key + ")");
                }

                var query = "DROP TABLE IF EXISTS " + table.Item2 + "; CREATE TABLE " + table.Item2 + " ( " + string.Join(", ", props) + ");";
                if (AppCore.Db.Execute(query) != 0)
                    throw new Exception("Failed to create table " + table.Item2);
            }

            var adminImage = new ImageResource() { FilePath = @"Images\User\admin.png" };
            AppCore.ImageResourceManager.SaveImageResource(adminImage);
            AppCore.Db.Insert(new User() { IsAdmin = true, Username = "Admin", Password = "81dc9bdb52d04dc20036dbd8313ed055", ImageId = adminImage.Id });
        }

        private static IEnumerable<Tuple<Type, string>> GetDbTableTypes()
        {
            foreach (Type type in Assembly.GetAssembly(typeof(AppCore)).GetTypes())
            {
                var attributes = type.GetCustomAttributes(typeof(IsDbTable), false);
                if (attributes.Length > 0)
                    yield return new Tuple<Type, string>(type, ((IsDbTable)attributes[0]).TableName);
            }
        }

        private static string GetPropertyType(Type propertyType, bool isJsonObject)
        {
            if (propertyType.Equals(typeof(bool)))
                return "TINYINT(1)";
            else if (propertyType.Equals(typeof(int)))
                return "INT(11)";
            else if (propertyType.Equals(typeof(decimal)))
                return "DECIMAL(18,6)";
            else if (propertyType.Equals(typeof(string)))
                return "VARCHAR(250)";
            else if (propertyType.Equals(typeof(DateTime)))
                return "DATETIME";
            else if (propertyType.Equals(typeof(byte[])))
                return "BLOB";
            else if (propertyType.BaseType != null && propertyType.BaseType.Equals(typeof(Enum)))
                return "VARCHAR(250)";
            else if (isJsonObject)
                return "LONGTEXT";

            throw new ArgumentOutOfRangeException(propertyType.Name);
        }

        private static string GetDefault(Type propertyType, bool autoIncrement)
        {
            if (propertyType.Equals(typeof(int)))
            {
                if (autoIncrement)
                    return "NOT NULL AUTO_INCREMENT";
                else
                    return "NOT NULL DEFAULT '0'";
            }
            else if (propertyType.Equals(typeof(bool))
                || propertyType.Equals(typeof(decimal)))
                return "NOT NULL DEFAULT '0'";

            return "NULL DEFAULT NULL";
        }
    }
}
