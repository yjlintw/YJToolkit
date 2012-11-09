using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;

namespace YJToolkit.YJToolkitCSharp.IP
{
    class IPSetUp
    {
        static readonly IPSetUp _instance = new IPSetUp();
        private string ipSetFileName = "ServerIP.txt";
        private StorageFolder ipFolder = KnownFolders.DocumentsLibrary;
        private StorageFile ipFile = null;

        public string ipString = string.Empty;

        private IPSetUp()
        {
            init();
        }

        private async void init()
        {
            await SetIPSettingsFile(ipSetFileName);
            await ReadIPSettingsAsync();
        }

        public static IPSetUp Instance
        {
            get
            {
                return _instance;
            }
        }

        private async Task<bool> DoesFileExistAsync(string fileName)
        {
            try
            {
                ipFile = await ipFolder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task SetIPSettingsFile(string fileName)
        {
            if (!(await DoesFileExistAsync(fileName)))
            {
                ipFile = await ipFolder.CreateFileAsync(fileName);
            }
            return;
        }

        public async void WriteIPSettingsAsync()
        {
            try
            {
                if (ipFile != null)
                {
                    await FileIO.WriteTextAsync(ipFile, ipString);
                }
            }
            catch (FileNotFoundException)
            {

            }
        }

        public async Task ReadIPSettingsAsync()
        {
            try
            {
                if (ipFile != null)
                {
                    ipString = await FileIO.ReadTextAsync(ipFile);
                }
            }
            catch (FileNotFoundException)
            {

            }

            return;

        }
    }
}
