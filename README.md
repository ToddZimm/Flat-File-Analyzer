# Flat File Analyzer
Command line utility for analyzing delimited flat files for data types and completeness. Creates an HTML file with the analysis results showing list of columns, suggested data types, percent populated, minimum and maximum values. Also includes a basic SQL Create Table statement template.

Input file specifications can be provided on the command line with the options below. If no options are provided, the user will be prompted.

### Command Line Options

|               |                                     |
|---------------|-------------------------------------|
|-f, --file     |Required. Input file to be analyzed. |
|-d, --delimiter|(Default: ,) Column delimiter.       |
|-q, --qualifier|(Default: none) Text qualifier.      |
|-h, --header   |(Default: true) File has header line.|
|--help         |Display the help screen.             |
|--version      |Display version information.         |
