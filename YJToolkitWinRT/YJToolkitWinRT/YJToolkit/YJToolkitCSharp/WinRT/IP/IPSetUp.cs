using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using YJToolkit.YJToolkitCSharp.Storage;

namespace YJToolkit.YJToolkitCSharp.IP
{
    public static class IPSetUp
    {
        public static async Task<StorageFile> SetIPSettingsFile(StorageFolder folder, string fileName)
        {
            return await FileUtil.SetFile(folder, fileName);
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
