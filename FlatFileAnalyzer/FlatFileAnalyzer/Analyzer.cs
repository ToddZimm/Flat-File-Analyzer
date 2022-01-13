using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace FlatFileAnalyzer
{
    public static class Analyzer
    {
        public static DataTable ReadFile(string InputFile, bool HasHeader = true, string delimiter = ",", string qualifier = "")
        {
            DataTable dt = new DataTable();
            long rowCount = 0;
            int colCount = 0;
            string[] separators = new string[] {string.Concat(qualifier, delimiter, qualifier)};

            foreach (string line in File.ReadLines(InputFile))
            {
                rowCount++;
                string row;

                //remove qualifier from beginning and end of line
                if (qualifier != string.Empty)
                    row = line.Substring(qualifier.Length, line.Length - (qualifier.Length * 2));
                else
                    row = line;
                
                string[] cols = row.Split(separators,StringSplitOptions.None);

                //Create columns in datatable from first row
                if (rowCount == 1)
                {
                    foreach (string col in cols)
                    {
                        colCount++;
                        string colName = HasHeader ? col.Trim() : "Col" + colCount.ToString();
                        dt.Columns.Add(colName, System.Type.GetType("System.String"));
                    }
                }

                //Add data rows
                if (rowCount > 1 || !HasHeader)
                {
                    DataRow dr = dt.NewRow();
                    for (int k = 0; k < cols.Count(); k++)
                    {
                        dr[k] = cols[k].ToString();
                    }
                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }

        public static List<ColumnInfo> AnalyzeColumns (ref DataTable table)
        {
            List<ColumnInfo> columns = new List<ColumnInfo>();
            List<string> boolValues = new List<string>() { "0", "1", "true", "false", "yes", "no" };

            foreach (DataColumn col in table.Columns)
            {
                columns.Add(new ColumnInfo { Name = col.ColumnName, 
                    IsBoolean = true, IsDate = true, IsDecimal = true, IsInteger = true, 
                    MaxStringLength = 0, LongestString = "", MaxWholeNumbers = 0, MaxDecimalPlaces = 0, 
                    MaxNumericValue = decimal.MinValue, MinNumericValue = decimal.MaxValue,
                    MaxDateValue = DateTime.MinValue, MinDateValue = DateTime.MaxValue, RecordsPopulated = 0 });
            }

            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    if (row[i].ToString().Length > columns[i].MaxStringLength)
                    {
                        columns[i].MaxStringLength = row[i].ToString().Length;
                        columns[i].LongestString = row[i].ToString();
                    }

                    string cellValue = row[i].ToString().Trim();
                    if (cellValue.Length > 0)
                    {
                        columns[i].RecordsPopulated++;

                        // Check for boolean data type
                        if (!boolValues.Contains(cellValue))
                            columns[i].IsBoolean = false;

                        // Check for datetime data type
                        if (DateTime.TryParse(cellValue, out DateTime date))
                        {
                            if (date > columns[i].MaxDateValue)
                                columns[i].MaxDateValue = date;
                            if (date < columns[i].MinDateValue)
                                columns[i].MinDateValue = date;
                        }
                        else
                            columns[i].IsDate = false;

                        // Check for integer data type
                        if (int.TryParse(cellValue, out int number))
                        {
                            if (number > columns[i].MaxNumericValue)
                                columns[i].MaxNumericValue = number;
                            if (number < columns[i].MinNumericValue)
                                columns[i].MinNumericValue = number;
                            if (Math.Abs(number).ToString().Count() > columns[i].MaxWholeNumbers)
                                columns[i].MaxWholeNumbers = Math.Abs(number).ToString().Count();
                        }
                        else
                            columns[i].IsInteger = false;

                        // Check decimal data type
                        if (cellValue.Contains(".") && decimal.TryParse(cellValue, out decimal decnumber))
                        {
                            if (decnumber > columns[i].MaxNumericValue)
                                columns[i].MaxNumericValue = decnumber;
                            if (decnumber < columns[i].MinNumericValue)
                                columns[i].MinNumericValue = decnumber;
                            int decimalPlaces = cellValue.Length - cellValue.IndexOf('.') - 1;
                            int wholeNumbers = cellValue.Replace("-", "").Replace(",", "").IndexOf('.');
                            if (decimalPlaces > columns[i].MaxDecimalPlaces)
                                columns[i].MaxDecimalPlaces = decimalPlaces;
                            if (wholeNumbers > columns[i].MaxWholeNumbers)
                                columns[i].MaxWholeNumbers = wholeNumbers;
                        }
                        else
                            columns[i].IsDecimal = false;
                    }
                }
            }

            return columns;
        }
    }
}
