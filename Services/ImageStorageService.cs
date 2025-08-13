using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace JouleadAssistantDesign.Services
{
    public class ImageStorageService
    {
        private static readonly string ImagesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

        public ImageStorageService()
        {
            // 确保图片目录存在
            if (!Directory.Exists(ImagesDirectory))
            {
                Directory.CreateDirectory(ImagesDirectory);
            }
        }

        /// <summary>
        /// 将图片文件复制到应用程序目录并返回相对路径
        /// </summary>
        /// <param name="sourceFilePath">源图片文件路径</param>
        /// <returns>相对路径（用于数据库存储）</returns>
        public string StoreImage(string sourceFilePath)
        {
            if (!File.Exists(sourceFilePath))
                throw new FileNotFoundException($"源文件不存在: {sourceFilePath}");

            // 生成唯一的文件名（使用文件内容的MD5哈希）
            string fileExtension = Path.GetExtension(sourceFilePath);
            string fileName = GenerateFileName(sourceFilePath) + fileExtension;
            string targetPath = Path.Combine(ImagesDirectory, fileName);

            // 如果文件已存在，直接返回相对路径
            if (File.Exists(targetPath))
            {
                return GetRelativePath(targetPath);
            }

            // 复制文件到目标位置
            File.Copy(sourceFilePath, targetPath, false);
            return GetRelativePath(targetPath);
        }

        /// <summary>
        /// 存储多个图片文件
        /// </summary>
        /// <param name="sourceFilePaths">源图片文件路径数组</param>
        /// <returns>相对路径字符串，用分号分隔</returns>
        public string StoreImages(string[] sourceFilePaths)
        {
            if (sourceFilePaths == null || sourceFilePaths.Length == 0)
                return string.Empty;

            var storedPaths = new string[sourceFilePaths.Length];
            for (int i = 0; i < sourceFilePaths.Length; i++)
            {
                storedPaths[i] = StoreImage(sourceFilePaths[i]);
            }

            return string.Join(";", storedPaths);
        }

        /// <summary>
        /// 获取图片的完整路径
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <returns>完整路径</returns>
        public string GetFullPath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return string.Empty;

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
        }

        /// <summary>
        /// 获取多个图片的完整路径
        /// </summary>
        /// <param name="relativePaths">相对路径字符串，用分号分隔</param>
        /// <returns>完整路径数组</returns>
        public string[] GetFullPaths(string relativePaths)
        {
            if (string.IsNullOrEmpty(relativePaths))
                return new string[0];

            var paths = relativePaths.Split(';', StringSplitOptions.RemoveEmptyEntries);
            return paths.Select(p => GetFullPath(p)).ToArray();
        }

        /// <summary>
        /// 删除图片文件
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        public void DeleteImage(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return;

            string fullPath = GetFullPath(relativePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        /// <summary>
        /// 删除多个图片文件
        /// </summary>
        /// <param name="relativePaths">相对路径字符串，用分号分隔</param>
        public void DeleteImages(string relativePaths)
        {
            if (string.IsNullOrEmpty(relativePaths))
                return;

            var paths = relativePaths.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var path in paths)
            {
                DeleteImage(path);
            }
        }

        /// <summary>
        /// 生成基于文件内容的唯一文件名
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>唯一文件名（不含扩展名）</returns>
        private string GenerateFileName(string filePath)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        /// <summary>
        /// 获取相对路径
        /// </summary>
        /// <param name="fullPath">完整路径</param>
        /// <returns>相对于应用程序目录的路径</returns>
        private string GetRelativePath(string fullPath)
        {
            var appBase = AppDomain.CurrentDomain.BaseDirectory;
            var uri1 = new Uri(appBase);
            var uri2 = new Uri(fullPath);
            var relativeUri = uri1.MakeRelativeUri(uri2);
            return Uri.UnescapeDataString(relativeUri.ToString());
        }
    }
}

