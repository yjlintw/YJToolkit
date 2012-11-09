using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace YJToolkit.YJTooklitCSharp.IP
{
    public class IpLogData
    {
        public StorageFile IpLogFile { set; get; }
        public StorageFolder IpLogFolder { set; get; }
        public string IpString { set; get; }
    }
}
