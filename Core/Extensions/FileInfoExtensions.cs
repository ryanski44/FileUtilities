using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace RAdams.FileUtilities.Core.Extensions
{
    public static class FileInfoExtensions
    {
        public static byte[] GenerateSHA1(this FileInfo fi)
        {
            using (System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create())
            {
                using (FileStream fs = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return sha1.ComputeHash(fs);
                }
            }
        }
    }
}
