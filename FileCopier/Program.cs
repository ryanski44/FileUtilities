using Fclp;
using RAdams.FileUtilities.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RAdams.FileUtilities.FileCopier
{
    public class Program
    {
        public static void Main(string[] args)
        {
            FluentCommandLineParser<ProgramArgs> parser = new FluentCommandLineParser<ProgramArgs>();

            parser.Setup(x => x.SourceDirectoryPath)
                .As("source")
                .WithDescription("Source Directory")
                .Required();

            parser.Setup(x => x.SourceIndexPath)
                .As("sourceIndex")
                .WithDescription("Source Index File")
                .Required();

            parser.Setup(x => x.TargetDirectoryPath)
                .As("target")
                .WithDescription("Target Directory")
                .Required();

            parser.Setup(x => x.TargetIndexPath)
                .As("targetIndex")
                .WithDescription("Target Index File")
                .Required();

            // sets up the parser to execute the callback when -? or --help is detected
            parser.SetupHelp("?", "help")
                  .Callback(text => Console.WriteLine(text));

            var result = parser.Parse(args);

            if (result.HelpCalled)
            {
                parser.HelpOption.ShowHelp(parser.Options);
                return;
            }

            if (result.HasErrors)
            {
                Console.WriteLine(result.ErrorText);
                parser.HelpOption.ShowHelp(parser.Options);
                return;
            }
            
            DirectoryInfo sourceDirectory = new DirectoryInfo(parser.Object.SourceDirectoryPath);
            FileIndex sourceIndex = new FileIndex(new FileInfo(parser.Object.SourceIndexPath));

            DirectoryInfo targetDirectory = new DirectoryInfo(parser.Object.TargetDirectoryPath);
            FileIndex targetIndex = new FileIndex(new FileInfo(parser.Object.TargetIndexPath));

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
                    FileInfo targetFile = new FileInfo(Path.Combine(targetDirectory.FullName, sourceFileName));
                    if(!targetFile.Directory.Exists)
                    {
                        targetFile.Directory.Create();
                    }
                    File.Copy(Path.Combine(sourceDirectory.FullName, sourceFileName), targetFile.FullName);
                }
            }

            Console.WriteLine("Done");
        }
    }

    public class ProgramArgs
    {
        public string SourceDirectoryPath { get; set; }
        public string SourceIndexPath { get; set; }
        public string TargetDirectoryPath { get; set; }
        public string TargetIndexPath { get; set; }
    }
}
