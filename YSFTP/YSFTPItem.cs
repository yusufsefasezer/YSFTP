using System;
using System.Text.RegularExpressions;

namespace YSFTP
{
    public class YSFTPItem
    {
        #region const
        const string PATTERN = @"^(?<FileType>[-ld])(?<Permissions>(?:[-r][-w][-xs]){3})\s+(?<Owner>\w+)\s+(?<Group>\w+)(?:\s+\w+)?\s+(?<Size>\d+)\s+(?<FileDateTime>\w+\s+\d+(?:\s+\d+(?::\d+)?))\s+(?<Filename>.+?)\s*$";
        #endregion

        #region Properties
        public string Path { get; set; }
        public string Filename { get; set; }
        public long Size { get; set; }
        public ItemType FileType { get; set; }
        public DateTime FileDateTime { get; set; }
        public string Permissions { get; set; }
        public string Owner { get; set; }
        public string Group { get; set; }
        public string OwnerGroup { get { return Owner + "/" + Group; } }
        public string FullPath
        {
            get
            {
                return Path + "/" + Filename;
            }
        }
        public string FileExtension
        {
            get
            {
                return System.IO.Path.GetExtension(Filename);
            }
        }
        public string FileNameWithoutExtension
        {
            get
            {
                return System.IO.Path.GetFileNameWithoutExtension(Filename);
            }
        }
        #endregion

        #region ctor
        public YSFTPItem(string line, string path)
        {
            var split = new Regex(PATTERN).Match(line);

            Filename = split.Groups["Filename"].Value;
            long tmpSize = 0;
            long.TryParse(split.Groups["Size"].Value, out tmpSize);
            Size = tmpSize;
            string tmpFileType = split.Groups["FileType"].Value;
            switch (tmpFileType)
            {
                case "d":
                    FileType = ItemType.Directory;
                    break;
                case "l":
                    FileType = ItemType.Link;
                    break;
                case "-":
                default:
                    FileType = ItemType.File;
                    break;
            }
            var tmpDateTime = DateTime.Now;
            DateTime.TryParse(split.Groups["FileDateTime"].Value, out tmpDateTime);
            FileDateTime = tmpDateTime;
            Permissions = split.Groups["Permissions"].Value;
            Owner = split.Groups["Owner"].Value;
            Owner = split.Groups["Group"].Value;
            Path = path;
        }
    }
    #endregion

    #region ItemType
    public enum ItemType
    {
        File,
        Directory,
        Link
    }
    #endregion
}