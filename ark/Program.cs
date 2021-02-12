using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using CommandLine;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Common.Zip;
using SharpCompress.Readers;
using SharpCompress.Writers;

namespace ark
{
    class Program
    {
        class Options
        {
            [Option('d',"decompress", Required = false, Default = false, HelpText = "Specify for Decompression.")]
            public bool decompressing { get; set; }
            [Option('c', "compress", Required = false, Default = false, HelpText = "Specify for Compressing.")]
            public bool compressing { get; set; }
            [Option('i', "input", Required = true, HelpText = "Input files to be processed.")]
            public IEnumerable<string> InputFiles { get; set; }
            [Option('o', "output", Default = "./", HelpText = "Output Location. Defaults to ./output.zip when compressing.")]
            public string output { get; set; }

            [Option('s', "store-paths", Default = false,
                HelpText =
                    "Stores Compressed files with the full path that they had on disk. Only useful during compression.")]
            public bool storepaths { get; set; }
            [Option('r', "recursive", Default = false, HelpText = "Recurse Directories when Compressing")]
            public bool RecurseDirectories { get; set; }
            // Omitting long name, defaults to name of property, ie "--verbose"
            [Option(
                Default = false,
                HelpText = "Prints all messages to standard output.")]
            public bool Verbose { get; set; }
        }
        static void Main(string[] args)
        {
            var argumentsParsed = CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options =>
                {
                    if (options.compressing)
                    {
                        CompressFiles(options.output, options.InputFiles.ToList(), options.Verbose, options.RecurseDirectories, options.storepaths);
                    } else if (options.decompressing)
                    {
                        DecompressFiles(options.output, options.InputFiles.ToList(), options.Verbose);
                    }
                });
        }

        private static void CompressFiles(string OutputFile, List<string> InputFiles, bool Verbose, bool Recursive, bool StoreFullPath)
        {
            if(Recursive && Verbose){Console.WriteLine("Updating File List Recursively...");}

            if (OutputFile == "./")
            {
                OutputFile = "./output.zip";
            }
            Console.WriteLine("Getting Files...");
            var MemoryStream = new MemoryStream();
            if(Verbose){ Console.WriteLine("Created MemoryStream");}
            using (var archive = ZipArchive.Create())
            {
                if (Recursive)
                {
                    //archive.WriteAll("D:\\temp", "*", SearchOption.AllDirectories);
                    foreach (string file in Directory.EnumerateFiles(InputFiles[0], "*.*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            if (Verbose)
                            {
                                Console.WriteLine("Adding " + file);
                            }

                            var inzipfile = file;
                            if (!StoreFullPath)
                            {
                                inzipfile = Path.GetRelativePath(InputFiles[0], file);
                            }
                            FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read);
                            archive.AddEntry(inzipfile, stream, stream.Length);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Failed to add " + file);
                            if (Verbose)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            else
                            {
                                Console.WriteLine("Use --verbose for more information.");
                            }
                        }
                    }
                }
                else
                {
                    foreach (var file in InputFiles)
                    {
                        if (!Directory.Exists(file))
                        {
                            var inzipfile = file;
                            if (!StoreFullPath)
                            {
                                inzipfile = Path.GetRelativePath(InputFiles[0], file);
                            }
                            if(Verbose){Console.WriteLine("Adding " + file);}
                            FileStream stream = new FileStream(file, FileMode.Open);
                            archive.AddEntry(inzipfile, stream, stream.Length);
                        }
                    }   
                }

                Console.WriteLine("Compressing...");
                archive.SaveTo(MemoryStream, new WriterOptions(CompressionType.Deflate));
            }
            MemoryStream.Position = 0;
            using (FileStream fs = new FileStream(OutputFile, FileMode.Create))
            {
                if(Verbose){Console.WriteLine("Writing to Disk...");}
                MemoryStream.CopyTo(fs);
                fs.Flush();
            }
            Console.WriteLine("Done!");
        }
        
        
        
        private static void DecompressFiles(string OutputFiles, List<string> InputFile, bool Verbose)
        {
            Console.WriteLine("Got Here");
            using (Stream stream = File.OpenRead(InputFile[0]))
            using (var reader = ReaderFactory.Open(stream))
            {
                if(Verbose){Console.WriteLine("Opened " + InputFile[0] + " for reading, writing to " + OutputFiles);}
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        Console.WriteLine(reader.Entry.Key);
                        reader.WriteEntryToDirectory(OutputFiles, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        });
                    }
                }
            }
        }


    }
}