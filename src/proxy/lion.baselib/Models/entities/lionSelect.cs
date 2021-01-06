using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LottoLion.BaseLib.Models.Entity
{
    public partial class LottoLionContext
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<T> MapToList<T>(IDataReader reader)
        {
            var results = new List<T>();

            var columnCount = reader.FieldCount;
            while (reader.Read())
            {
                var item = Activator.CreateInstance<T>();

                //reader properties
                var rdrProperties = Enumerable.Range(0, columnCount).Select(i => reader.GetName(i)).ToArray();
                foreach (var property in typeof(T).GetProperties())
                {
                    //check if property is matched with T Element
                    if ((typeof(T).GetProperty(property.Name).GetGetMethod().IsVirtual) || (!rdrProperties.Contains(property.Name)))
                    {
                        continue; //if there is properties with virtual or mismatch with reader properties and T Elements
                    }
                    else
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                        {
                            var convertTo = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                            property.SetValue(item, Convert.ChangeType(reader[property.Name], convertTo), null);
                        }
                    }
                }
                results.Add(item);
            }

            return results;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command_text"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IEnumerable<T> SelectFunction<T>(string command_text, params NpgsqlParameter[] parameters)
        {
            var _results = Enumerable.Empty<T>();

            using (var conn = new NpgsqlConnection(connectionString: __connection_string))
            {
                conn.Open();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = command_text;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddRange(parameters);

                    using (var dataReader = command.ExecuteReader())
                    {
                        _results = MapToList<T>(dataReader);
                    }
                }
            }

            return _results;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command_text"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IEnumerable<T> SelectQuery<T>(string command_text, params NpgsqlParameter[] parameters)
        {
            var _results = Enumerable.Empty<T>();

            using (var conn = new NpgsqlConnection(connectionString: __connection_string))
            {
                conn.Open();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = command_text;
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddRange(parameters);

                    using (var dataReader = command.ExecuteReader())
                    {
                        _results = MapToList<T>(dataReader);
                    }
                }
            }

            return _results;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command_text"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IEnumerable<T> ExecuteFunction<T>(string command_text, params NpgsqlParameter[] parameters)
        {
            var _results = Enumerable.Empty<T>();

            using (var conn = new NpgsqlConnection(connectionString: __connection_string))
            {
                conn.Open();

                using (var tran = conn.BeginTransaction())
                {
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = command_text;
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddRange(parameters);

                        using (var dataReader = command.ExecuteReader())
                        {
                            _results = MapToList<T>(dataReader);
                        }
                    }

                    tran.Commit();
                }
            }

            return _results;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command_text"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteFunction(string command_text, params NpgsqlParameter[] parameters)
        {
            var _result = 0;

            using (var conn = new NpgsqlConnection(connectionString: __connection_string))
            {
                conn.Open();

                using (var tran = conn.BeginTransaction())
                {
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = command_text;
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddRange(parameters);

                        _result = command.ExecuteNonQuery();
                    }

                    tran.Commit();
                }
            }

            return _result;
        }

        public int ExecuteCommand(string command_text, params NpgsqlParameter[] parameters)
        {
            var _result = 0;

            using (var conn = new NpgsqlConnection(connectionString: __connection_string))
            {
                conn.Open();

                using (var tran = conn.BeginTransaction())
                {
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = command_text;
                        command.CommandType = CommandType.Text;

                        command.Parameters.AddRange(parameters);

                        _result = command.ExecuteNonQuery();
                    }

                    tran.Commit();
                }
            }

            return _result;
        }
    }
}