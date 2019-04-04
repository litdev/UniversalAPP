using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick; //文档地址 https://github.com/dlemstra/Magick.NET/tree/master/Documentation

namespace UniversalAPP.Web
{
    public class UploadHelper
    {
        private IHostingEnvironment _env;

        public UploadHelper(IHostingEnvironment env)
        {
            _env = env;
        }

        /// <summary>
        /// BASE64转图片
        /// UploadFromBase64("/files/base/", "data:image/jpeg;base64,其他数据");
        /// </summary>
        /// <param name="serverPath"></param>
        /// <param name="base64"></param>
        /// <returns></returns>
        public UnifiedResultEntity<string> UploadFromBase64(string serverPath, string base64)
        {
            UnifiedResultEntity<string> response_entity = new UnifiedResultEntity<string>();
            response_entity.msg = 0;
            if (string.IsNullOrEmpty(base64))
            {
                response_entity.msgbox = "缺少BASE64数据";
                return response_entity;
            }
            string ext_reg = "data:image/(?<EXT>[^<].*?);base64";
            string[] ext_list = Tools.WebHelper.GetRegValue(base64, ext_reg, "EXT", true);
            if (ext_list.Length != 1)
            {
                response_entity.msgbox = "BASE64不是图片数据";
                return response_entity;
            }
            var file_ext = ext_list[0].ToString();
            string replace_text = "data:image/" + file_ext + ";base64,";
            base64 = base64.Replace(replace_text, "");
            //要保存的文件名
            var file_name = DateTime.Now.ToFileTime() + "." + file_ext;//重新拼接

            Tools.FileHelper tf = new Tools.FileHelper(_env.WebRootPath);
            tf.CreateFiles(serverPath, true);

            var file_io_path = tf.MapPath(serverPath);
            var save_io_path = file_io_path + file_name;
            var photoBytes = Convert.FromBase64String(base64);
            System.IO.File.WriteAllBytes(save_io_path, photoBytes);
            if (!tf.IsExist(save_io_path, false))
            {
                response_entity.msgbox = "图片保存失败";
                return response_entity;
            }

            var md5 = tf.GetMD5HashFromFile(save_io_path);
            var temp_md5_name = md5 + "." + file_ext;
            var temp_md5_server_path = serverPath + temp_md5_name;
            //改名
            tf.Move(save_io_path, file_io_path, temp_md5_name);

            response_entity.msg = 1;
            response_entity.msgbox = "上传成功";
            response_entity.data = temp_md5_server_path;
            return response_entity;
        }


        public UnifiedResultEntity<string> Upload(Microsoft.AspNetCore.Http.IFormFile formFile, string serverPath, bool isThumb = true)
        {
            UnifiedResultEntity<string> response_entity = new UnifiedResultEntity<string>();
            response_entity.msg = 0;
            if (formFile == null)
            {
                response_entity.msgbox = "请选择要上传的文件";
                return response_entity;
            }

            Tools.FileHelper tf = new Tools.FileHelper(_env.WebRootPath);
            tf.CreateFiles(serverPath, true);

            var source_file_name = formFile.FileName;
            var source_file_ext = tf.GetFileExtNoFile(source_file_name);

            var file_io_path = tf.MapPath(serverPath);

            var temp_guid_name = DateTime.Now.ToFileTime() + "." + source_file_ext;
            var temp_new_guid_path = file_io_path + temp_guid_name;

            using (System.IO.FileStream fileStream = System.IO.File.Create(temp_new_guid_path))
            {
                formFile.CopyTo(fileStream);
                fileStream.Flush();
            }
            var md5 = tf.GetMD5HashFromFile(temp_new_guid_path);
            var temp_md5_name = md5 + "." + source_file_ext;
            var temp_md5_server_path = serverPath + temp_md5_name;
            //改名
            tf.Move(temp_new_guid_path, file_io_path, temp_md5_name);

            response_entity.msg = 1;
            response_entity.msgbox = "上传成功";
            response_entity.data = temp_md5_server_path;
            return response_entity;
        }

        /// <summary>
        /// 图片水印
        /// </summary>
        /// <param name="souceImagePath">原图片</param>
        /// <param name="saveImagePath">保存的图片地址，包括文件名</param>
        /// <param name="waterImagePath">水印图片</param>
        /// <param name="waterLocation">水印位置,0=未定义，1=左上，2=中上，3=右上，4=左中，5=居中，6=右中，7=左下，8=下中，9=右下</param>
        /// <param name="waterTransparency">水印的透明度 1--10 10为不透明</param>
        /// <returns></returns>
        private bool ImageWaterMark(string souceImagePath, string saveImagePath, string waterImagePath, int waterLocation, int waterTransparency)
        {
            Tools.FileHelper fileHelper = new Tools.FileHelper(_env.WebRootPath);
            if (!fileHelper.IsExist(souceImagePath, false)) return false;
            if (!fileHelper.IsExist(waterImagePath, false)) return false;

            using (MagickImage image = new MagickImage(fileHelper.MapPath(souceImagePath)))
            {
                using (MagickImage watermark = new MagickImage(fileHelper.MapPath(waterImagePath)))
                {
                    image.Composite(watermark, (Gravity)waterLocation, CompositeOperator.Over);
                    watermark.Evaluate(Channels.Alpha, EvaluateOperator.Divide, waterTransparency);

                    ////设置水印边角位置
                    //int x = image.Width - watermark.Width - 100;
                    //int y = image.Height - watermark.Height - 100;
                    ////image.Composite(watermark, x, y, CompositeOperator.Over);
                }
                image.Write(fileHelper.MapPath(saveImagePath));
            }
            return true;
        }

        /// <summary>
        /// 文字水印
        /// </summary>
        /// <param name="souceImagePath">原图片</param>
        /// <param name="saveImagePath">保存的图片地址，包括文件名</param>
        /// <param name="waterText">水印文字 换行：\r\n</param>
        /// <param name="waterLocation">水印位置,0=未定义，1=左上，2=中上，3=右上，4=左中，5=居中，6=右中，7=左下，8=下中，9=右下</param>
        /// <returns></returns>
        private bool ImageTextWaterMark(string souceImagePath, string saveImagePath, string waterText, int waterLocation)
        {
            Tools.FileHelper fileHelper = new Tools.FileHelper(_env.WebRootPath);
            if (!fileHelper.IsExist(souceImagePath, false)) return false;

            using (MagickImage image = new MagickImage(fileHelper.MapPath(souceImagePath)))
            {
                new Drawables()
                    .TextEncoding(System.Text.Encoding.UTF8)
                    .TextAntialias(true)
                    .FontPointSize(72)
                    .FillColor(MagickColors.White)
                    .Gravity((Gravity)waterLocation)
                    .Font("Microsoft YaHei & Microsoft YaHei UI")
                    .Text(38, 38, waterText)
                    .Draw(image);

                image.Write(fileHelper.MapPath(saveImagePath));
            }
            return true;
        }


        /// <summary>
        /// 图片旋转
        /// </summary>
        /// <param name="souceImagePath">原图片</param>
        /// <param name="saveImagePath">保存的图片地址，包括文件名</param>
        /// <param name="degrees">旋转度数，顺时针。为0时自动旋转</param>
        /// <returns></returns>
        private bool ImageRotate(string souceImagePath, string saveImagePath, double degrees)
        {
            Tools.FileHelper fileHelper = new Tools.FileHelper(_env.WebRootPath);
            if (!fileHelper.IsExist(souceImagePath, false)) return false;

            using (MagickImage image = new MagickImage(fileHelper.MapPath(souceImagePath)))
            {
                if (degrees <= 0) image.AutoOrient();
                else image.Rotate(degrees);
                image.Write(fileHelper.MapPath(saveImagePath));
            }
            return true;
        }

        /// <summary>
        /// 图片调整大小和压缩
        /// </summary>
        /// <param name="souceImagePath">原图片</param>
        /// <param name="saveImagePath">保存的图片地址，包括文件名</param>
        /// <param name="quality">压缩比例，100为无损压缩，递减</param>
        /// <param name="new_width">新的宽度，不调整则同时和new_height传入0</param>
        /// <param name="new_height">新的宽度，不调整则同时和new_width传入0</param>
        /// <returns></returns>
        private bool ImageReSizeAndCompression(string souceImagePath, string saveImagePath, int quality, int new_width, int new_height)
        {
            Tools.FileHelper fileHelper = new Tools.FileHelper(_env.WebRootPath);
            if (!fileHelper.IsExist(souceImagePath, false)) return false;
            using (MagickImage image = new MagickImage(fileHelper.MapPath(souceImagePath)))
            {
                image.Quality = quality;
                if (new_width > 0 && new_height > 0) image.Resize(new_width, new_height);
                image.Write(fileHelper.MapPath(saveImagePath));
            }
            return true;
        }

    }
}
