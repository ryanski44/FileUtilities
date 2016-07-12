using RAdams.FileUtilities.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileCopier
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if(args.Length != 4)
            {
                Console.WriteLine("Usage: FileCopier {source directory} {source directory index file} {target directory} {target directory index file}");
                return;
            }
            DirectoryInfo sourceDirectory = new DirectoryInfo(args[0]);
            FileIndex sourceIndex = new FileIndex(new FileInfo(args[1]));

            DirectoryInfo targetDirectory = new DirectoryInfo(args[2]);
            FileIndex targetIndex = new FileIndex(new FileInfo(args[3]));

            foreach(var sourceFileName in sourceIndex.Files.Keys)
            {
                bool copy = false;
                if(!targetIndex.Files.ContainsKey(sourceFileName))
                {
                    copy = true;
                }
                else
                {
                    var sourceFile = sourceIndex.Files[sourceFileName];
                    var targetFile = targetIndex.Files[sourceFileName];
                    if(sourceFile.SHA1Hash != null && targetFile.SHA1Hash != null)
                    {
                        if(!sourceFile.SHA1Hash.SequenceEqual(targetFile.SHA1Hash))
                        {
                            copy = true;
                        }
                    }
                    else if(sourceFile.FileLength != targetFile.FileLength)
                    {
                        copy = true;
                    }
                }
                if(copy)
                {
                    Console.WriteLine("Copying " + sourceFileName);
                    File.Copy(Path.Combine(sourceDirectory.FullName, sourceFileName), Path.Combine(targetDirectory.FullName, sourceFileName));
                }
            }

            Console.WriteLine("Done");
        }
    }
}
