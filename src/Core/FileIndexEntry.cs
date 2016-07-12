using System;

namespace RAdams.FileUtilities.Core
{
    public class FileIndexEntry
    {
        public string FileName;
        public long FileLength;
        public byte[] SHA1Hash;

        public string SHA1HashBase64
        {
            get
            {
                if(SHA1Hash != null)
                {
                    return Convert.ToBase64String(SHA1Hash);
                }
                else
                {
                    return String.Empty;
                }
            }
        }
    }
}