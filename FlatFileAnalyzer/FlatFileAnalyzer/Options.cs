using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace FlatFileAnalyzer
{
    public class Options
    {
        [Option('f', "file", Required = true, HelpText = "Input file to be analyzed.")]
        public string InputFile { get; set; }
        [Option('d', "delimiter", Default = ",", Required = false, HelpText = "Column delimiter.")]
        public string Delimiter { get; set; }
        [Option('q', "qualifier", Default = "", Required = false, HelpText = "Text qualifier.")]
        public string Qualifier { get; set; }
        [Option('h', "header", Default = true, Required = false, HelpText = "File has header line.")]
        public bool HasHeader { get; set; }
    }
}
