using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Wikiled.News.Monitoring.Retriever
{
    public class SimpleDataRetriever : IDataRetriever
    {
        private const string defaultEncoding = "utf-8";

        private readonly HttpState httpStateRequest = new HttpState();

        private readonly ILogger logger;

        private readonly IConcurentManager manager;

        private Stream readStream;

        private HttpWebResponse responseReading;

        public SimpleDataRetriever(ILoggerFactory loggerFactory, IConcurentManager manager, Uri uri)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            logger = loggerFactory.CreateLogger<SimpleDataRetriever>();
            Timeout = 2 * 60 * 1000;
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));
            DocumentUri = uri ?? throw new ArgumentNullException(nameof(uri));
        }

        public CookieCollection AllCookies
        {
            get => httpStateRequest.CookieContainer.GetCookies(DocumentUri);
            set
            {
                if (value != null)
                {
                    httpStateRequest.CookieContainer.Add(value);
                }
            }
        }

        public bool AllowGlobalRedirection { get; set; }

        public ICredentials Credentials { get; set; }

        public string Data { get; private set; }

        public Uri DocumentUri { get; }

        public IPAddress Ip { get; private set; }

        public bool IsDispossed { get; private set; }

        public string Referer { get; set; }

        public HttpWebRequest Request => httpStateRequest.HttpRequest;

        public Uri ResponseUri { get; private set; }

        public bool Success { get; private set; }

        public int Timeout { get; set; }

        public virtual void Dispose()
        {
            IsDispossed = true;
            responseReading?.Close();
        }

        public async Task PostData(Tuple<string, string>[] parameters, bool prepareCall = true)
        {
            string postData = string.Empty;
            foreach (var parameter in parameters)
            {
                if (postData.Length > 0)
                {
                    postData += "&";
                }

                postData += $"{parameter.Item1}={parameter.Item2}";
            }

            await PostData(postData, prepareCall).ConfigureAwait(false);
        }

        public async Task PostData(string postData, bool prepareCall = true)
        {
            if (prepareCall)
            {
                PrepareCall(HttpProtocol.POST);
            }

            httpStateRequest.HttpRequest.ContentType = "application/x-www-form-urlencoded";
            httpStateRequest.HttpRequest.Headers.Add("X-Prototype-Version", "1.7.1");
            httpStateRequest.HttpRequest.Headers.Add("Origin", "https://seekingalpha.com");
            httpStateRequest.HttpRequest.Referer = "https://seekingalpha.com/";
            httpStateRequest.HttpRequest.KeepAlive = true;

            /*             
Origin: https://seekingalpha.com
Content-type: application/x-www-form-urlencoded; charset=UTF-8
Referer: https://seekingalpha.com/
Accept-Encoding: gzip, deflate, br
Accept-Language: en-GB,en;q=0.9,en-US;q=0.8,lt;q=0.7,ru;q=0.6
             */
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(postData);
            using (Stream newStream = httpStateRequest.HttpRequest.GetRequestStream())
            {
                // Send the data.
                newStream.Write(data, 0, data.Length);
                responseReading = (HttpWebResponse)httpStateRequest.HttpRequest.GetResponse();
                await StartReading().ConfigureAwait(false);
                newStream.Close();
            }
        }

        private void PrepareCall(HttpProtocol protocol = HttpProtocol.GET)
        {
            logger.LogDebug("Download: {0}", DocumentUri);
            CreateRequest(protocol);
            //httpStateRequest.HttpRequest.Pipelined = false;
            //httpStateRequest.HttpRequest.ServicePoint.ConnectionLimit = concurentManager.MaxProcessors;

            Ip = manager.StartDownloading(DocumentUri);
            httpStateRequest.HttpRequest.ServicePoint.BindIPEndPointDelegate =
                (servicePoint, endPoint, target) =>
                {
                    logger.LogInformation("BindIp: {1} - {0}", DocumentUri, Ip);
                    return new IPEndPoint(Ip, 0);
                };
        }

        public async Task ReceiveData(Stream stream = null)
        {
            readStream = stream;
            PrepareCall();
            responseReading = (HttpWebResponse)await httpStateRequest.HttpRequest.GetResponseAsync().ConfigureAwait(false);
            await StartReading().ConfigureAwait(false);
        }

        private static bool ValidateRemoteCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors policyErrors)
        {
            return true;
        }

        private void CreateRequest(HttpProtocol protocol)
        {
            logger.LogDebug("CreateRequest: {0} ({1})", DocumentUri, protocol);

            ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;

            // Open the requested URL            
            httpStateRequest.HttpRequest = (HttpWebRequest)WebRequest.Create(DocumentUri.AbsoluteUri);
            httpStateRequest.HttpRequest.Method = protocol.ToString();
            httpStateRequest.HttpRequest.AllowAutoRedirect = true;
            httpStateRequest.HttpRequest.Credentials = Credentials;
            httpStateRequest.HttpRequest.MaximumAutomaticRedirections = 10;
            httpStateRequest.HttpRequest.Referer = Referer;
            httpStateRequest.HttpRequest.Accept = "text/html, application/xhtml+xml, */*";
            httpStateRequest.HttpRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
            httpStateRequest.HttpRequest.KeepAlive = false;
            httpStateRequest.HttpRequest.Timeout = Timeout;
            httpStateRequest.HttpRequest.CookieContainer = httpStateRequest.CookieContainer;

            httpStateRequest.HttpRequest.Accept = "text/javascript, text/html, application/xml, text/xml, */*";
            httpStateRequest.HttpRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
            //POST https://seekingalpha.com/authentication/login HTTP/1.1
            //Host: seekingalpha.com
            //Connection: keep - alive
            //Content - Length: 130
            //Accept: text/javascript, text/html, application/xml, text/xml, */*
            //X-Prototype-Version: 1.7.1
            //Origin: https://seekingalpha.com
            //User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36
            //Content-type: application/x-www-form-urlencoded; charset=UTF-8
            //Referer: https://seekingalpha.com/
            //Accept-Encoding: gzip, deflate, br
            //Accept-Language: en-GB,en;q=0.9,en-US;q=0.8,lt;q=0.7,ru;q=0.6
            //Cookie: _pxvid=0cbb7120-5aa3-11e8-88e2-ed2e40a2cd2c; hubspotutk=e4a63cf601154af43e8b56ff869b37cc; pxvid=0cbb7120-5aa3-11e8-88e2-ed2e40a2cd2c; __utmc=150447540; __hssrc=1; _igt=cf938032-df6c-4fa1-f092-e3ed61953383; __utmz=150447540.1539017799.88.6.utmcsr=google|utmccn=(organic)|utmcmd=organic|utmctr=(not%20provided); __utma=150447540.1207112929.1526651698.1539080301.1539085222.90; __hstc=234155329.e4a63cf601154af43e8b56ff869b37cc.1526651698761.1539080302906.1539085222812.89; _ig=e6733454-7e8d-415e-aafa-86b6cf212df2; _sapi_session_id=eyJzZXNzaW9uX2lkIjoiNjk3NDY3ZGY3ZDMwMWFmOWRkYzQ2ZjAwYzhjMjc2NTUiLCJvbW5pYXV0aC5zdGF0ZSI6Ijg3MGVlMTE5NjAwNjc1MjExMzQ3NzlhYTkyMzhlZWJkOTBkNzBmMTEyOGYyNjAxNyIsInNldHRpbmdzX3JlZmVycmVyIjoiaHR0cHM6Ly9zZWVraW5nYWxwaGEuY29tL2FjY291bnQvc3Vic2NyaXB0aW9uc19zZXR0aW5ncyIsIl9jc3JmX3Rva2VuIjoicElTdDF4Z09NcVl0RHFaQWVmZWJCeE1WNEtFRDhEY1hMYTU1dlZmU1RKRT0ifQ%3D%3D--57dded3ea3efd91682e7934f110f9266aab1976f; h_px=1; machine_cookie=0539983042310; has_paid_subscription=true; __utmt=1; _px=e2dDVovAitYCtH9op85ZYIPqZv9SSypj4HI8BbroEjtcOPX4pOxSazcPXmxcLB4wnDiEBq7DdvJbZnhO/XutrA==:1000:EJgNMsHVJCbw96Sm3vToyXp7VmSUpmum/ekeBcYWqlDN/MYf+BRtPCBdOkaqH5tRVJWYpVPaKNX4oYLMwet4EyL89XP0Q4hTCAJayKYYY8G8wuMDjlSP282y8N2u9fVBZddPEn7yBtO/zDT8oK2EOXPyc+CNS3BTP1mzJM5PeqlE36vB+2Fi4EpZUCaWOuRf5w8xNQPwGk86fzLCoge0VxjPIkOs49/+D+VrqFIBCCvOmuwUdKF98rb/iEZFXzKGziiuXl70hgjS8Wl81se68g==; _px2=eyJ1IjoiNmQyMzhmNTAtY2JiYi0xMWU4LTkzNzctNzE1NzE0MjdiMmRkIiwidiI6IjBjYmI3MTIwLTVhYTMtMTFlOC04OGUyLWVkMmU0MGEyY2QyYyIsInQiOjE1MzkwODcxNDk4NjQsImgiOiIzZjQyZmJlN2FhNTdmNmRmNGQ1YWIzMjk2OWZmNDE1M2JmOWUyMzA2NjA3NGFjZDI2ZDc1YjhlNDNhOTY0ZTAzIn0=; __hssc=234155329.31.1539085222812; __utmb=150447540.60.9.1539086678103

            //id=headtabs_login&activity=footer_login&function=FooterBar.Login&user%5Bemail%5D=keistokas%40gmail.com&user%5Bpassword%5D=Kla1peda
        }

        private async Task ReadData()
        {
            HttpWebResponse webResponse = httpStateRequest.HttpResponse;
            ResponseUri = webResponse.ResponseUri;
            if (!AllowGlobalRedirection &&
                (string.Compare(
                     DocumentUri.Host,
                     httpStateRequest.ResponseHost,
                     StringComparison.OrdinalIgnoreCase) != 0))
            {
                logger.LogWarning("URI from another host is not supported", httpStateRequest.ResponseHost);
                return;
            }

            foreach (Cookie cookie in webResponse.Cookies)
            {
                AllCookies.Add(cookie);
            }

            string encoding = string.Empty;
            if (!string.IsNullOrEmpty(webResponse.ContentEncoding))
            {
                encoding = webResponse.ContentEncoding;
            }
            else if (string.IsNullOrEmpty(encoding))
            {
                encoding = defaultEncoding; // default
            }

            if (readStream != null)
            {
                await webResponse.GetResponseStream().CopyToAsync(readStream).ConfigureAwait(false);
            }

            using (var stream = new StreamReader(webResponse.GetResponseStream(), Encoding.GetEncoding(encoding)))
            {
                Data = await stream.ReadToEndAsync().ConfigureAwait(false);
            }
        }

        private async Task ReadResponse()
        {
            if (httpStateRequest.HttpResponse == null)
            {
                return;
            }

            if (!AllowGlobalRedirection &&
                (string.Compare(
                     httpStateRequest.ResponseHost,
                     httpStateRequest.RequestHost,
                     StringComparison.OrdinalIgnoreCase) != 0))
            {
                logger.LogWarning(
                    "{0} redirected to another host {1} and that is not supported",
                    httpStateRequest.HttpRequest.RequestUri,
                    httpStateRequest.HttpResponse.ResponseUri);
                Success = false;
                return;
            }

            await ReadData().ConfigureAwait(false);
            httpStateRequest.HttpResponse.Close();
            Success = true;
        }

        private async Task StartReading()
        {
            try
            {
                logger.LogDebug("StartReading: {0}", DocumentUri);
                // End of the Asynchronous request.
                httpStateRequest.HttpResponse = responseReading;
                httpStateRequest.HttpResponse.Cookies = AllCookies;
                await ReadResponse();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Page processing failed: {0} on {1}", httpStateRequest.HttpRequest.RequestUri, Ip);
            }
            finally
            {
                logger.LogError("Page processing completed: {0} on {1}", httpStateRequest.HttpRequest.RequestUri, Ip);
                manager.FinishedDownloading(DocumentUri, Ip);

                httpStateRequest.HttpResponse?.Close();
                ServicePointManager.ServerCertificateValidationCallback -= ValidateRemoteCertificate;
            }
        }
    }
}
