namespace RealwareApiAssistant.Core.Models.Settings
{
    public class RealWareApiValue
    {
        public string Path { get; set; }
        public string RealWareColumn { get; set; }
        public string ExcelFromColumn { get; set; }
        public string ExcelToColumn { get; set; }
        public object FromValue { get; set; }
        public object ToValue { get; set; }
    }
}
