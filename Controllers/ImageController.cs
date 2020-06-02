using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyWebApi.Extensions;
using MyWebApi.Models;
using Npgsql;
using NpgsqlTypes;


namespace MyWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerExtension
    {
        private readonly IConfiguration _config;

        public ImageController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("GetAllImages")]
        public ActionResult GetAllImages(String ImageName)
        {
            string sql = "SELECT * FROM public.\"Image\"";
            DataTable dt = SelectData(sql);

            var objList = DataTableToList<Image>(dt);

            objList.Cast<Image>().ToList();
            //connection.Open();
            //connection.TypeMapper.UseJsonNet();

            //string sql = "SELECT * FROM public.\"Image\"";

            //var cmd = new NpgsqlCommand(sql, connection);
            ////cmd.Parameters.AddWithValue("@ImageName", ImageName);

            //NpgsqlDataReader dr = cmd.ExecuteReader();
            //StringBuilder sb = new StringBuilder();
            //while (dr.Read())
            //{
            //    Image image = new Image();
            //    image.Id = (int)dr["Id"];
            //    image.ImageName = (string)dr["ImageName"];
            //    image.Description = (string)dr["Description"];
            //    sb.Append(dr[1]);
            //}

            //connection.Close();
            return Json(objList);
        }

        [HttpGet]
        [Route("ImageId/{id}")]
        public ActionResult GetImageById(int id)
        {
            string sql = "SELECT * FROM public.\"Image\" WHERE \"Id\" = @id";
            DataTable dt = SelectData(sql, "Id", id);

            var objList = DataTableToList<Image>(dt);
            objList.Cast<Image>().ToList();
            return Json(objList);
        }


        //get db version:
        [HttpGet]
        [Route("getdbversion")]
        public string GetDBVersion()
        {
            var cs = "User ID = SuperUser;Password=qwerty;Server=localhost;Port=5432;Database=MyWebApi.Dev;Integrated Security=true;Pooling=true";
            using var con = new NpgsqlConnection(cs);
            con.Open();

            var sql = "SELECT version()";
            using var cmd = new NpgsqlCommand(sql, con);

            var version = cmd.ExecuteScalar().ToString();
            con.Close();
            return version;
        }
    }
}
