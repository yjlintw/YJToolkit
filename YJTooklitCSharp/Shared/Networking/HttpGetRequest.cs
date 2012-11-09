using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace YJToolkit.YJToolkitCSharp.Networking
{
    public interface IHttpRequest
    {
        Uri Uri { get; }
        Encoding Encoding { get; }
        object Tag { get; }
        int ConnectionTimeout { get; }

        bool ResponseAsStream { get; }
    }

    public class HttpGetRequest : IHttpRequest
    {
        public HttpGetRequest(string uri)
            :this(new Uri(uri, UriKind.RelativeOrAbsolute))
        {}

        public HttpGetRequest(string uri, Dictionary<string, string> query)
            : this(new Uri(uri, UriKind.RelativeOrAbsolute), query)
        {}

        public HttpGetRequest(Uri uri)
            : this(uri, new Dictionary<string, string>())
        {}

        public HttpGetRequest(Uri uri, Dictionary<string, string> query)
        {
            Uri = uri;
            Query = query ?? new Dictionary<string, string>();
            Headers = new Dictionary<string, string>();
            Cookies = new List<Cookie>();
            UseCache = true;
            Encoding = Encoding.UTF8;
            ConnectionTimeout = 0;
            ResponseAsStream = false;
        }

        public int ConnectionTimeout { get; set; }
        public bool ResponseAsStream { get; set; }

        public Uri Uri { get; private set; }
        public Dictionary<string, string> Query { get; private set; }
        public List<Cookie> Cookies { get; private set; }
        public Dictionary<string, string> Headers { get; private set; }

        public bool UseCache { get; set; }
        public Encoding Encoding { get; set; }
        public string ContentType { get; set; }

        public object Tag { get; set; }
        public ICredentials Credentials { get; set; }

        
    }
}
