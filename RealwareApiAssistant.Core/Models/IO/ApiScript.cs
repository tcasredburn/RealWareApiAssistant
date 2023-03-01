using RealwareApiAssistant.Core.Models.Settings;
using RestSharp;

namespace RealwareApiAssistant.Core.Models.IO
{
    /// <summary>
    /// Script settings file.
    /// </summary>
    public class ApiScript
    {
        public RealWareApiSettings ApiSettings { get; set; }
        public string ExcelFile { get; set; }
        public string ModelFile { get; set; }
        public List<RealWareApiId> IdColumns { get; set; }
        public List<RealWareApiValue> ValueChanges { get; set; }
        public List<RealWareApiValue> ValueInserts { get; set; }
        public string ApiOperation { get; set; }
        public Method? Method { get; set; }
        public string CustomLogFileLocation { get; set; }
        public bool SkipConfirmations { get; set; }
        public bool SkipWarningPrompts { get; set; }
        public bool RetryImmediatelyAfterBadRequest { get; set; }
        public int ExcelFileRowUpdateCount { get; set; } = 5000;
        public bool ForceExcelNULLValues { get; set; } = true;
        public ApiExportJsonToFileSettings ExportJsonSettings { get; set; }
        public int Threads { get; set; } = 1;


        public List<RealWareApiId> GetStaticIdColumns()
            => IdColumns.FindAll(x => x.StaticValue != null);

        public bool IsRealWareApiId(string name)
            => IdColumns.Find(x => x.ColumnValue == name && x.StaticValue == null) != null;

        public RealWareApiId GetRealWareApiId(string name)
            => IdColumns.First(x => x.ColumnValue == name && x.StaticValue == null);

        public bool IsRealWareApiValueChange(string name)
            => ValueChanges.Find(x => x.ExcelFromColumn == name || x.ExcelToColumn == name) != null;

        public bool IsRealWareApiValueInsert(string name)
            => ValueInserts.Find(x => x.ExcelFromColumn == name || x.ExcelToColumn == name) != null;

        public List<RealWareApiValue> GetRealWareApiValueChanges(string name)
        {
            var result = ValueChanges.FindAll(x => x.ExcelFromColumn == name);
            if (result.Count == 0)
                result = ValueChanges.FindAll(x => x.ExcelToColumn == name);
            return result ?? new List<RealWareApiValue>();
        }

        public List<RealWareApiValue> GetRealWareApiValueInserts(string name)
        {
            var result = ValueInserts.FindAll(x => x.ExcelFromColumn == name);
            if (result.Count == 0)
                result = ValueInserts.FindAll(x => x.ExcelToColumn == name);
            return result ?? new List<RealWareApiValue>();
        }

        internal string scriptFilePath = null;
        internal string modelFileData = null;

        public static ApiScript GetDefault()
        {
            return new ApiScript
            {
                ApiOperation = "PUT",
                ApiSettings = new RealWareApiSettings
                {
                    ApiPath = "http://{server name}/Production/EncompassAPI",
                    Token = "{insert token here}"
                },
                RetryImmediatelyAfterBadRequest = true,
                CustomLogFileLocation = null,
                ExcelFile = "{insert excel file path here}",
                ExportJsonSettings = new ApiExportJsonToFileSettings
                {
                    ExportJsonFiles = false,
                    FilePath = "/json"
                },
                ForceExcelNULLValues= true,
                SkipConfirmations = false,
                SkipWarningPrompts= false,
                Method = RestSharp.Method.PUT,
                Threads = 8,
                ExcelFileRowUpdateCount = 5000,
                
            };
        }
    }
}
