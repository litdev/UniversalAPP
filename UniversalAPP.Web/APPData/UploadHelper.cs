using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public UnifiedResultEntity<string> UploadFromBase64(string serverPath,string base64)
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
            if (!tf.IsExist(save_io_path,false))
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
    }
}
