using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyWebApi.Extensions;
using MyWebApi.Models;
using MyWebApi.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        [HttpGet]
        [Route("ImageName/{ImageName}")]
        public ActionResult GetImageByName(string ImageName)
        {
            string sql = "SELECT * FROM public.\"Image\" WHERE \"ImageName\" LIKE @ImageName";
            DataTable dt = SelectDataMatchSubStr(sql, "ImageName", ImageName);

            var objList = DataTableToList<Image>(dt);
            objList.Cast<Image>().ToList();
            return Json(objList);
        }


        [HttpGet]
        [Route("ImageByUserId/{UserId}")]
        public ActionResult GetImageAssociatedWithUser(int UserId)
        {
            string sql = "SELECT * FROM public.\"Image\" WHERE \"UserId\" = @UserId";
            DataTable dt = SelectData(sql, "UserId", UserId);

            var objList = DataTableToList<Image>(dt);
            objList.Cast<Image>().ToList();
            return Json(objList);
        }

        [HttpGet]
        [Route("ImageDescription/{Description}")]
        public ActionResult GetImageByDescription(string Description)
        {
            string sql = "SELECT * FROM public.\"Image\" WHERE \"Description\" LIKE @Description";
            DataTable dt = SelectDataMatchSubStr(sql, "Description", Description);

            var objList = DataTableToList<Image>(dt);
            objList.Cast<Image>().ToList();
            return Json(objList);
        }

        [HttpPost]
        [Route("DeleteImage")]
        public IActionResult DeleteImage(DeleteImageUserModel imageModel)
        {
            if (imageModel == null)
                throw new ArgumentException("invalid imageModel");

            string sql = "DELETE FROM public.\"Image\" WHERE \"Id\" = @ImageId";

            return ExecuteDeleteImage(sql, imageModel) == true ? Json(HttpStatusCode.OK) : Json(HttpStatusCode.BadRequest);
        }

        [HttpPost]
        [Route("InsertImage")]
        public IActionResult InsertImage(InsertImageModel imageModel)
        {
            //could ideally use LINQ or other ORM library, or even move this to XML level to clean it up.
            //just avoid sql injection by parameterizing for now: 
            string query = "INSERT INTO public.\"Image\"(\"ImageName\", \"Description\", \"Price\", \"Quantity\", \"Isprivate\", \"UserId\", \"ImgByte\") " +
                "VALUES(@ImageName, @Description, @price, @quantity, @IsPrivate, @UserId, @ImgByte); ";

            //read the file to bytes 
            ImageService service = new ImageService();
            byte[] filebytes = service.ReadAllBytes(imageModel.Path);
            imageModel.ImgByte = filebytes;

            return ExecuteInsertImage(query, imageModel) == true ? Json(HttpStatusCode.OK) : Json(HttpStatusCode.BadRequest);
        }

        [HttpPut]
        [Route("UpdateImage")]
        public IActionResult UpdateImage(UpdateImageUserModel imageModel)
        {
            string updateQuery = "UPDATE public.\"Image\" SET \"ImageName\" = @ImageName, \"Description\" = @Description, \"Price\" = @price, \"Quantity\" = @quantity, \"Isprivate\" = @IsPrivate, \"ImgByte\" = @ImgByte WHERE \"Id\" = @ImageId";

            ImageService service = new ImageService();
            if (imageModel.Path != null)
            {
                byte[] filebytes = service.ReadAllBytes(imageModel.Path);
                imageModel.ImgByte = filebytes;
            }

            return ExecuteUpdateImage(updateQuery, imageModel) == true ? Json(HttpStatusCode.OK) : Json(HttpStatusCode.BadRequest);
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
