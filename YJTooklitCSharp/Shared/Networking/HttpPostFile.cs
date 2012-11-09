using System.IO;

namespace YJToolkit.YJToolkitCSharp.Networking
{
    public class HttpPostFile
    {
        public HttpPostFile(string name, string filename, Stream stream, bool closeStream = true)
        {
            Name = name;
            Filename = filename;
            Stream = stream;
            CloseStream = closeStream;
        }

        public string Name { get; private set; }
        public string Filename { get; private set; }
        public string Path { get; private set; }
        public Stream Stream { get; private set; }
        public bool CloseStream { get; private set; }

        public string ContentType { get; set; }
    }
}
