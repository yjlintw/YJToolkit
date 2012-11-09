using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Media.MediaProperties;
using Windows.Storage;

namespace YJToolkit.YJToolkitCSharp.Camera
{
    public class SetCameraUtil
    {
        static readonly SetCameraUtil _instance = new SetCameraUtil();
        private string _cameraSetFileName = "cameraSettings.txt";
        private StorageFolder _cameraSetFolder = KnownFolders.DocumentsLibrary;
        private StorageFile _cameraSettingsFile = null;

        public string cameraID1 = string.Empty;
        public uint camera1Width = 0;
        public uint camera1Height = 0;
        public string camera1Subtype = string.Empty;
        public VideoEncodingProperties cameraProperties1 = null;

        public string cameraID2 = string.Empty;
        public uint camera2Width = 0;
        public uint camera2Height = 0;
        public string camera2Subtype = string.Empty;
        public VideoEncodingProperties cameraProperties2 = null;

        private SetCameraUtil()
        {
            init();
        }

        private async void init()
        {
            await setCameraSettingsFile(_cameraSetFileName);
        }

        public static SetCameraUtil Instance
        {
            get
            {
                return _instance;
            }
        }

        public VideoEncodingProperties CameraProperties1
        {
            set
            {
                cameraProperties1 = value;
                camera1Height = cameraProperties1.Height;
                camera1Width = cameraProperties1.Width;
            }
            get
            {
                return cameraProperties1;
            }
        }

        public VideoEncodingProperties CameraProperties2
        {
            set
            {
                cameraProperties2 = value;
                camera2Height = cameraProperties2.Height;
                camera2Width = cameraProperties2.Width;
            }
            get
            {
                return cameraProperties2;
            }
        }

        private async Task<bool> DoesFileExistAsync(string fileName)
        {
            try
            {
                _cameraSettingsFile = await _cameraSetFolder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task setCameraSettingsFile(string fileName)
        {
            if (!(await DoesFileExistAsync(fileName)))
            {
                _cameraSettingsFile = await _cameraSetFolder.CreateFileAsync(fileName);
            }
            return;
        }

        public async void WriteCameraSettingsAsync()
        {
            try
            {
                if (_cameraSettingsFile != null)
                {
                    await FileIO.WriteTextAsync(_cameraSettingsFile, String.Format("{0},{1},{2},{3},{4},{5},{6},{7}", cameraID1, camera1Width, camera1Height, camera1Subtype, cameraID2, camera2Width, camera2Height, camera2Subtype));
                }
            }
            catch (FileNotFoundException)
            {

            }
        }

        public async Task ReadCameraSettingsAsync()
        {
            try
            {
                if (_cameraSettingsFile != null)
                {
                    string tempData = await FileIO.ReadTextAsync(_cameraSettingsFile);
                    string[] dataArr = tempData.Split(',');
                    cameraID1 = dataArr[0];
                    camera1Width = uint.Parse(dataArr[1]);
                    camera1Height = uint.Parse(dataArr[2]);
                    camera1Subtype = dataArr[3];

                    cameraID2 = dataArr[4];
                    camera2Width = uint.Parse(dataArr[5]);
                    camera2Height = uint.Parse(dataArr[6]);
                    camera2Subtype = dataArr[7];
                }
            }
            catch (FileNotFoundException)
            {

            }

            return;

        }
    }
}
