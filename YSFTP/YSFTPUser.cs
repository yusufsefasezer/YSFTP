using System;

namespace YSFTP
{
    [Serializable]
    public class YSFTPUser
    {
        public string ServerName { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string UserPass { get; set; }
    }
}