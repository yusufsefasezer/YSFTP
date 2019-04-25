using System.Collections.Generic;

namespace YSFTP
{
    public class YSFTPDirectory : List<YSFTPItem>
    {
        #region ctor
        public YSFTPDirectory() { }

        public YSFTPDirectory(string[] lines, string path)
        {
            foreach (string line in lines)
            {
                if (line != string.Empty)
                {
                    Add(new YSFTPItem(line, path));
                }
            }
        }
        #endregion
    }
}