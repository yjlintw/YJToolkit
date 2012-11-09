using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;

namespace YJToolkit.YJToolkitCSharp.IP
{
    public static class IPSetUp
    {

        private static async Task<StorageFile> DoesFileExistAsync(StorageFolder folder, string fileName)
        {
            StorageFile file = null;
            try
            {
                file = await folder.GetFileAsync(fileName);
                return file;
            }
            catch
            {
                return file;
            }
        }

        public static async Task<StorageFile> SetIPSettingsFile(StorageFolder folder, string fileName)
        {
            StorageFile file = await DoesFileExistAsync(folder, fileName);
            if (file == null)
            {
                file = await folder.CreateFileAsync(fileName);
            }
            return file;
        }

        public static async void WriteIPSettingsAsync(StorageFile file, string ipString)
        {
            try
            {
                if (file != null)
                {
                    await FileIO.WriteTextAsync(file, ipString);
                }
            }
            catch (FileNotFoundException)
            {

            }
        }

        public static async Task<string> ReadIPSettingsAsync(StorageFile file)
        {
            string result = string.Empty;
            try
            {
                if (file != null)
                {
                    result = await FileIO.ReadTextAsync(file);
                }
                return result;
            }
            catch (FileNotFoundException)
            {
                return result;
            }
        }
    }
}
