using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using CommandLine;

namespace FlatFileAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunAnalysis);
            }
            else
            {
                Options options = new Options();

                Console.WriteLine("Enter file to analyze: ");
                options.InputFile = Console.ReadLine();
                while (options.InputFile != "" && !File.Exists(options.InputFile))
                {
                    Console.WriteLine("Input file not found.");
                    Console.WriteLine("Enter file to analyze or press Enter to exit: ");
                    options.InputFile = @"" + Console.ReadLine();
                }

                if (options.InputFile != "")
                {
                    Console.WriteLine("Enter column delimiter: ");
                    options.Delimiter = @"" + Console.ReadLine();

                    Console.WriteLine("Enter text qualifier: ");
                    options.Qualifier = @"" + Console.ReadLine();

                    options.HasHeader = true;
                    string headeryn = "";
                    while (headeryn != "Y" && headeryn != "N")
                    {
                        Console.WriteLine("File has header row? (Y/N): ");
                        headeryn = @"" + Console.ReadLine();
                    }
                    if (headeryn == "N")
                        options.HasHeader = false;

                    RunAnalysis(options);
                }
            }
        }

        private static void RunAnalysis(Options options)
        {
            string outputfile = Path.ChangeExtension(options.InputFile, "html");
            FlatFileInfo fileInfo = null;

            // Read the input file to a datatable
            try
            {
                fileInfo = Analyzer.ReadFile(options);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading file: " + ex.Message);
                _ = Console.ReadKey();
            }

            // Analyze column contents
            if (fileInfo.ParsedData != null)
            {
                try
                {
                    fileInfo.Columns = Analyzer.AnalyzeColumns(fileInfo.ParsedData);

                    // Output results to html file
                    File.WriteAllText(outputfile, fileInfo.GetHtmlResults());
                    _ = System.Diagnostics.Process.Start(outputfile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error analyzing columns: " + ex.Message);
                    _ = Console.ReadKey();
                }

                
            }
        }
    }
}
