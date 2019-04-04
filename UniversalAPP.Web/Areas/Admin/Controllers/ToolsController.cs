using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ImageMagick;

namespace UniversalAPP.Web.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize]
    public class ToolsController : BaseAdminController
    {
        /// <summary>
        /// 设置分页大小Cookie
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ContentResult SetPageCookie(string cname, int num)
        {
            if (!string.IsNullOrWhiteSpace(cname) && num >= 3)
            {
                SetCookies(cname, num.ToString(), 10000);
            }
            return Content("");
        }

        /// <summary>
        /// 设置排序方式Cookie
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ContentResult SetSortCookie(string cname, string orderby)
        {
            if (!string.IsNullOrWhiteSpace(cname) && !string.IsNullOrWhiteSpace(orderby))
            {
                SetCookies(cname, orderby, 10000);
            }
            return Content("");
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        //[AdminPermission("其他","上传文件")]
        [HttpGet]
        public IActionResult UploadFile(AdminFileUploadType type, string folder, int num, string call_back_ele, string call_func)
        {
            string file_ext = "";
            string file_txt = "";
            string file_size = "0KB";
            bool multi = false; //是否可上传多个
            if (type == AdminFileUploadType.MorePicture)
            {
                multi = true;
            }
            //可上传文件后缀
            switch (type)
            {
                case AdminFileUploadType.OnePicture:
                case AdminFileUploadType.MorePicture:
                    file_ext = "image/*";
                    file_txt = "图片";
                    file_size = "5MB";
                    break;
                case AdminFileUploadType.APK:
                    file_ext = "*.apk;*.APK";
                    file_txt = "apk文件";
                    file_size = "50MB";
                    break;
                case AdminFileUploadType.IPA:
                    file_ext = "*.ipa;*.IPA";
                    file_txt = "ipa文件";
                    file_size = "50MB";
                    break;
                case AdminFileUploadType.ZIP:
                    file_ext = "*.zip;*.ZIP";
                    file_txt = "zip压缩包";
                    file_size = "50MB";
                    break;
                default:
                    break;
            }
            ViewData["call_back_ele"] = call_back_ele;
            ViewData["file_ext"] = file_ext;
            ViewData["multi"] = multi.ToString().ToLower();
            ViewData["file_txt"] = file_txt;
            ViewData["file_size"] = file_size;
            ViewData["folder"] = folder;
            ViewData["num"] = num;
            ViewData["call_func"] = call_func;
            ViewData["upload_type"] = type;
            return View();
        }


        [HttpPost]
        public IActionResult UploadFile(IList<IFormFile> Filedata, string operation, AdminFileUploadType upload_type)
        {
            if (Filedata.Count == 0)
            {
                return ResultBasicString(0, "没有上传文件", "");
            }
            if (string.IsNullOrWhiteSpace(operation))
            {
                return ResultBasicString(0, "保存位置不明确", "");
            }

            //保存的目录
            string filePath = "/files/" + operation + "/";
            UploadHelper uploadHelper = new UploadHelper(_env);
            switch (upload_type)
            {
                case AdminFileUploadType.OnePicture:
                case AdminFileUploadType.MorePicture:
                case AdminFileUploadType.APK:
                case AdminFileUploadType.IPA:
                case AdminFileUploadType.ZIP:
                    var result = uploadHelper.Upload(Filedata[0], filePath);
                    return Json(result);
                case AdminFileUploadType.FroalaEditor:
                    var result_textarea = uploadHelper.Upload(Filedata[0], filePath);
                    return Json(new FroalaEditorResult(result_textarea.msg == 1 ? result_textarea.data : ""));
                default:
                    break;
            }


            return ResultBasicString(0, "aaa", "dsdf");
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Verification()
        {
            using (MagickImage image = new MagickImage(MagickColors.White, 100, 40))
            {
                new Drawables()
                    .TextEncoding(System.Text.Encoding.UTF8)
                    .TextAntialias(true)
                    .FontPointSize(40)
                    .FillColor(MagickColors.Green)
                    .Gravity(Gravity.Center)
                    .Font("Microsoft YaHei & Microsoft YaHei UI")
                    .Rotation(-8)
                    .Text(1, 1, "ABDR")
                    .Draw(image);
                image.Quality = 70;
                image.AddNoise(NoiseType.Poisson);//噪点
                return File(image.ToByteArray(MagickFormat.Jpg), "image/jpg");
            }
        }

    }


    /// <summary>
    /// 富文本编辑器上传文件返回值
    /// </summary>
    internal class FroalaEditorResult
    {
        /// <summary>
        /// 
        /// </summary>
        public FroalaEditorResult()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="link"></param>
        public FroalaEditorResult(string link)
        {
            this.link = link;
        }

        /// <summary>
        /// 
        /// </summary>
        public string link { get; set; }
    }

}