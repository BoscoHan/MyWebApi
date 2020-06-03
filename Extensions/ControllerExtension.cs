﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyWebApi.Models;
using Npgsql;
using NpgsqlTypes;
using Microsoft.AspNetCore.Mvc.Routing;

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
                    connection.Close();
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
                    connection.Close();
                }

                connection.Close();
                return _dt;
            }
        }


        /// <summary>
        /// Get data a DataTable from a query with params and match param
        /// </summary>
        /// <param name="query">Query to execute. Example: select * from sales where product like '%@prodName%'</param>
        /// <param name="paramName">Param name. Example: "prodName"</param>
        /// <param name="paramValue">Param value. Example: (string)fridge</param>
        /// <returns></returns>
        public static DataTable SelectDataMatchSubStr(string query, string paramName, object paramValue)
        {
            connection.Open();
            using (var cmd = new NpgsqlCommand(query, connection))
            {
                //using sqlParams like this prevents sql injection:
                cmd.Parameters.AddWithValue(paramName, "%" + paramValue + "%");
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
                    connection.Close();
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
                    connection.Close();
                }

                connection.Close();
                return _dt;
            }
        }


        //execute with ExecuteNonQuery() method, 
        //args: SQL query and list of params
        public Boolean ExecuteInsertImage(string query, InsertImageModel imageModel)
        {
            connection.Open();
            bool success = true;
            using (var cmd = new NpgsqlCommand(query, connection))
            {
                try
                {
                    cmd.Parameters.AddWithValue("ImageName", imageModel.ImageName);
                    cmd.Parameters.AddWithValue("Description", imageModel.Description);
                    cmd.Parameters.AddWithValue("price", imageModel.Price);
                    cmd.Parameters.AddWithValue("quantity", imageModel.Quantity);
                    cmd.Parameters.AddWithValue("IsPrivate", imageModel.Isprivate);
                    cmd.Parameters.AddWithValue("UserId", imageModel.UserId);
                    cmd.Parameters.AddWithValue("ImgByte", imageModel.ImgByte);

                    int result = cmd.ExecuteNonQuery();
                    //nothing inserted, something went wrong
                    if (result < 0)
                        success = false;
                }
                catch (SqlException e)
                {
                    Console.WriteLine("SqlException caught " + e);
                }
            }
            connection.Close();
            return success;
        }

        //would be nice to wrap in transaction, when multiple instances co-exist:
        public Boolean ExecuteDeleteImage(string deleteQuery, UpdateImageUserModel imageUserModel)
        {
            bool success = true;
            //NpgsqlTransaction transaction = null;

            //first fetch the table to see owner and permissions:
            string selectSql = "SELECT * FROM public.\"Image\" WHERE \"Id\" = @id";

            //transaction = connection.BeginTransaction();
            DataTable dt = SelectData(selectSql, "Id", imageUserModel.ImageId);
            var objList = DataTableToList<Image>(dt);
            objList.Cast<Image>().ToList();
            
            if (!objList.Any())
            {
                Console.WriteLine("Image cannot be deleted as it does not exist");
                //transaction.Rollback();
                return false;
            }

            int OwnerId = objList[0].UserId;
            var isPrivate = objList[0].Isprivate;

            if (isPrivate && imageUserModel.UserId != OwnerId)
            {
                Console.WriteLine("Image cannot be deleted as the current user does not have rights to modify!");
                //transaction.Rollback();
                return false;
            }

            //images not marked private could be modified by anyone:

            connection.Open();
            using (var cmd = new NpgsqlCommand(deleteQuery, connection))
            {
                try 
                {
                    cmd.Parameters.AddWithValue("ImageId", imageUserModel.ImageId);
                    int result = cmd.ExecuteNonQuery();

                    if (result < 0)
                        success = false;
                }
                catch (SqlException e)
                {
                    Console.WriteLine("SqlException caught " + e);
                }
            }
            //transaction.Commit();
            return success;
        }
    }
}
