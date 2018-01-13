using System.IO;

namespace GMBT
{
    public static class ResourcesManager
    {
        public static string GetResourceFilePath(this byte[] bytes)
        {
            return GetResourceFilePath(bytes, string.Empty);
        }

        public static string GetResourceFilePath(this byte[] bytes, string extension)
        {
            string path = Path.GetTempFileName();
            string newPath = path + extension;

            File.Move(path, newPath);

            File.WriteAllBytes(newPath, bytes);

            return newPath;
        }
    }
}
