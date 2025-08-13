using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Fonts;

using System.IO;

namespace JouleadAssistantDesign.Services
{
    public class FontResolver : IFontResolver
    {
        // 当 MigraDoc 请求某个字体时，都会走这里
        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            // 统一映射到微软雅黑或加粗微软雅黑
            var face = isBold ? "Microsoft YaHei Bold" : "Microsoft YaHei";
            return new FontResolverInfo(face);
        }

        // 告诉 MigraDoc 去哪加载字体文件
        public byte[] GetFont(string faceName)
        {
            // Windows 系统字体目录
            var fonts = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);

            // 根据解析器中返回的 FaceName，找到对应的 .ttc 文件
            string file = faceName switch
            {
                "Microsoft YaHei" => "msyh.ttc",
                "Microsoft YaHei Bold" => "msyhbd.ttc",
                _ => throw new FileNotFoundException($"找不到字体：{faceName}")
            };

            var path = Path.Combine(fonts, file);
            if (!File.Exists(path))
                throw new FileNotFoundException($"字体文件不存在：{path}");

            return File.ReadAllBytes(path);
        }
    }
}