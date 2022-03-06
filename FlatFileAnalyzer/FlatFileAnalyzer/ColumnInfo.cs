using System;

namespace FlatFileAnalyzer
{
    public class ColumnInfo
    {
        public string Name { get; set; }
        public int MaxStringLength { get; set; }
        public string LongestString { get; set; }
        public DateTime MinDateValue { get; set; }
        public DateTime MaxDateValue { get; set; }
        public decimal MinNumericValue { get; set; }
        public decimal MaxNumericValue { get; set; }
        public int MaxWholeNumbers { get; set; }
        public int MaxDecimalPlaces { get; set; }
        public int RecordsPopulated { get; set; }
        public bool IsDate { get; set; }
        public bool IsInteger { get; set; }
        public bool IsDecimal { get; set; }
        public bool IsBoolean { get; set; }
        public string MinValue
        {
            get
            {
                string val = "";
                if (RecordsPopulated > 0)
                {
                    if (IsDate && !IsDecimal)
                        val = MinDateValue.ToString();
                    else if (IsInteger)
                        val = MinNumericValue.ToString();
                    else if (IsDecimal)
                        val = MinNumericValue.ToString();
                }
                return val;
            }
        }
        public string MaxValue
        {
            get
            {
                string val = "";
                if (RecordsPopulated > 0)
                {
                    if (IsDate && !IsDecimal)
                        val = MaxDateValue.ToString();
                    else if (IsInteger)
                        val = MaxNumericValue.ToString();
                    else if (IsDecimal)
                        val = MaxNumericValue.ToString();
                    else if (IsBoolean)
                        val = "";
                    else
                        val = LongestString;
                }
                return val;
            }
        }
        public string SqlDataType
        {
            get
            {
                string sqlType = "";
                if (RecordsPopulated == 0)
                    sqlType = "nvarchar(1)";
                else if (IsBoolean)
                    sqlType = "bit";
                else if (IsDate && !IsDecimal)
                    sqlType = "datetime";
                else if (IsInteger)
                    sqlType = "int";
                else if (IsDecimal)
                {
                    int totalDigits = (MaxWholeNumbers + MaxDecimalPlaces < 38) ? MaxWholeNumbers + MaxDecimalPlaces : 38;
                    int decimalDigits = MaxDecimalPlaces < 38 ? MaxDecimalPlaces : 38;
                    sqlType = string.Format("decimal({0},{1})", totalDigits.ToString(), decimalDigits.ToString());
                }
                else if (MaxStringLength <= 4000)
                    sqlType = string.Format("nvarchar({0})", Math.Ceiling(MaxStringLength / 10.0) * 10);
                else
                    sqlType = "nvarchar(max)";

                return sqlType;
            }
        }
    }
}
