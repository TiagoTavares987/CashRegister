using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Database
{
    public class MySql
    {
        private const string _passwordIv = "password";

        private static IMySqlConfig _config;

        private static string _connectionString;

        public MySql(IMySqlConfig config)
        {
            if (!string.IsNullOrWhiteSpace(_connectionString))
                return;

            _config = config;

            var query = "SET GLOBAL  max_allowed_packet=1024*1024*1024;";
            try
            {
                SetConnectionString();
                using (MySqlConnection con = new MySqlConnection(_connectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand() { CommandText = query, CommandType = CommandType.Text, Connection = con })
                        try { cmd.ExecuteNonQuery(); } catch { }
                    con.Close();
                }
            }
            catch
            {
                try
                {
                    SetConnectionString(false);
                    using (MySqlConnection con = new MySqlConnection(_connectionString))
                    {
                        con.Open();
                        using (MySqlCommand cmd = new MySqlCommand() { CommandText = query, CommandType = CommandType.Text, Connection = con })
                            try { cmd.ExecuteNonQuery(); } catch { }
                        con.Close();
                    }
                    _config.Password = EncodePassword(_config.Password);
                    _config.Save();
                }
                catch
                {
                    _connectionString = null;
                    ////throw new InvalidOperationException("Connetion to database failed"); 
                }
            }
        }

        public IList<T> GetAll<T>(IReadOnlyDictionary<string, object> find = null) where T : class, new()
        {
            var composer = new SqlComposer<T>();
            var query = composer.GetTableQuery(find);

            return GetList(query, composer, composer.Parameters);
        }
        public IList<T> GetList<T>(string query, IReadOnlyDictionary<string, object> parameters) where T : class, new()
            => GetList(query, new SqlComposer<T>(), parameters);
        public IList<T> GetList<T>(IReadOnlyDictionary<string, IList<object>> findIn) where T : class, new()
        {
            var composer = new SqlComposer<T>();
            var query = composer.GetTableQuery(findIn);

            return GetList(query, composer, composer.Parameters);
        }
        public T GetEntity<T>(params object[] tableKeysValues) where T : class, new()
        {
            var composer = new SqlComposer<T>();
            var query = composer.GetRowQuery(tableKeysValues);

            if (string.IsNullOrEmpty(query))
                return null;

            using (MySqlConnection con = new MySqlConnection(_connectionString))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand() { CommandText = query, CommandType = CommandType.Text, Connection = con })
                {
                    var parameters = composer.Parameters;
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (KeyValuePair<string, object> parameter in parameters)
                            cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }

                    DataSet ds = new DataSet();
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    da.Fill(ds);

                    if (ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                        return GetEntity(composer, ds.Tables[0].Rows[0]);
                }
                con.Close();
            }

            return null;
        }
        public int Insert<T>(T entity) where T : class
        {
            var sqlComposer = new SqlComposer<T>();
            return InsertRow(sqlComposer.GetInsertQuery(entity), sqlComposer.Parameters);
        }
        public int Update<T>(T entity) where T : class
        {
            var sqlComposer = new SqlComposer<T>();
            return Execute(sqlComposer.GetUpdateQuery(entity), sqlComposer.Parameters);
        }
        public int Delete<T>(T entity) where T : class
        {
            var sqlComposer = new SqlComposer<T>();
            return Execute(sqlComposer.GetDeleteQuery(entity), sqlComposer.Parameters);
        }
        public int SelectLastInsertedId<T>()
        {
            var composer = new SqlComposer<T>();
            var query = composer.GetMaxIdQuery();

            if (string.IsNullOrEmpty(query))
                return -1;

            int result = -1;
            DataRow dr = null;
            using (MySqlConnection con = new MySqlConnection(_connectionString))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand() { CommandText = query, CommandType = CommandType.Text, Connection = con })
                {
                    try
                    {
                        DataSet ds = new DataSet();
                        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                        da.Fill(ds);

                        if (ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                        {
                            dr = ds.Tables[0].Rows[0];
                            result = dr["MaxVal"] is DBNull ? 0 : Convert.ToInt32(dr["MaxVal"]);
                        }
                    }
                    catch (Exception ex) { throw ex; }
                }
                con.Close();
            }

            return result;
        }

        public int InsertRow(string query, IReadOnlyDictionary<string, object> parameters = null)
        {
            if (string.IsNullOrEmpty(query))
                return -1;

            int result = -1;
            query += "SELECT LAST_INSERT_ID();";
            DataRow dr = null;
            using (MySqlConnection con = new MySqlConnection(_connectionString))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand() { CommandText = query, CommandType = CommandType.Text, Connection = con })
                {
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (KeyValuePair<string, object> parameter in parameters)
                            cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }
                    try
                    {
                        DataSet ds = new DataSet();
                        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                        da.Fill(ds);

                        if (ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                        {
                            dr = ds.Tables[0].Rows[0];
                            if (dr["LAST_INSERT_ID()"] != null)
                                result = Convert.ToInt32(dr["LAST_INSERT_ID()"]);
                        }
                    }
                    catch (Exception ex) { throw ex; }
                }
                con.Close();
            }

            return result;
        }
        public int Execute(string query, IReadOnlyDictionary<string, object> parameters = null)
        {
            if (string.IsNullOrEmpty(query))
                return -1;

            int result = -1;
            using (MySqlConnection con = new MySqlConnection(_connectionString))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand() { CommandText = query, CommandType = CommandType.Text, Connection = con })
                {
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (KeyValuePair<string, object> parameter in parameters)
                            cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }
                    try { result = cmd.ExecuteNonQuery(); }
                    catch (Exception ex) { throw ex; }
                }
                con.Close();
            }
            return result;
        }

        private IList<T> GetList<T>(string query, SqlComposer<T> composer, IReadOnlyDictionary<string, object> parameters) where T : class, new()
        {
            if (string.IsNullOrEmpty(query))
                return null;

            using (MySqlConnection con = new MySqlConnection(_connectionString))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand() { CommandText = query, CommandType = CommandType.Text, Connection = con })
                {
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (KeyValuePair<string, object> parameter in parameters)
                            cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }

                    DataSet ds = new DataSet();
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    da.Fill(ds);

                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        var list = new List<T>();
                        foreach (var dr in ds.Tables[0].Rows)
                        {
                            var entity = GetEntity(composer, (DataRow)dr);
                            if (entity != null)
                                list.Add(entity);
                        }
                        return list;
                    }
                }
                con.Close();
            }
            return null;
        }
        private T GetEntity<T>(SqlComposer<T> composer, DataRow dr) where T : class, new()
        {
            if (dr == null)
                return null;

            if (composer.TableKeyFields.Count == 0 && composer.Fields.Count == 0)
                return null;

            T entity = new T();
            foreach (var property in composer.TableKeyFields)
            {
                property.SetValue(entity, dr[property.Name]);
            }
            foreach (var property in composer.Fields)
            {
                if (!(dr[property.Name] is DBNull))
                {
                    if (property.PropertyType.IsEnum)
                    {
                        try { property.SetValue(entity, Enum.Parse(property.PropertyType, dr[property.Name].ToString())); }
                        catch { }
                    }
                    else
                        property.SetValue(entity, dr[property.Name]);
                }
            }
            foreach (var property in composer.FieldsJson)
            {
                if (!(dr[property.Name] is DBNull))
                    property.SetValue(entity, JsonConvert.DeserializeObject(dr[property.Name]?.ToString(), property.PropertyType));
            }

            return entity;
        }

        private static void SetConnectionString(bool encodedPassword = true)
        {
            var result = true;
            var connection = new MySqlConnectionStringBuilder();

            try
            {
                connection.Server = _config.Address;
            }
            catch
            {
                _config.Address = string.Empty;
                result = false;
            }

            try
            {
                connection.Port = Convert.ToUInt32(_config.Port);
            }
            catch
            {
                _config.Port = string.Empty;
                result = false;
            }

            try
            {
                connection.Database = _config.Database;
            }
            catch
            {
                _config.Database = string.Empty;
                result = false;
            }

            try
            {
                connection.UserID = _config.Username;
            }
            catch
            {
                _config.Username = string.Empty;
                result = false;
            }

            try
            {
                var password = _config.Password;
                connection.Password = encodedPassword ? DecodePassword(password) : password;
            }
            catch
            {
                if (!encodedPassword)
                    _config.Password = string.Empty;
                result = false;
            }

            if (result)
                _connectionString = connection.ConnectionString;
            else
                throw new ArgumentException("Invalid server settings in config");
        }
        private static string EncodePassword(string password) => Base64.Encode(_passwordIv + Base64.Encode(password));
        private static string DecodePassword(string password) => Base64.Decode(Base64.Decode(password).Replace(_passwordIv, string.Empty));

    }
}
