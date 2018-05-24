using RAdams.FileUtilities.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fclp;

namespace RAdams.FileUtilities.FileIndexer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            FluentCommandLineParser<ProgramArgs> parser = new FluentCommandLineParser<ProgramArgs>();

            parser.Setup(x => x.DirectoryPath)
                .As('d', "directory")
                .WithDescription("Directory to Index")
                .Required();

            parser.Setup(x => x.OutputFilePath)
                .As('o', "output")
                .WithDescription("The output index file path")
                .Required();

            parser.Setup(x => x.IncludeHash)
                .As('h', "hash")
                .WithDescription("Include the hash?");

            parser.Setup(x => x.Recursive)
                .As('r', "recursive")
                .WithDescription("Include subfolders recursivly");

            // sets up the parser to execute the callback when -? or --help is detected
            parser.SetupHelp("?", "help")
                  .Callback(text => Console.WriteLine(text));

            var result = parser.Parse(args);

            if(result.HelpCalled)
            {
                parser.HelpOption.ShowHelp(parser.Options);
                return;
            }

            if(result.HasErrors)
            {
                Console.WriteLine(result.ErrorText);
                parser.HelpOption.ShowHelp(parser.Options);
                return;
            }
            
            DirectoryInfo directory = new DirectoryInfo(parser.Object.DirectoryPath);
            FileIndex.IndexerOptions options = FileIndex.IndexerOptions.None;
            if(parser.Object.IncludeHash)
            {
                options |= FileIndex.IndexerOptions.IncludeHash;
            }
            if(parser.Object.Recursive)
            {
                options |= FileIndex.IndexerOptions.Recursive;
            }
            FileIndex index = new FileIndex(directory, options);
            index.Write(parser.Object.OutputFilePath);
        }
    }

    public class ProgramArgs
    {
        public string DirectoryPath { get; set; }
        public string OutputFilePath { get; set; }
        public bool IncludeHash { get; set; }
        public bool Recursive { get; set; }
    }
}
