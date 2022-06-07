using RealwareApiAssistant.Models.Settings;
using RestSharp;

namespace RealwareApiAssistant.Models.IO
{
    /// <summary>
    /// Script settings file.
    /// </summary>
    public class ApiScript
    {
        public RealWareApiSettings ApiSettings { get; set; }
        public string ExcelFile { get; set; }
        public List<RealWareApiId> IdColumns { get; set; }
        public List<RealWareApiValue> ValueChanges { get; set; }
        public string ApiOperation { get; set; }
        public Method? Method { get; set; }
        public string CustomLogFileLocation { get; set; }
        public bool SkipConfirmations { get; set; }
        public bool SkipWarningPrompts { get; set; }
        public bool RetryImmediatelyAfterBadRequest { get; set; }
        public int ExcelFileRowUpdateCount { get; set; } = 250;



        public List<RealWareApiId> GetStaticIdColumns()
            => IdColumns.FindAll(x => x.StaticValue != null);

        public bool IsRealWareApiId(string name)
            => IdColumns.Find(x => x.ColumnValue == name && x.StaticValue == null) != null;

        public RealWareApiId GetRealWareApiId(string name)
            => IdColumns.First(x => x.ColumnValue == name && x.StaticValue == null);

        public bool IsRealWareApiValueChange(string name)
            => ValueChanges.Find(x => x.ExcelFromColumn == name || x.ExcelToColumn == name) != null;

        public RealWareApiValue GetRealWareApiValueChange(string name)
        {
            var result = ValueChanges.Find(x => x.ExcelFromColumn == name);
            if (result == null)
                result = ValueChanges.Find(x => x.ExcelToColumn == name);
            return result;
        }

        internal string scriptFilePath = null;
    }
}
