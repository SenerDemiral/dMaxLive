using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataLibrary;
using DataLibrary.Models;

namespace dMax.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private IDataAccess db;

        public UploadController(IWebHostEnvironment hostingEnvironment, IDataAccess data)
        {
            _hostingEnvironment = hostingEnvironment;
            db = data;
        }

        [Route("UploadFile/{dtid}/{id}")]
        [HttpPost]
        public ActionResult UploadFile(int dtid, int id, IFormFile myFile)
        {
            try
            {
                // Specify the target location for the uploaded files.
                var path = Path.Combine(_hostingEnvironment.WebRootPath, $"uploads/{dtid}");
                // Check whether the target directory exists; otherwise, create it.
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                //using (var fileStream = System.IO.File.Create(Path.Combine(path, myFile.FileName)))
                //FileInfo fi = new FileInfo(myFile.FileName);
                //using FileStream fileStream = System.IO.File.Create(Path.Combine(path, $"KT{id}{fi.Extension}"));
                using FileStream fileStream = System.IO.File.Create(Path.Combine(path, $"{id}.jpg"));
                // Check the file here (its extension, safity, and so on). 
                // If all checks are passed, save the file.
                myFile.CopyTo(fileStream);
            }
            catch
            {
                Response.StatusCode = 400;
            }

            return new EmptyResult();
        }

        [Route("UploadDocumentFile/{dtid}/{id}")]
        [HttpPost]
        public ActionResult UploadDocumentFile(int dtid, int id, IFormFile myFile)
        {
            try
            {
                // Specify the target location for the uploaded files.
                var path = Path.Combine(_hostingEnvironment.WebRootPath, $"uploads/{dtid}");
                // Check whether the target directory exists; otherwise, create it.
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                FileInfo fi = new FileInfo(myFile.FileName);
                using FileStream fileStream = System.IO.File.Create(Path.Combine(path, $"{id}{fi.Extension}"));
                // Check the file here (its extension, safity, and so on). 
                // If all checks are passed, save the file.
                myFile.CopyTo(fileStream);

                //db.SaveData<FHmodel>($"update FH set Ext = '{fi.Extension}' where FhID = {id}", new FHmodel { });
                db.SaveData($"update FH set Ext = '{fi.Extension}' where FhID = {id}", new { });
            }
            catch
            {
                Response.StatusCode = 400;
            }

            return new EmptyResult();
        }
    }
}
