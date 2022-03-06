using System.Collections.Generic;
using System.Data;
using System.Text;
using System.IO;
using System.Linq;

namespace FlatFileAnalyzer
{
    public class FlatFileInfo
    {
        public string InputFile { get; set; }
        public int RecordCount { get; set; }
        public int ColumnCount { get; set; }
        public List<ColumnInfo> Columns { get; set; }
        public DataTable ParsedData { get; set; }
        public List<string> InvalidLines { get; set; }
        public string GetSqlTableStatement () 
        {
            string tableName = Path.GetFileNameWithoutExtension(InputFile).Replace(' ','_');
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("CREATE TABLE " + tableName + " (");
            for (int i = 0; i < Columns.Count; i++)
            {
                if (i == 0)
                    sql.AppendLine("   [" + Columns[i].Name.Replace(" ", "_") + "] " + Columns[i].SqlDataType + " NULL");
                else
                    sql.AppendLine("  ,[" + Columns[i].Name.Replace(" ", "_") + "] " + Columns[i].SqlDataType + " NULL");
            }
            sql.AppendLine(")");
            return sql.ToString();
        }

        public string GetHtmlResults()
        {
            StringBuilder html = new StringBuilder();
            DataTable sampleData = ParsedData.Rows.Cast<System.Data.DataRow>().Take(10).CopyToDataTable();

            html.AppendLine("<html>\r\n<head>\r\n<style>");
            html.AppendLine("body { font-family: arial,sans; font-size: 11pt; }");
            html.AppendLine("table { border: 1px solid black; border-collapse: collapse; font-family: arial,sans; font-size: 11pt; }");
            html.AppendLine("table, td, th { border: 1px solid black; padding: 5px; font-family: arial,sans; font-size: 11pt; }");
            html.Append("</style>\r\n</head>\r\n<body>");
            html.AppendLine("<h2>File Information</h2>");
            
            // Basic file info
            html.AppendLine("<div>\r\n<table>");
            html.AppendFormat("<tr><th>Input file:</th><td>{0}</td></tr>\r\n", InputFile);
            html.AppendFormat("<tr><th>Records in file:</th><td>{0}</td></tr>\r\n", RecordCount.ToString("N0"));
            html.AppendFormat("<tr><th>Columns in file:</th><td>{0}</td></tr>\r\n", ColumnCount.ToString("N0"));
            html.AppendLine("</table>\r\n</div>");

            // Column details
            html.AppendLine("<h2>Column Information</h2>");
            html.AppendLine("<div>\r\n<table>");
            html.AppendLine("<tr><th>Column Name</th><th>SQL Data Type</th><th>Populated</th><th>Max String Lenth</th><th>Min Value</th><th>Max Value / Longest String</th></tr>");
            foreach (ColumnInfo col in Columns)
            {
                html.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td></tr>\r\n",
                    col.Name, col.SqlDataType,
                    ((decimal)col.RecordsPopulated / RecordCount).ToString("P0") + " (" + col.RecordsPopulated.ToString() + ")", 
                    col.MaxStringLength, col.MinValue, col.MaxValue);
            }
            html.AppendLine("</table>\r\n</div>");

            // Table SQL
            html.AppendLine("<h2>Table SQL Statement</h2>");
            html.AppendFormat("<div>\r\n<pre>\r\n{0}\r\n</pre>\r\n</div>\r\n", GetSqlTableStatement());

            // Sample data
            html.AppendLine("<h2>Sample Data</h2>");
            if (ParsedData.Rows.Count > 0)
            {
                html.Append(sampleData.GetHtml());
            }

            // Invalid file lines
            html.AppendLine("<h2>Invalid File Lines</h2>");
            if (InvalidLines != null && InvalidLines.Count > 0)
            {
                html.AppendLine("<div>\r\n<table>");
                foreach (string line in InvalidLines)
                {
                    html.AppendFormat("<tr><td>{0}&nbsp;</td></tr>\r\n", line);
                }
                html.AppendLine("</table>\r\n</div>");
            }
            else
            {
                html.AppendLine("<div>No invalid lines found.</div>");
            }
            
            html.AppendLine("</body>\r\n</htlm>");
            return html.ToString();
        }
    }
}
