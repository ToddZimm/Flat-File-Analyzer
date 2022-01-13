using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace FlatFileAnalyzer
{
    public static class Extensions
    {
        public static String GetHtml(this DataTable dataTable)
        {
            StringBuilder sbControlHtml = new StringBuilder();
            using (StringWriter stringWriter = new StringWriter())
            {
                using (HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter))
                {
                    using (var htmlTable = new HtmlTable())
                    {
                        // Add table header row
                        using (var headerRow = new HtmlTableRow())
                        {
                            foreach (DataColumn dataColumn in dataTable.Columns)
                            {
                                using (var htmlColumn = new HtmlTableCell())
                                {
                                    htmlColumn.InnerText = dataColumn.ColumnName;
                                    headerRow.Cells.Add(htmlColumn);
                                }
                            }
                            htmlTable.Rows.Add(headerRow);
                        }
                        // Add data rows
                        foreach (DataRow row in dataTable.Rows)
                        {
                            using (var htmlRow = new HtmlTableRow())
                            {
                                foreach (DataColumn column in dataTable.Columns)
                                {
                                    using (var htmlColumn = new HtmlTableCell())
                                    {
                                        htmlColumn.InnerText = row[column].ToString();
                                        htmlRow.Cells.Add(htmlColumn);
                                    }
                                }
                                htmlTable.Rows.Add(htmlRow);
                            }
                        }
                        htmlTable.RenderControl(htmlWriter);
                        sbControlHtml.Append(stringWriter.ToString());
                    }
                }
            }
            return sbControlHtml.ToString();
        }
    }
}

