using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YSFTP
{
    class YSFTPClient
    {
        #region Properties
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 21;
        public string UserName { get; set; } = "anonymous";
        public string UserPass { get; set; } = string.Empty;
        public string Path { get; set; } = "/";
        public FtpWebResponse Response { get; set; }
        #endregion

        #region ctor
        public YSFTPClient() { }

        public YSFTPClient(string host)
        {
            Host = host;
        }

        public YSFTPClient(YSFTPUser user)
        {
            Host = user.Host;
            Port = user.Port;
            UserName = user.UserName;
            UserPass = user.UserPass;
        }

        public YSFTPClient(string host, int port, string username, string userpass)
        {
            Host = host;
            Port = port;
            UserName = username;
            UserPass = userpass;
        }
        #endregion

        #region CreateRequest
        private FtpWebRequest CreateRequest(string path)
        {
            var uri = CreateFTPURI(path);
            var ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(uri);
            ftpWebRequest.Credentials = new NetworkCredential(UserName, UserPass);
            ftpWebRequest.UseBinary = true;
            ftpWebRequest.UsePassive = true;
            ftpWebRequest.KeepAlive = true;
            return ftpWebRequest;
        }
        #endregion

        #region GetResponseStream
        private Stream GetResponseStream(FtpWebRequest request)
        {
            var ftpWebResponse = (FtpWebResponse)request.GetResponse();
            return ftpWebResponse.GetResponseStream();
        }
        #endregion

        #region GetResponseStreamAsString
        private string GetResponseAsString(FtpWebRequest request)
        {
            var responseStream = GetResponseStream(request);
            using (var streamReader = new StreamReader(responseStream))
            {
                return streamReader.ReadToEnd();
            }
        }
        #endregion

        #region GetRequestStream
        private Stream GetRequestStream(FtpWebRequest request)
        {
            return request.GetRequestStream();
        }
        #endregion


        #region ListDirectory
        public YSFTPDirectory ListDirectory(string directoryPath)
        {
            Path = directoryPath;
            var request = CreateRequest(Path);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            string result = GetResponseAsString(request);
            return new YSFTPDirectory(result.Split('\n'), Path);
        }

        public YSFTPDirectory ListDirectory()
        {
            var request = CreateRequest(Path);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            string result = GetResponseAsString(request);
            return new YSFTPDirectory(result.Split('\n'), Path);
        }
        #endregion

        #region Delete
        public void Delete(string path)
        {
            var request = CreateRequest(path);
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            Response = (FtpWebResponse)request.GetResponse();
        }
        #endregion

        #region Utility methods
        private Uri CreateFTPURI(string path)
        {
            var uriBuilder = new UriBuilder()
            {
                Scheme = Uri.UriSchemeFtp,
                Host = Host,
                Port = Port,
                UserName = UserName,
                Password = UserPass,
                Path = path
            };
            return uriBuilder.Uri;
        }
        #endregion
    }
}
