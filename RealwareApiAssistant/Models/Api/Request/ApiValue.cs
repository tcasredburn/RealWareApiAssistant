namespace RealwareApiAssistant.Models.Api.Request
{
    public class ApiValue
    {
        public string Path { get; set; }
        public string RealwarePropertyName { get; set; }
        public object FromValue { get; set; }
        public object ToValue { get; set; }
    }
}
