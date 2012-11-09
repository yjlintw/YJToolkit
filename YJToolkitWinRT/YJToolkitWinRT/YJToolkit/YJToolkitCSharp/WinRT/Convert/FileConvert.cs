using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace YJToolkit.YJToolkitCSharp.Convert
{
    class FileConvert
    {
        public static async Task<Stream> IRandomAccessStreamToStream(IRandomAccessStream irStream)
        {

            byte[] pixels = await IRandomAccessStreamToByteArr(irStream);

            Stream stream = new MemoryStream(pixels);

            return stream;
        }

        public static async Task<byte[]> IRandomAccessStreamToByteArr(IRandomAccessStream irStream)
        {
            DataReader reader = new Windows.Storage.Streams.DataReader(irStream.GetInputStreamAt(0));
            await reader.LoadAsync((uint)irStream.Size);

            byte[] pixels = new byte[irStream.Size];
            reader.ReadBytes(pixels);

            return pixels;
        }

        public static async Task<Stream> UriToStream(Uri uri)
        {
            RandomAccessStreamReference raReference = RandomAccessStreamReference.CreateFromUri(uri);
            IRandomAccessStream irStream = await raReference.OpenReadAsync();

            Stream stream = await IRandomAccessStreamToStream(irStream);

            return stream;
        }
    }
}
