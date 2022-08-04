namespace RealwareApiAssistant.Models.IO
{
    public class ApiExportJsonToFileSettings
    {
        public bool ExportJsonFiles { get; set; } = false;
        public string FilePath { get; set; }

        public static ApiExportJsonToFileSettings Default()
        {
            return new ApiExportJsonToFileSettings
            {
                ExportJsonFiles = false,
                FilePath = null
            };
        }
    }
}
