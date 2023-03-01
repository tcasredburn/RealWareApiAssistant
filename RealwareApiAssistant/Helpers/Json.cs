using RealwareApiAssistant.Core.Models.IO;
using RealwareApiAssistant.Models.Api.Request;
using RestSharp;

namespace RealwareApiAssistant.Helpers
{
    public class Json
    {
        public static string GetCleanName(string path)
        {
            if (path.IndexOf('[') >= 0)
                return path.Substring(0, path.LastIndexOf('['));
            return path;
        }

        public static bool IsArrayPath(string path)
            => path.Contains("[") || path.Contains("]");

        public static int GetPathArrayIndex(string path)
        {
            var iStart = path.LastIndexOf('[') + 1;
            var iEnd = path.LastIndexOf(']');
            var length = iEnd - iStart;

            // Return -1 when not found or object is not an array
            if (length < 0)
                return -1;

            var arrayIndex = path.Substring(iStart, length);

            return Convert.ToInt32(arrayIndex);
        }

        public static void ExportJsonToFile(ApiExportJsonToFileSettings settings, 
            List<ApiColumn> ids, Method method, string json, int index)
        {
            string name = method.ToString() + "_" + String.Join("_", ids.Select(x=>x.Value)) + $"_{index}" + ".json";
            string fullPath = Path.Combine(settings.FilePath, "Json", name);
            try
            {
                File.WriteAllText(fullPath, json);
            }
            catch { }
        }
    }
}
