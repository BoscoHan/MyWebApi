using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyWebApi.Models;
using Npgsql;
using NpgsqlTypes;

namespace MyWebApi.Extensions
{
    public class ControllerExtension : Controller
    {
        protected readonly IConfiguration _config;
        protected static string cs = "User ID = SuperUser;Password=qwerty;Server=localhost;Port=5432;Database=MyWebApi.Dev;Integrated Security=true;Pooling=true";
        static readonly NpgsqlConnection connection = new NpgsqlConnection(cs);

        /// <summary>
        /// Converts a DataTable to a list with generic objects
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="table">DataTable</param>
        /// <returns>List with generic objects</returns>
        public List<T> DataTableToList<T>(DataTable table) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();
                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();
                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    list.Add(obj);
                }
                return list;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// Get data from a simple query. No params needed.
        /// </summary>
        /// <param name="query">Query to execute. Example: select * from sales</param>
        /// <returns></returns>
        public DataTable SelectData(string query)
        {
            connection.Open();
            using (var cmd = new NpgsqlCommand(query, connection))
            {
                cmd.Prepare();

                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);

                DataSet _ds = new DataSet();
                DataTable _dt = new DataTable();

                da.Fill(_ds);

                try
                {
                    _dt = _ds.Tables[0];
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Erro: ---> " + ex.Message);
                }

                connection.Close();
                return _dt;
            }
        }

        /// <summary>
        /// Get data a DataTable from a query with params.
        /// </summary>
        /// <param name="query">Query to execute. Example: select * from sales where product = @prodId</param>
        /// <param name="paramName">Param name. Example: "prodId"</param>
        /// <param name="paramValue">Param value. Example: (int)15</param>
        /// <returns></returns>
        public static DataTable SelectData(string query, string paramName, object paramValue)
        {
            connection.Open();
            using (var cmd = new NpgsqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue(paramName, paramValue);

                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);

                DataSet _ds = new DataSet();
                DataTable _dt = new DataTable();

                da.Fill(_ds);

                try
                {
                    _dt = _ds.Tables[0];
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error: ---> " + ex.Message);
                }

                connection.Close();
                return _dt;
            }
        }


        /// <summary>
        /// Get data a DataTable from a query with multiple params.
        /// </summary>
        /// <param name="query">Query to execute. Example: select * from sales where product = @prodId and sale_date = @date</param>
        /// <param name="paramName">Param name. Example: []{"prodId". "qtd"}</param>
        /// <param name="paramValue">Param value. Example: []{(int)15,(DateTime)"2017-01-01"}</param>
        /// <returns></returns>
        public static DataTable SelectData(string query, string[] paramName, object[] paramValue)
        {
            connection.Open();
            using (var cmd = new NpgsqlCommand(query, connection))
            {
                //Verify if the name's count equals the value's count
                if (paramName.Count() != paramValue.Count())
                {
                    Debug.WriteLine("ParamName Count != ParamValue Count");
                    return null;
                }

                //Add params in the arrays
                for (int i = 0; i < paramName.Count(); i++)
                {
                    cmd.Parameters.AddWithValue(paramName[i], paramValue[i]);
                }


                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);

                DataSet _ds = new DataSet();
                DataTable _dt = new DataTable();

                da.Fill(_ds);

                try
                {
                    _dt = _ds.Tables[0];
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error: ---> " + ex.Message);
                }

                connection.Close();
                return _dt;
            }
        }
    }
}
