using AWS_Demo.Helper;
using AWS_Demo.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AWS_Demo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            List<S3ObjectModel> s3ObjectModels = S3Helper.Instance.ListingObjects();
            return View(s3ObjectModels);
        }


        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    S3Helper.Instance.SimpleUploadFile(file);
                }

                return Redirect("~/?msg=success");
            }
            catch
            {
                return Redirect("~/?msg=error");
            }
        }


        public ActionResult DeleteObject(string name)
        {
            try
            {
                S3Helper.Instance.DeletingAnObject(name);
                return Redirect("~/?msg=deleted");
            }
            catch
            {
                return Redirect("~/?msg=Notdeleted");
            }
        }

        public ActionResult DownloadObject(string name)
        {
            try
            {
                string filePath = Server.MapPath("~/Content/Download/");
                if (System.IO.File.Exists(filePath + name))
                {
                    System.IO.File.Delete(filePath + name);
                }

                string origninalFileName = S3Helper.Instance.GetFile(name, filePath);
                if (!String.IsNullOrEmpty(origninalFileName))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath + origninalFileName);
                    return File(fileBytes, MimeMapping.GetMimeMapping(origninalFileName), origninalFileName);
                }


                return Redirect("~/?msg=Notdownload");
            }
            catch
            {
                return Redirect("~/?msg=Notdownload");
            }
        }

        public ActionResult CreateBucket()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateBucket(S3BucketModel model)
        {
            string message= S3Helper.Instance.CreateBucket(model.BucketName);

            ViewBag.Message = message;

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}