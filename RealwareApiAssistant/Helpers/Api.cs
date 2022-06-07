namespace RealwareApiAssistant.Helpers
{
    public class Api
    {
        public static string GetUrlPath(string realwareModule, List<Models.Api.Request.ApiColumn> ids)
        {
            string temp = realwareModule;
            foreach (var id in ids)
                temp = temp.Replace(id.ColumnId, id.Value);
            return temp;
        }
    }
}
