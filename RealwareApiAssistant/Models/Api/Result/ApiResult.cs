using RestSharp;

namespace RealwareApiAssistant.Models.Api.Result
{
    public class ApiResult
    {
        public ApiResult()
        {
            Message = string.Empty;
            ChangeCount = 0;
            InsertCount = 0;
        }
        public string Message { get; set; }
        public string MessageDetail { get; set; }
        public int ChangeCount { get; set; }
        public int InsertCount { get; set; }
        public IRestResponse Response { get; set; }
    }
}
