using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace UniversalAPP.Tools
{
    /// <summary>
    /// IO操作
    /// </summary>
    public class IOHelper
    {
        /// <summary>
        /// 获取图片的宽高,返回HashTable，宽的键:w,高的键:h
        /// </summary>
        /// <param name="file_path">绝对路径</param>
        /// <returns></returns>
        public static Hashtable GetImageWidthHeight(string file_path)
        {

            Hashtable hash = new Hashtable();
            //System.Drawing.Image temp_image = System.Drawing.Image.FromFile(file_path);
            
            hash["w"] = 10;//temp_image.Width;
            hash["h"] = 20;//temp_image.Height;
            //temp_image.Dispose();
            return hash;
        }
        

    }
}
