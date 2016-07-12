using RAdams.FileUtilities.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RAdams.FileUtilities.FileIndexer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: FileIndexer {directory to index} {output file path for index file}");
                Console.WriteLine("Example: FileIndexer \"c:\\some directory\" \"some directory.index\"");
                return;
            }
            
            DirectoryInfo directory = new DirectoryInfo(args[0]);
            FileIndex index = new FileIndex(directory, FileIndex.IndexerOptions.None);
            index.Write(args[1]);
        }
    }
}
