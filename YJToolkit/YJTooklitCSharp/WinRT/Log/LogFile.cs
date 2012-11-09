using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace YJToolkit.YJToolkitCSharp.Log
{
    class LogFile
    {
        private static async Task<bool> DoesFileExistAsync(StorageFile file, StorageFolder folder, string fileName)
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

        public static async Task WriteNewLogAsync(StorageFile file, StorageFolder folder, string fileName, string logString)
        {
            if (!(await DoesFileExistAsync(file, folder, fileName)))
            {
                file = await folder.CreateFileAsync(fileName);
            }

            await FileIO.AppendTextAsync(file, logString + Environment.NewLine);
        }
    }
}
