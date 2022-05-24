using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

namespace Lion.Share.Data
{
    public partial class AppDbContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        /// <example>
        /// public class TopUser
        /// {
        ///     public string Name { get; set; }
        ///     public int Count { get; set; }
        /// }
        /// 
        /// var result = Helper.RawSqlQuery(
        /// "SELECT TOP 10 Name, COUNT(*) FROM Users U"
        /// + " INNER JOIN Signups S ON U.UserId = S.UserId"
        /// + " GROUP BY U.Name ORDER BY COUNT(*) DESC",
        /// x => new TopUser { Name = (string)x[0], Count = (int)x[1] });
        /// 
        /// result.ForEach(x => Console.WriteLine($"{x.Name,-25}{x.Count}"));
        /// </example>
        public List<T> RawSqlQuery<T>(string query, Func<DbDataReader, T> map)
        {
            var _results = new List<T>();

            var _connection = this.Database.GetDbConnection();
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;

                    if (_connection.State != ConnectionState.Open) _connection.Open();

                    using (var result = command.ExecuteReader())
                    {
                        while (result.Read())
                            _results.Add(map(result));
                    }
                }
            }

            return _results;
        }

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
        public IEnumerable<T> SelectFunction<T>(string command_text, params SqlParameter[] parameters)
        {
            var _results = Enumerable.Empty<T>();

            var _connection = this.Database.GetDbConnection();
            {
                if (_connection.State != ConnectionState.Open) _connection.Open();

                using (var command = _connection.CreateCommand())
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
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IEnumerable<T> SelectQuery<T>(string query, params SqlParameter[] parameters)
        {
            var _results = Enumerable.Empty<T>();

            var _connection = this.Database.GetDbConnection();
            {
                if (_connection.State != ConnectionState.Open) _connection.Open();

                using (var command = _connection.CreateCommand())
                {
                    command.CommandText = query;
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
        public IEnumerable<T> ExecuteFunction<T>(string command_text, params SqlParameter[] parameters)
        {
            var _results = Enumerable.Empty<T>();

            var _connection = this.Database.GetDbConnection();
            {
                if (_connection.State != ConnectionState.Open) _connection.Open();

                using (var tran = _connection.BeginTransaction())
                {
                    using (var command = _connection.CreateCommand())
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
        public int ExecuteFunction(string command_text, params SqlParameter[] parameters)
        {
            var _result = 0;

            var _connection = this.Database.GetDbConnection();
            {
                if (_connection.State != ConnectionState.Open) _connection.Open();

                using (var tran = _connection.BeginTransaction())
                {
                    using (var command = _connection.CreateCommand())
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

        public int ExecuteCommand(string command_text, params SqlParameter[] parameters)
        {
            var _result = 0;

            var _connection = this.Database.GetDbConnection();
            {
                if (_connection.State != ConnectionState.Open) _connection.Open();

                using (var tran = _connection.BeginTransaction())
                {
                    using (var command = _connection.CreateCommand())
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