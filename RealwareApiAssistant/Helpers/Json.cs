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
            var arrayIndex = path.Substring(iStart, length);

            return Convert.ToInt32(arrayIndex);
        }
    }
}
