using RestSharp;

namespace RealwareApiAssistant.Models.Api.Result
{
    public class ApiResult
    {
        public ApiResult()
        {
            Message = string.Empty;
            ChangeCount = 0;
        }
        public string Message { get; set; }
        public int ChangeCount { get; set; }
        public IRestResponse Response { get; set; }
    }
}
