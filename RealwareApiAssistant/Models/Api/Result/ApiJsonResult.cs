namespace RealwareApiAssistant.Models.Api.Result
{
    public class ApiJsonResult
    {
        public string Json { get; set; }
        public int ChangeCount { get; set; }
        public int InsertCount { get; set; }
        public int TotalChangeCount { get; set; }
        public int TotalInsertCount { get; set; }
    }
}
