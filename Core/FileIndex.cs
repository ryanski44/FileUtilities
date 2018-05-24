using RAdams.FileUtilities.Core.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RAdams.FileUtilities.Core
{
    public class FileIndex
    {
        private Dictionary<string, FileIndexEntry> entries;

        public FileIndex()
        {
            this.entries = new Dictionary<string, FileIndexEntry>();
        }

        public FileIndex(DirectoryInfo directory, IndexerOptions options) : this()
        {
            Parse("", directory, options);
        }

        private void Parse(string prefix, DirectoryInfo directory, IndexerOptions options)
        {
            foreach (var file in directory.GetFiles())
            {
                FileIndexEntry entry = new FileIndexEntry();
                entry.FileName = prefix + file.Name;
                entry.FileLength = file.Length;

                if (options.HasFlag(IndexerOptions.IncludeHash))
                {
                    entry.SHA1Hash = file.GenerateSHA1();
                }

                entries[entry.FileName] = entry;
            }
            if (options.HasFlag(IndexerOptions.Recursive))
            {
                foreach(var subDir in directory.GetDirectories())
                {
                    Parse(prefix + subDir.Name + "\\", subDir, options);
                }
            }
        }

        public FileIndex(FileInfo indexFile ) : this()
        {
            using (StreamReader sr = new StreamReader(new FileStream(indexFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                string line = null;
                while((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split('\t');
                    if(parts.Length == 3)
                    {
                        long length;
                        if(Int64.TryParse(parts[0], out length))
                        {
                            FileIndexEntry entry = new FileIndexEntry();
                            entry.FileLength = length;
                            entry.FileName = parts[1];
                            if(!String.IsNullOrWhiteSpace(parts[2]))
                            {
                                entry.SHA1Hash = Convert.FromBase64String(parts[2]);
                            }

                            entries[entry.FileName] = entry;
                        }
                    }
                }
            }
        }

        public void Write(string outputFilePath)
        {
            using (var writer = new StreamWriter(new FileStream(outputFilePath, FileMode.Create, FileAccess.Write)))
            {
                foreach (string fileName in entries.Keys.OrderBy(str => str))
                {
                    var entry = entries[fileName];
                    writer.WriteLine(String.Join("\t", entry.FileLength, entry.FileName, entry.SHA1HashBase64));
                }
            }
        }

        public IDictionary<string, FileIndexEntry> Files
        {
            get
            {
                return entries;
            }
        }

        [Flags]
        public enum IndexerOptions
        {
            None = 0,
            Recursive = 1,
            IncludeHash = 2
        }
    }
}
