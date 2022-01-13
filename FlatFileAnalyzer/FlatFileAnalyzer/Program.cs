using System;
using System.Data;
using System.IO;
using System.Linq;

namespace FlatFileAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter file to analyze: ");
            string inputFile = Console.ReadLine();
            while (inputFile != "" && !File.Exists(inputFile))
            {
                Console.WriteLine("Input file not found.");
                Console.WriteLine("Enter file to analyze or press Enter to exit: ");
                inputFile = @"" + Console.ReadLine();
            }
            
            if (inputFile != "")
            {
                Console.WriteLine("Enter column delimiter: ");
                string delimiter = @"" + Console.ReadLine();

                Console.WriteLine("Enter text qualifier: ");
                string qualifier = @"" + Console.ReadLine();

                bool header = true;
                string headeryn = "";
                while (headeryn != "Y" && headeryn != "N")
                {
                    Console.WriteLine("File has header row? (Y/N): ");
                    headeryn = @"" + Console.ReadLine();
                }
                if (headeryn == "N")
                    header = false;

                string outputfile = Path.ChangeExtension(inputFile, "html");
                DataTable table = null;

                // Read the input file to a datatable
                try
                {
                    table = Analyzer.ReadFile(inputFile, header, delimiter, qualifier);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error reading file: " + ex.Message);
                    Console.WriteLine();
                    Console.WriteLine("Press enter to exit.");
                    _ = Console.ReadLine();
                }

                // Analyze column contents
                if (table != null)
                {
                    FlatFileInfo flatFile = new FlatFileInfo();
                    flatFile.InputFile = Path.GetFullPath(inputFile);
                    flatFile.RecordCount = table.Rows.Count;
                    flatFile.ColumnCount = table.Columns.Count;
                    try
                    {
                        flatFile.Columns = Analyzer.AnalyzeColumns(ref table);
                        flatFile.SampleRows = table.Rows.Cast<System.Data.DataRow>().Take(10).CopyToDataTable();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error analyzing contents: " + ex.Message);
                        Console.WriteLine();
                        Console.WriteLine("Press enter to exit.");
                        _ = Console.ReadLine();
                    }

                    // Output results to html file
                    File.WriteAllText(outputfile, flatFile.GetHtmlResults());
                    System.Diagnostics.Process.Start(outputfile);
                }
            }
        }
    }
}
