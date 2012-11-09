using System;
using System.Threading.Tasks;
using Windows.Storage;
using YJToolkit.YJToolkitCSharp.Storage;

namespace YJToolkit.YJToolkitCSharp.Log
{
    class LogFile
    {
        public static async Task WriteNewLogAsync(StorageFile file, StorageFolder folder, string fileName, string logString)
        {
            if (!(await FileUtil.DoesFileExistAsync(file, folder, fileName)))
            {
                file = await folder.CreateFileAsync(fileName);
            }

            await FileIO.AppendTextAsync(file, logString + Environment.NewLine);
        }
    }
}
