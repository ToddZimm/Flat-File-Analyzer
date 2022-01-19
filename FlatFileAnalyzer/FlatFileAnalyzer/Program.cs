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
            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunAnalysis);
        }

        private static void RunAnalysis(Options options)
        {
            string outputfile = Path.ChangeExtension(options.InputFile, "html");
            DataTable table = null;

            // Read the input file to a datatable
            try
            {
                table = Analyzer.ReadFile(options);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading file: " + ex.Message);
            }

            // Analyze column contents
            if (table != null)
            {
                FlatFileInfo flatFile = new FlatFileInfo();
                flatFile.InputFile = Path.GetFullPath(options.InputFile);
                flatFile.RecordCount = table.Rows.Count;
                flatFile.ColumnCount = table.Columns.Count;
                try
                {
                    flatFile.Columns = Analyzer.AnalyzeColumns(ref table);
                    flatFile.SampleRows = table.Rows.Cast<System.Data.DataRow>().Take(10).CopyToDataTable();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error analyzing columns: " + ex.Message);
                }

                // Output results to html file
                File.WriteAllText(outputfile, flatFile.GetHtmlResults());
                System.Diagnostics.Process.Start(outputfile);
            }
        }
    }
}
