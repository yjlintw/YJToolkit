using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace YJToolkit.YJToolkitCSharp.Networking
{
    public class HttpResponse
    {
        internal HttpResponse()
        {
            Cookies = new List<Cookie>();
            Headers = new Dictionary<string, string>();
        }

        public HttpResponse(IHttpRequest request)
            : this()
        {
            Request = request;
        }

        ///
        /// Getter/Setter
        /// 

        public bool Processed
        {
            get
            {
                return Canceled || Successful || HasException;
            }
        }

        public IHttpRequest Request { get; internal set; }
        internal HttpWebRequest WebRequest { get; set; }

        public bool IsConnected { get; internal set; }

        public bool IsPending
        {
            get
            {
                return !HasException && !Canceled && !Successful;
            }
        }

        public void Abort()
        {
            if (Request != null && !Successful)
                WebRequest.Abort();
        }

        private Exception exception;
        public Exception Exception
        {
            get { return exception; }
            set
            {
                if (exception != null && exception is TimeoutException)
                    return; // already set

                if (value is WebException && ((WebException)value).Status == WebExceptionStatus.RequestCanceled)
                {
                    exception = null;
                    Canceled = true;
                }
                else
                {
                    exception = value;
                    Canceled = false;
                }
            }
        }

        public bool Canceled { get; private set; }
        public bool Successful
        {
            get
            {
                return !Canceled && Exception == null && (Response != null || ResponseStream != null);
            }
        }
        public bool HasException
        {
            get
            {
                return Exception != null;
            }
        }

        public string Response
        {
            get
            {
                return RawResponse == null ? null : Request.Encoding.GetString(RawResponse, 0, RawResponse.Length);
            }
        }

        public byte[] RawResponse { get; internal set; }
        public Stream ResponseStream { get; internal set; }

        public List<Cookie> Cookies { get; private set; }
        public Dictionary<string, string> Headers { get; private set; }

        private HttpStatusCode code = HttpStatusCode.OK;
        public HttpStatusCode HttpStatusCode
        {
            get { return code; }
            internal set { code = value; }
        }

        internal void CreateTimeoutTimer(HttpWebRequest request)
        {
            if (Request.ConnectionTimeout > 0)
            {
                request.ContinueTimeout = Request.ConnectionTimeout;
            }
        }
    }
}
