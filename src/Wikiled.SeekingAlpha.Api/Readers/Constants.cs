using System;
using System.Net;

namespace Wikiled.SeekingAlpha.Api.Readers
{
    public static class Constants
    {
        public static Action<HttpWebRequest> Ajax =>
            request =>
            {
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("X-Prototype-Version", "1.7.1");
                request.Headers.Add("Origin", "https://seekingalpha.com");
                request.Referer = "https://seekingalpha.com/";
                request.KeepAlive = true;
                request.Accept = "text/javascript, text/html, application/xml, text/xml, */*";
                request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            };
    }
}
