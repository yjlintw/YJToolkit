using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace YJToolkit.YJToolkitCSharp.Storage
{
    public class FileUtil
    {
        public static async Task<bool> DoesFileExistAsync(StorageFile file, StorageFolder folder, string fileName)
        {
            try
            {
                file = await folder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<StorageFile> SetFile(StorageFolder folder, string fileName)
        {
            StorageFile file = null;
            if (!(await DoesFileExistAsync(file, folder, fileName)))
            {
                file = await folder.CreateFileAsync(fileName);
            }
            return file;
        }
    }
}
