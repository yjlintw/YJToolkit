using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using System.IO;

namespace YJToolkit.YJToolkitCSharp.Camera
{
    public class DeviceInfo
    {
        public string deviceName { get; set; }

        public string deviceID { get; set; }

        public List<VideoEncodingProperties> resolutionList;

        public Windows.Devices.Enumeration.DeviceInformation deviceInfo { get; set; }

        public DeviceInfo()
        {
            resolutionList = new List<VideoEncodingProperties>();
        }
    }

    public class CameraUtil
    {
        public static async Task<MediaCapture> SetCameraWithSetting(MediaCapture mediaCapture, DeviceInfo device, uint width, uint height, string type, MediaCaptureInitializationSettings captrueInitSettings)
        {
            setCaptureSettings(out captrueInitSettings, device.deviceID);

            mediaCapture = new MediaCapture();
            await mediaCapture.InitializeAsync(captrueInitSettings);
            int index = -1;
            for (int i = 0; i < device.resolutionList.Count; i++)
            {
                VideoEncodingProperties properties = device.resolutionList[i];
                if (properties.Width == width && properties.Height == height)
                {
                    if (type == string.Empty)
                    {
                        index = i;
                        break;
                    }
                    else if (string.Compare(properties.Subtype, type) == 0)
                    {
                        index = i;
                        break;
                    }
                }
            }

            if (index == -1)
                return null;

            try
            {
                await mediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.VideoPreview, device.resolutionList[index]);
            }
            catch
            {
            }
            return mediaCapture;
        }

        public static async Task<List<DeviceInfo>> enumerateCameras()
        {
            var deviceInfo = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(Windows.Devices.Enumeration.DeviceClass.VideoCapture);
            List<DeviceInfo> devices = new List<DeviceInfo>();
            for (int i = 0; i < deviceInfo.Count; i++)
            {
                DeviceInfo device = new DeviceInfo();
                device.deviceID = deviceInfo[i].Id;
                device.deviceInfo = deviceInfo[i];
                device.deviceName = deviceInfo[i].Name;

                try
                {
                    MediaCaptureInitializationSettings mediaSetting = new MediaCaptureInitializationSettings();
                    setCaptureSettings(out mediaSetting, device.deviceID);
                    MediaCapture mCapture = new MediaCapture();
                    await mCapture.InitializeAsync(mediaSetting);
                    device.resolutionList = updateResolution(mCapture);
                }
                catch
                {
                }

                devices.Add(device);
            }

            return devices;
        }

        public static DeviceInfo getDeviceByID(List<DeviceInfo> devices, string deviceID)
        {
            foreach (DeviceInfo device in devices)
            {
                if (string.Compare(device.deviceID, deviceID) == 0)
                {
                    return device;
                }
            }
            return null;
        }

        public static List<VideoEncodingProperties> updateResolution(MediaCapture mediaCapture)
        {
            System.Collections.Generic.IReadOnlyList<IMediaEncodingProperties> res = mediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.VideoPreview);

            List<VideoEncodingProperties> vps = new List<VideoEncodingProperties>();
            foreach (var r in res)
            {
                VideoEncodingProperties vp = r as VideoEncodingProperties;
                vps.Add(vp);
            }

            return vps;
        }

        public static void setCaptureSettings(out MediaCaptureInitializationSettings captrueInitSettings, string id)
        {
            captrueInitSettings = new MediaCaptureInitializationSettings();
            captrueInitSettings.AudioDeviceId = "";
            captrueInitSettings.VideoDeviceId = "";
            captrueInitSettings.StreamingCaptureMode = StreamingCaptureMode.Video;
            captrueInitSettings.PhotoCaptureSource = PhotoCaptureSource.VideoPreview;
            captrueInitSettings.VideoDeviceId = id;
        }

        public static void debugWriteAll(List<DeviceInfo> devices)
        {
            foreach (DeviceInfo device in devices)
            {
                debugWriteSingle(device);
            }
        }

        public static void debugWriteSingle(DeviceInfo device)
        {
            string outputString = String.Format(@"Device Name: {0}Device Id: {1}", device.deviceName + Environment.NewLine, device.deviceID + Environment.NewLine);
            string resString = "";
            int count = 0;
            foreach (VideoEncodingProperties property in device.resolutionList)
            {
                resString += string.Format("Index: {0}", count);
                resString += Environment.NewLine;
                resString += string.Format("Resolution: {0} x {1}", property.Width, property.Height);
                resString += Environment.NewLine;
                resString += string.Format("Type: {0}", property.Subtype);
                resString += Environment.NewLine + Environment.NewLine;

                count++;
            }
            outputString += resString;

            Debug.WriteLine(outputString);
        }
    }

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