namespace RealwareApiAssistant.Models.Api.Request
{
    public class ApiRequest
    {
        public ApiRequest()
        {
            Ids = new List<ApiColumn>();
            Values = new List<ApiValue>();
        }
        public string Url { get; set; }
        public List<ApiColumn> Ids { get; set; }
        public List<ApiValue> Values { get; set; }
    }
}
