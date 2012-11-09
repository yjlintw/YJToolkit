﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using Windows.Storage;

namespace YJToolkit.YJToolkitCSharp.Networking
{
    public class AuthenticatedUri : Uri
    {
        public AuthenticatedUri(string uriString, string username, string password)
            : base(uriString)
        {
            UserName = username;
            Password = password;
        }

        public AuthenticatedUri(string uriString, UriKind uriKind, string username, string password)
            : base(uriString, uriKind)
        {
            UserName = username;
            Password = password;
        }

        public AuthenticatedUri(Uri baseUri, string relativeUri, string username, string password)
            : base(baseUri, relativeUri)
        {
            UserName = username;
            Password = password;
        }

        public AuthenticatedUri(Uri baseUri, Uri relativeUri, string username, string password)
            : base(baseUri, relativeUri)
        {
            UserName = username;
            Password = password;
        }


        public string UserName { get; set; }
        public string Password { get; set; }

        public ICredentials Credentials
        {
            get
            {
                return UserName == null ? null : new NetworkCredential(UserName, Password);
            }
        }

    }

    public static class Http
    {
        private static readonly List<HttpResponse> pendingRequests = new List<HttpResponse>();
        public static void AbortAllRequests()
        {
            lock (pendingRequests)
            {
                foreach (var r in pendingRequests)
                {
                    r.Abort();
                }
            }
        }

        public static void AbortRequests(Func<HttpResponse, bool> abortPredicate)
        {
            lock (pendingRequests)
            {
                foreach (var r in pendingRequests.Where(abortPredicate))
                {
                    r.Abort();
                }
            }
        }

        private static HttpWebRequest CreateRequest(HttpGetRequest req)
        {
            var queryString = GetQueryString(req.Query);

            HttpWebRequest request = null;
            if (string.IsNullOrEmpty(queryString))
            {
                request = (HttpWebRequest)WebRequest.Create(req.Uri.AbsoluteUri);
            }
            else if (req.Uri.AbsoluteUri.Contains(":"))
            {
                request = (HttpWebRequest)WebRequest.Create(req.Uri.AbsoluteUri + "&" + queryString);
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(req.Uri.AbsoluteUri + "?" + queryString);
            }

            return request;
        }

        public static Task<HttpResponse> GetAsync(string url)
        {
            var task = new TaskCompletionSource<HttpResponse>();

            Get(url, result =>
            {
                if (result.Successful)
                {
                    task.SetResult(result);
                }
                else if (result.Canceled)
                {
                    task.SetCanceled();
                }
                else
                {
                    task.SetException(result.Exception);
                }
            });
            return task.Task;
        }

        public static Task<HttpResponse> GetAsync(HttpGetRequest request)
        {
            var task = new TaskCompletionSource<HttpResponse>();
            Get(request, result =>
            {
                if (result.Successful)
                {
                    task.SetResult(result);
                }
                else if (result.Canceled)
                {
                    task.SetCanceled();
                }
                else
                {
                    task.SetException(result.Exception);
                }
            });
            return task.Task;
        }

        public static Task<HttpResponse> PostAsync(string url)
        {
            var task = new TaskCompletionSource<HttpResponse>();
            Post(url, result =>
            {
                if (result.Successful)
                    task.SetResult(result);
                else if (result.Canceled)
                    task.SetCanceled();
                else
                    task.SetException(result.Exception);
            });
            return task.Task;
        }

        public static Task<HttpResponse> PostAsync(HttpPostRequest request)
        {
            var task = new TaskCompletionSource<HttpResponse>();
            Post(request, result =>
            {
                if (result.Successful)
                    task.SetResult(result);
                else if (result.Canceled)
                    task.SetCanceled();
                else
                    task.SetException(result.Exception);
            });
            return task.Task;
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static HttpResponse Get(string uri, Action<HttpResponse> action)
        {
            return Get(new HttpGetRequest(uri), action);
        }

        public static HttpResponse Get(HttpGetRequest req, Action<HttpResponse> action)
        {
            var response = new HttpResponse(req);
            try
            {
                if (!req.UseCache)
                {
                    req.Query["__dcachetime"] = DateTime.Now.Ticks.ToString();
                }

                var request = CreateRequest(req);

                if (req.Credentials != null)
                {
                    request.Credentials = req.Credentials;
                }
                response.WebRequest = request;

                if (req.Cookies.Count > 0)
                {
                    request.CookieContainer = new CookieContainer();
                    foreach (var c in req.Cookies)
                    {
                        request.CookieContainer.Add(request.RequestUri, c);
                    }
                }

                request.Method = "GET";
                if (req.ContentType != null)
                {
                    request.ContentType = req.ContentType;
                }

                if (req.Headers.Count > 0)
                {
                    foreach (var item in req.Headers)
                    {
                        request.Headers[item.Key] = item.Value;
                    }
                }

                response.CreateTimeoutTimer(request);
                request.BeginGetRequestStream(r => ProcessResponse(r, request, response, action), request);
            }
            catch (Exception ex)
            {
                response.Exception = ex;
                if (action != null)
                {
                    action(response);
                }
            }

            lock (pendingRequests)
            {
                pendingRequests.Add(response);
            }

            return response;
        }

        /// <summary>
        /// Post
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static HttpResponse Post(string uri, Action<HttpResponse> action)
        {
            return Post(new HttpPostRequest(uri), action);
        }

        public static HttpResponse Post(HttpPostRequest req, Action<HttpResponse> action)
        {
            return Post(req, action, "POST");
        }

        public static HttpResponse Put(string uri, Action<HttpResponse> action)
        {
            return Post(new HttpPostRequest(uri), action, "PUT");
        }

        public static HttpResponse Put(HttpPostRequest req, Action<HttpResponse> action)
        {
            return Post(req, action, "PUT");
        }

        public static HttpResponse Delete(string uri, Action<HttpResponse> action)
        {
            return Post(new HttpPostRequest(uri), action, "DELETE");
        }

        public static HttpResponse Delete(HttpPostRequest req, Action<HttpResponse> action)
        {
            return Post(req, action, "DELETE");
        }

        private static HttpResponse Post(HttpPostRequest req, Action<HttpResponse> action, string method)
        {
            var response = new HttpResponse(req);

            try
            {
                var boundary = "";
                var request = CreateRequest(req);

                if (req.Credentials != null)
                {
                    request.Credentials = req.Credentials;
                }

                response.WebRequest = request;

                if (req.Files.Count == 0)
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                }
                else
                {
                    boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
                    request.ContentType = "multipart/form-data; boundary=" + boundary;
                }

                if (req.Cookies.Count > 0)
                {
                    request.CookieContainer = new CookieContainer();
                    foreach (var c in req.Cookies)
                    {
                        request.CookieContainer.Add(request.RequestUri, c);
                    }
                }

                request.Method = method;
                if (req.ContentType != null)
                {
                    request.ContentType = req.ContentType;
                }

                if (req.Headers.Count > 0)
                {
                    foreach (var item in req.Headers)
                    {
                        request.Headers[item.Key] = item.Value;
                    }
                }

                response.CreateTimeoutTimer(request);
                request.BeginGetRequestStream(delegate(IAsyncResult ar1)
                {
                    try
                    {
                        using (var stream = request.EndGetRequestStream(ar1))
                        {
                            if (req.Files.Count > 0)
                            {
                                WritePostData(stream, boundary, req);
                            }
                            else
                            {
                                WritePostData(stream, req);
                            }
                        }
                        request.BeginGetResponse(r => ProcessResponse(r, request, response, action), request);
                    }
                    catch (Exception ex)
                    {
                        response.Exception = ex;
                        if (action != null)
                            action(response);
                    }
                }, request);
            }
            catch (System.Exception ex)
            {
                response.Exception = ex;
                if (action != null)
                {
                    action(response);
                }
            }

            lock (pendingRequests)
            {
                pendingRequests.Add(response);
            }

            return response;
        }

        private static void WritePostData(Stream stream, HttpPostRequest request)
        {
            var bytes = request.RawData ?? request.Encoding.GetBytes(GetQueryString(request.Data));
            stream.Write(bytes, 0, bytes.Length);
        }

        private static void WritePostData(Stream stream, string boundary, HttpPostRequest request)
        {
            var boundarybytes = request.Encoding.GetBytes("\r\n--" + boundary + "\r\n");
            var formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";
            if (request.RawData != null)
            {
                throw new Exception("RawData not allowed if uploading files");
            }

            foreach (var tuple in request.Data)
            {
                stream.Write(boundarybytes, 0, boundarybytes.Length);

                var formitem = string.Format(formdataTemplate, tuple.Key, tuple.Value);
                var formitembytes = request.Encoding.GetBytes(formitem);
                stream.Write(formitembytes, 0, formitembytes.Length);
            }

            const string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type:{2}\r\n\r\n";
            foreach (var file in request.Files)
            {
                stream.Write(boundarybytes, 0, boundarybytes.Length);

                var header = string.Format(headerTemplate, file.Name, file.Filename, file.ContentType ?? "application/octet-stream");
                var headerbytes = request.Encoding.GetBytes(header);

                stream.Write(headerbytes, 0, headerbytes.Length);

                var fileStream = file.Stream;

                if (fileStream == null)
                {
                    var f = StorageFile.GetFileFromPathAsync(file.Path);
                    var t = f.AsTask();
                    t.RunSynchronously();

                    var f2 = f.GetResults().OpenReadAsync();
                    var t2 = f2.AsTask();
                    t2.RunSynchronously();

                    fileStream = f2.GetResults().AsStreamForRead();
                }

                try
                {
                    var buffer = new byte[1024];
                    var bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        stream.Write(buffer, 0, bytesRead);
                    }
                }
                finally
                {
                    if (file.CloseStream)
                        fileStream.Dispose();
                }

                boundarybytes = request.Encoding.GetBytes("\r\n--" + boundary + "--\r\n");
                stream.Write(boundarybytes, 0, boundarybytes.Length);
            }
        }

        private static void ProcessResponse(IAsyncResult asyncResult, WebRequest request, HttpResponse resp, Action<HttpResponse> action)
        {
            lock (pendingRequests)
            {
                if (pendingRequests.Contains(resp))
                {
                    pendingRequests.Remove(resp);
                }
            }

            try
            {
                var response = request.EndGetResponse(asyncResult);
                resp.IsConnected = true;
                var origResponse = response;

                using (response)
                {
                    if (resp.Request.ResponseAsStream)
                    {
                        resp.ResponseStream = response.GetResponseStream();
                    }
                    else
                    {
                        resp.RawResponse = response.GetResponseStream().ReadToEnd();
                    }
                    if (origResponse.Headers.AllKeys.Contains("Set-Cookie"))
                    {
                        var cookies = origResponse.Headers["Set-Cookie"];
                        var index = cookies.IndexOf(';');
                        if (index != -1)
                        {
                            foreach (var c in ParseQueryString(cookies.Substring(0, index)))
                            {
                                resp.Cookies.Add(new Cookie(c.Key, c.Value));
                            }
                        }
                    }

                    foreach (var key in origResponse.Headers.AllKeys)
                    {
                        var value = origResponse.Headers[key];
                        resp.Headers.Add(key, value);
                    }   
                }
            }
            catch (System.Exception ex)
            {
            	var we = ex as WebException;
                if (we != null)
                {
                    var temp = we.Response as HttpWebResponse;
                    if (temp != null)
                    {
                        resp.HttpStatusCode = temp.StatusCode;
                    }
                }

                if (resp.ResponseStream != null)
                {
                    resp.ResponseStream.Dispose();
                    resp.ResponseStream = null;
                }

                resp.Exception = ex;
                if (action != null)
                {
                    action(resp);
                }
                return;
            }
            if (action != null)
            {
                action(resp);
            }
        }

        private static string GetQueryString(Dictionary<string, string> query)
        {
            var queryString = "";
            foreach (var p in query)
                queryString += Uri.EscapeDataString(p.Key) + "=" + (p.Value == null ? "" : Uri.EscapeDataString(p.Value)) + "&";
            return queryString.Trim('&');
        }

        public static byte[] ReadToEnd(this Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, read);
                return ms.ToArray();
            }
        }

        public static Dictionary<string, string> ParseQueryString(string queryString)
        {
            var dict = new Dictionary<string, string>();
            foreach (var s in queryString.Split('&'))
            {
                var index = s.IndexOf('=');
                if (index != -1 && index + 1 < s.Length)
                {
                    var key = s.Substring(0, index);
                    var value = Uri.UnescapeDataString(s.Substring(index + 1));
                    dict.Add(key, value);
                }
            }
            return dict;
        }
    }
}
